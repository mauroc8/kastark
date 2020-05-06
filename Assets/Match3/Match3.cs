using System;
using UnityEngine;

public class Match3 : UpdateAsStream
{
    protected EventStream<Match3Cursor> cursor =
        new EventStream<Match3Cursor>();

    protected StateStream<uint> availableMovements =
        new StateStream<uint>(4);

    void Start()
    {
        cursor.Push(new Match3NotClicking());
    }

    void Awake()
    {
        // Interacting
        var worldScene = GetComponentInParent<WorldScene>();
        var worldState = worldScene.State;

        var board =
            GetComponentInChildren<Board>();

        var isAboutToInteract =
            worldState
                .Map(state =>
                    Functions.IsTypeOf<InteractState, InteractStates.InFrontOfMatch3>(state)
                        && ((InteractStates.InFrontOfMatch3)state).board == board
                )
                .Lazy();

        var isInteracting =
            worldState
                .Map(state =>
                    Functions.IsTypeOf<InteractState, InteractStates.PlayingMatch3>(state)
                        && ((InteractStates.PlayingMatch3)state).board == board
                )
                .Lazy();

        var interactionUpdate =
            isInteracting
                .AndThen(interacting =>
                    interacting
                        ? update
                        : Stream.None<Void>()
                );

        // --- Show Text

        var interactText = Node.Query(this, "interact-text");

        isAboutToInteract
            .Get(value =>
            {
                interactText.SetActive(value);
            });

        // --- Hide Aira on Interaction ---

        var lerpedInteracting =
            isInteracting
                .Map(value => value ? 1.0f : 0.0f)
                .AndThen(Functions.LerpStreamOverTime(update, 0.8f));


        var aira = Node.Query(transform.root, "aira");

        lerpedInteracting
            .Map(t => t == 0)
            .Lazy()
            .Get(value =>
            {
                aira.SetActive(value);
            });

        // Move Camera

        var camera =
            Camera.main;

        var interactingCameraTransform =
            Node.Query(this, "camera").transform;

        var interactingCameraComponent =
            Query.From(this, "camera").Get<Camera>();

        Stream.Combine(
            isInteracting
                .Filter(value => value)
                .Map(_ =>
                    (
                        camera.transform.position,
                        camera.transform.rotation,
                        camera.fieldOfView
                    )
                )
            ,
            lerpedInteracting
                .Filter(t => t >= 0)
        )
            .Get((cam, t) =>
            {
                t = Functions.EaseInOut(t);

                camera.transform.position =
                    Vector3.Lerp(
                        cam.Item1,
                        interactingCameraTransform.position,
                        t
                    );

                camera.transform.rotation =
                    Quaternion.Slerp(
                        cam.Item2,
                        interactingCameraTransform.rotation,
                        t
                    );

                camera.fieldOfView =
                    Mathf.Lerp(
                        cam.Item3,
                        interactingCameraComponent.fieldOfView,
                        t
                    );
            });


        // --- Mouse Controller ---

        var hover =
            interactionUpdate
                .Map(_ =>
                    RaycastHelper
                        .OptionalGameObjectAtScreenPoint(
                            Input.mousePosition,
                            1 << 10
                        )
                        .Map(block =>
                            (
                                block.transform.localPosition,
                                block.CompareTag("EmptyBlock")
                            )
                        )
                )
                .Lazy();

        Stream
            .Combine(hover, cursor)
            .AndThen(update.Always)
            .Get((optionalHover, currentCursor) =>
            {
                if (Input.GetMouseButtonDown(0))
                {
                    optionalHover.Get(hoverValue =>
                    {
                        var (value, isEmpty) = hoverValue;

                        if (!isEmpty)
                            cursor.Push(new Match3Clicking
                            {
                                anchor = new Vector2(value.x, value.y),
                                focus = new Vector2(value.x, value.y)
                            });
                    });
                }
                else if (Input.GetMouseButton(0))
                {
                    optionalHover.Get(hoverValue =>
                    {
                        var (value, isEmpty) = hoverValue;

                        switch (currentCursor)
                        {
                            case Match3Clicking click:
                                var focusIsInRange =
                                    Vector2.Distance(click.anchor, value) <= 1.01f;

                                cursor.Push(new Match3Clicking
                                {
                                    anchor = click.anchor,
                                    focus =
                                        focusIsInRange
                                            ? new Vector2(value.x, value.y)
                                            : click.focus
                                });
                                break;
                        }
                    });
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    cursor.Push(new Match3NotClicking());
                }
                else
                {
                    optionalHover.Get(hoverValue =>
                    {
                        var (value, isEmpty) = hoverValue;

                        cursor.Push(
                            new Match3Hovering
                            {
                                anchor = new Vector2(value.x, value.y)
                            }
                        );
                    });
                }
            });

        // --- Show cursor ---

        var cursorGameObject = Node.Query(this, "cursor");

        cursor
            .AndThen(update.Always)
            .Get(currentCursor =>
            {
                if (currentCursor.ShowCursor)
                    cursorGameObject.transform.localPosition =
                        Vector3.Lerp(
                            cursorGameObject.transform.localPosition,
                            currentCursor.CursorLocalPosition,
                            13.2f * Time.deltaTime
                        );
            });

        isInteracting
            .Get(interacting =>
            {
                cursorGameObject.SetActive(interacting);
            });

        // --- Highlight selected block ---

        var selectionGameObject = Node.Query(this, "selection");

        var cachedCursorMaterial =
            cursorGameObject.GetComponent<Renderer>().material;
        var cachedSelectionMaterial =
            selectionGameObject.GetComponent<Renderer>().material;

        cursor
            .Get(currentCursor =>
            {
                selectionGameObject.SetActive(currentCursor.ShowSelection);

                if (currentCursor.ShowSelection)
                {
                    selectionGameObject.transform.localPosition =
                        currentCursor.SelectionLocalPosition;

                    // Set cursor's game object material == selection
                    cursorGameObject.GetComponent<Renderer>().material =
                        cachedSelectionMaterial;
                }
                else
                {
                    // Reset cursor's material
                    cursorGameObject.GetComponent<Renderer>().material =
                        cachedCursorMaterial;
                }
            });

        // Commit movement

        cursor
            .WithLastValue(new Match3NotClicking())
            .Get((lastCursor, currentCursor) =>
            {
                if (
                    Functions.IsTypeOf<Match3Cursor, Match3Clicking>(lastCursor)
                        && Functions.IsTypeOf<Match3Cursor, Match3NotClicking>(currentCursor)
                    )
                {
                    // OnClickEnd
                    var click = (Match3Clicking)lastCursor;

                    if (availableMovements.Value > 0)
                    {
                        var anchor =
                            new Vector3(
                                Mathf.Floor(click.anchor.x),
                                Mathf.Floor(click.anchor.y),
                                0.0f
                            );

                        var focus =
                            new Vector3(
                                Mathf.Floor(click.focus.x),
                                Mathf.Floor(click.focus.y),
                                0.0f
                            );

                        if (
                            board.TryCommitMovement(anchor, focus)
                        )
                            availableMovements.Value--;
                    }
                }
            });

        // Show available movements!

        var availableMovementsNode =
            Node.Query(this, "available-movements");

        isInteracting.Get(availableMovementsNode.SetActive);

        var availableMovementsText =
            availableMovementsNode.GetComponent<TMPro.TextMeshProUGUI>();

        var localization =
            GetComponentInParent<LocalizationSource>()
                .localization;

        var movements =
            localization.GetLocalizedString("Movements");

        availableMovements
            .Get(value =>
            {
                availableMovementsText.text =
                    $"{movements}: {value}";
            });


        // Press ESC to leave!

        var escapeButton =
            Query
                .From(transform.root, "esc-return")
                .Get<EscReturnButton>();

        isInteracting
            .Get(value =>
            {
                if (value)
                    escapeButton.label.Value =
                        "Return";
            });

        var escapeClickUnfiltered =
            Query
                .From(transform.root, "esc-return")
                .Get<HoverAndClickEventTrigger>()
                .click
                .Always(new Void());

        var escapeButtonClick =
            isInteracting
                .AndThen(interacting =>
                    interacting
                        ? escapeClickUnfiltered
                        : Stream.None<Void>()
                );

        Stream.Merge(
            interactionUpdate
                .Filter(_ => Input.GetKeyDown(KeyCode.Escape))
            ,
            escapeButtonClick
        )
            .Get(_ =>
            {
                worldScene.ExitInteractState();
            });

        // Save progress

        isInteracting
            .Get(_ =>
            {
                Globals.Save(
                    board.boardTag == BoardTag.FirstBoard
                        ? Checkpoint.FirstMatch3
                        : Checkpoint.SecondMatch3
                );
            });
    }
}

public interface Match3Cursor
{
    bool ShowCursor { get; }
    Vector3 CursorLocalPosition { get; }

    bool ShowSelection { get; }
    Vector3 SelectionLocalPosition { get; }
}

public struct Match3NotClicking : Match3Cursor
{
    public bool ShowCursor => false;

    public Vector3 CursorLocalPosition => Vector3.zero;

    public bool ShowSelection => false;

    public Vector3 SelectionLocalPosition => Vector3.zero;
}

public struct Match3Hovering : Match3Cursor
{
    public Vector2 anchor;

    public bool ShowCursor => true;

    public Vector3 CursorLocalPosition =>
        new Vector3(anchor.x, anchor.y, 0);

    public bool ShowSelection => false;

    public Vector3 SelectionLocalPosition => Vector3.zero;
}

public struct Match3Clicking : Match3Cursor
{
    public Vector2 anchor;
    public Vector2 focus;

    public bool ShowCursor => true;

    public Vector3 CursorLocalPosition =>
        new Vector3(focus.x, focus.y, 0);

    public bool ShowSelection => true;

    public Vector3 SelectionLocalPosition =>
        new Vector3(anchor.x, anchor.y, 0);
}
