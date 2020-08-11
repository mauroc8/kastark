using System;
using UnityEngine;
using Vector3Extensions;

public enum MinigameTag
{
    Garbage,
    PuzzleA,
    PuzzleB2,
    PuzzleC
};

public struct MinigameState
{
    public Tag tag;

    public Block block;
    public (int, int) delta;

    public enum Tag
    {
        Idle,
        Hovering,
        Clicking,
        DragError,
        Committing,
        Animating
    }

    public static MinigameState Idle()
    {
        return new MinigameState
        {
            tag = Tag.Idle
        };
    }

    public static MinigameState Hovering(Block block)
    {
        return new MinigameState
        {
            tag = Tag.Hovering,
            block = block
        };
    }

    public static MinigameState Clicking(Block block, (int, int) delta)
    {
        return new MinigameState
        {
            tag = Tag.Clicking,
            block = block,
            delta = delta
        };
    }

    public static MinigameState DragError(Block block, (int, int) delta)
    {
        return new MinigameState
        {
            tag = Tag.DragError,
            block = block,
            delta = delta
        };
    }

    public static MinigameState Committing(Block block, (int, int) delta)
    {
        return new MinigameState
        {
            tag = Tag.Committing,
            block = block,
            delta = delta
        };
    }

    public static MinigameState Animating(Block block, (int, int) delta)
    {
        return new MinigameState
        {
            tag = Tag.Animating,
            block = block,
            delta = delta
        };
    }
}

public class Minigame : UpdateAsStream
{
    protected StateStream<MinigameState> state =
        new StateStream<MinigameState>(MinigameState.Idle());

    public MinigameTag minigameTag;

    protected virtual void Awake()
    {
        var board =
            Query
                .From(this)
                .Get<Board>();

        board.Init(minigameTag);

        // Interacting

        var worldScene = GetComponentInParent<WorldScene>();
        var worldState = worldScene.State;

        var isAboutToInteract =
            worldState
                .Map(state =>
                    Functions.IsTypeOf<InteractState, InteractStates.InFrontOfMinigame>(state)
                        && ((InteractStates.InFrontOfMinigame)state).minigameTag == minigameTag
                )
                .Lazy();

        var isInteracting =
            worldState
                .Map(state =>
                    Functions.IsTypeOf<InteractState, InteractStates.PlayingMinigame>(state)
                        && ((InteractStates.PlayingMinigame)state).minigameTag == minigameTag
                )
                .Lazy();

        var interactionUpdate =
            isInteracting
                .AndThen(interacting =>
                    interacting
                        ? update
                        : Stream.None<Void>()
                );

        var lerpedInteracting =
            isInteracting
                .Map(value => value ? 1.0f : 0.0f)
                .AndThen(Functions.LerpStreamOverTime(update, 0.8f));

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

        // --- State Controller ---

        isInteracting
            .Get(_ =>
            {
                state.Value =
                    MinigameState.Idle();
            });

        var hoveredSlot =
            interactionUpdate
                .Map(_ =>
                    RaycastHelper
                        .OptionalGameObjectAtScreenPoint(
                            Input.mousePosition,
                            1 << 10
                        )
                        .Map(slot =>
                            (
                                (int)slot.transform.localPosition.x,
                                (int)slot.transform.localPosition.y
                            )
                        )
                );


        // HOVER & IDLE

        hoveredSlot
            .Filter(_ => !Input.GetMouseButton(0) && !Input.GetMouseButtonUp(0)
                && (state.Value.tag == MinigameState.Tag.Idle
                    || state.Value.tag == MinigameState.Tag.Hovering))
            .Get(hoverCoords =>
            {
                hoverCoords
                    .AndThen(board.TryGetBlockAt)
                    .CaseOf(
                        value =>
                        {
                            state.Value =
                                MinigameState.Hovering(value);
                        },
                        () =>
                        {
                            state.Value =
                                MinigameState.Idle();
                        }
                    );
            });

        // CLICK

        state
            .AndThen(value =>
                value.tag == MinigameState.Tag.Hovering
                    ? interactionUpdate.Always(value.block)
                    : Stream.None<Block>()
            )
            .Filter(_ => Input.GetMouseButtonDown(0))
            .Get(block =>
            {
                state.Value =
                    MinigameState.Clicking(block, (0, 0));
            });

        // DRAG

        hoveredSlot
            .FilterMap(a => a)
            .Filter(_ =>
                Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0)
                    && (state.Value.tag == MinigameState.Tag.Clicking
                        || state.Value.tag == MinigameState.Tag.DragError)
            )
            .Get(target =>
            {
                var (x, y) =
                    target;

                var block =
                    state.Value.block;

                board
                    .TryGetValidDelta(block, target)
                    .CaseOf(
                        delta =>
                        {
                            state.Value =
                                MinigameState.Clicking(block, delta);
                        },
                        () =>
                        {
                            if (!block.CollidesAt(x, y)
                                && Mathf.Abs(block.x - x) + Mathf.Abs(block.y - y) < 2
                            )
                            {
                                var delta =
                                    (x - block.x, y - block.y);

                                state.Value =
                                    MinigameState.DragError(block, delta);
                            }
                        }
                    );
            });

