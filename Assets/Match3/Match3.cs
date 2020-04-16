using System;
using UnityEngine;

public class Match3 : UpdateAsStream
{
    protected StreamSource<Match3Cursor> cursor =
        new StreamSource<Match3Cursor>();

    void Start()
    {
        cursor.Push(new Match3NotClicking());
    }

    void Awake()
    {
        // Interacting
        var worldScene = GetComponentInParent<WorldScene>();
        var worldState = worldScene.State;

        var isInteracting =
            worldState
                .Map(state => state.interactState)
                .Map(Functions.IsTypeOf<InteractState, InteractStates.PlayingMatch3>)
                .Lazy();

        var interactionUpdate =
            isInteracting
                .AndThen(interacting =>
                    interacting
                        ? update
                        : Stream.None<Void>()
                );

        // --- Mouse Controller ---

        var hover =
            interactionUpdate
                .Map(_ =>
                    RaycastHelper
                        .OptionalGameObjectAtScreenPoint(
                            Input.mousePosition,
                            1 << 10
                        )
                        .Map(block => block.transform.localPosition)
                )
                .Lazy();

        Stream
            .Combine(hover, cursor)
            .AndThen(update.Always)
            .Get((position, currentCursor) =>
            {
                if (Input.GetMouseButtonDown(0))
                {
                    position.Get(value =>
                    {
                        cursor.Push(new Match3Clicking
                        {
                            anchor = new Vector2(value.x, value.y),
                            focus = new Vector2(value.x, value.y)
                        });
                    });
                }
                else if (Input.GetMouseButton(0))
                {
                    position.Get(value =>
                    {
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
                    position.Get(value =>
                    {
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

        var board =
            GetComponentInChildren<Board>();

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

                    board.CommitMovement(
                        new Vector3(
                            Mathf.Floor(click.anchor.x),
                            Mathf.Floor(click.anchor.y),
                            0.0f
                        ),
                        new Vector3(
                            Mathf.Floor(click.focus.x),
                            Mathf.Floor(click.focus.y),
                            0.0f
                        )
                    );
                }
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