        // DROP

        state
            .AndThen(value =>
                value.tag == MinigameState.Tag.Clicking
                    || value.tag == MinigameState.Tag.DragError
                    ? interactionUpdate
                    : Stream.None<Void>()
            )
            .Filter(_ => Input.GetMouseButtonUp(0))
            .Get(_ =>
            {
                state.Value =
                    state.Value.tag == MinigameState.Tag.Clicking
                        && state.Value.delta != (0, 0)
                        ? MinigameState.Committing(state.Value.block, state.Value.delta)
                        : MinigameState.Idle();
            });

        // Commit movement

        state
            .Filter(state => state.tag == MinigameState.Tag.Committing)
            .Get(stateValue =>
            {
                var block =
                    stateValue.block;

                var delta =
                    stateValue.delta;

                if (
                    board.TryPushMovement(
                        new Movement
                        {
                            block =
                                block,
                            target =
                                (block.x + delta.Item1, block.y + delta.Item2),
                            startTime =
                                Time.time
                        }
                    )
                )
                {
                    state.Value =
                        MinigameState.Animating(block, delta);
                }
                else
                {
                    state.Value =
                        MinigameState.Idle();
                }
            });

        // ANIMATION END

        board
            .movement
            .Map(Optional.ToBool)
            .Get(isMoving =>
            {
                if (!isMoving && state.Value.tag == MinigameState.Tag.Animating)
                {
                    state.Value =
                        MinigameState.Idle();
                }
            });

        // Show hovered and selected blocks

        var hover =
            Query.From(this, "hover")
            .Get<Transform>();

        var selection =
            Query.From(this, "selection")
            .Get<Transform>();

        var animation =
            Query.From(this, "animation")
            .Get<Transform>();

        var dragError =
            Query
                .From(this, "drag-error")
                .Get<Transform>();

        state
            .Get(value =>
            {
                var block =
                    value.block;

                var showHover =
                    value.tag == MinigameState.Tag.Hovering;

                hover.gameObject.SetActive(showHover);

                if (showHover)
                {
                    ApplyRectToTransform(block.x, block.y, block.width, block.height, hover);
                }

                var showSelected =
                    value.tag == MinigameState.Tag.Clicking;

                selection.gameObject.SetActive(showSelected);

                if (showSelected)
                {
                    ApplyRectToTransform(
                        block.x + Mathf.Min(0, value.delta.Item1),
                        block.y + Mathf.Min(0, value.delta.Item2),
                        block.width + Mathf.Abs(value.delta.Item1),
                        block.height + Mathf.Abs(value.delta.Item2),
                        selection
                    );
                }

                var showAnimation =
                    value.tag == MinigameState.Tag.Committing
                        || value.tag == MinigameState.Tag.Animating;

                animation.gameObject.SetActive(showAnimation);

                if (showAnimation)
                {
                    ApplyRectToTransform(
                        block.x + Mathf.Min(0, value.delta.Item1),
                        block.y + Mathf.Min(0, value.delta.Item2),
                        block.width + Mathf.Abs(value.delta.Item1),
                        block.height + Mathf.Abs(value.delta.Item2),
                        animation
                    );
                }

                var showError =
                    value.tag == MinigameState.Tag.DragError;

                dragError.gameObject.SetActive(showError);

                if (showError)
                {
                    ApplyRectToTransform(
                        block.x + Mathf.Min(0, value.delta.Item1),
                        block.y + Mathf.Min(0, value.delta.Item2),
                        block.width + Mathf.Abs(value.delta.Item1),
                        block.height + Mathf.Abs(value.delta.Item2),
                        dragError
                    );
                }
            });

        // --- Press ESC or E to leave ---

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
                .Filter(_ => Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
            ,
            escapeButtonClick
        )
            .Get(_ =>
            {
                worldScene.ExitInteractState();
            });

        // Solve the puzzle

        var solveAudio =
            Query
                .From(this, "audio solve")
                .Get<AudioSource>();

        board
            .isInRightOrder
            .Filter(value => value)
            .Get(_ =>
            {
                var resources =
                    Globals.playerResources.Value;

                // Give resources
                if (minigameTag == MinigameTag.PuzzleA)
                {
                    Globals.playerResources.Value =
                        resources.Add(new PlayerResources(0, 5, 1));

                    solveAudio.Play();
                }
                else if (minigameTag == MinigameTag.PuzzleB2)
                {
                    Globals.playerResources.Value =
                        resources.Add(new PlayerResources(0, 6, 3));

                    solveAudio.Play();
                }
                else if (minigameTag == MinigameTag.PuzzleC)
                {
                    Globals.playerResources.Value =
                        resources.Add(new PlayerResources(0, 6, 6));

                    solveAudio.Play();
                }

                worldScene.ExitInteractState();
            });

        // Turn off collider on solve

        var collider =
            Query
                .From(this, "collider")
                .Get();

        board
            .isInRightOrder
            .Initialized
            .Get(value =>
            {
                collider.SetActive(!value);
            });

        // Save progress

        isInteracting
            .Get(_ =>
            {
                Globals.checkpoint =
                    minigameTag == MinigameTag.Garbage ? Checkpoint.Garbage :
                    minigameTag == MinigameTag.PuzzleA ? Checkpoint.PuzzleA :
                    minigameTag == MinigameTag.PuzzleB2 ? Checkpoint.PuzzleB2 :
                    minigameTag == MinigameTag.PuzzleC ? Checkpoint.PuzzleC :
                    Globals.checkpoint;

                Globals.Save();
            });

        // Change circle color

        var circle =
            Query
                .From(this, "circle-mesh")
                .Get<Renderer>();

        lerpedInteracting
            .AndThen(update.Always)
            .Get(t =>
            {
                circle.material
                    .SetColor("_EmissionColor",
                        new Color(
                            0.8f,
                            0.8f,
                            1.0f,
                            1.0f
                        )
                            * Mathf.Lerp(
                                0.17f + Functions.SinusoidalWave(Time.time, 0.17f) * 0.25f,
                                0.8f,
                                t
                            )
                    );
            });
    }

    public static void ApplyRectToTransform(float x, float y, float width, float height, Transform transform)
    {
        transform.localPosition =
                new Vector3(
                    x + (width - 1.0f) / 2.0f,
                    y + (height - 1.0f) / 2.0f,
                    0.0f
                );

        transform.localScale =
            new Vector3(
                width,
                height,
                1.0f
            );
    }
}
