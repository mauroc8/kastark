using UnityEngine;


public class WorldScene : StreamBehaviour
{
    protected StreamSource<WorldState> stateStream = new StreamSource<WorldState>();
    public Stream<WorldState> State => stateStream;

    private WorldState _worldState =
        new WorldState();

    protected override void Awake()
    {
        start.Do(() =>
        {
            stateStream.Push(_worldState);
        });

        var playerCanMoveUpdate =
            this.State
                .AndThen(state =>
                    state.interactState.LockPlayerControl
                        ? Stream.None<Void>()
                        : update
                );


        // Mouse movement controls camera.

        var horizontalScale = 0.03f;
        var verticalScale = 0.02f;

        playerCanMoveUpdate
            .Map(_ =>
            {
                return new Vector2(
                    Input.GetAxis("Mouse X"),
                    Input.GetAxis("Mouse Y"));
            })
            .Filter(vec2 => vec2 != Vector2.zero)
            .Get(mouseMovement =>
            {
                var pos = _worldState.cameraPosition;

                _worldState.cameraPosition =
                    new PolarVector3
                    (
                        // WARNING: The radius field is redundant RIGHT NOW.
                        pos.elevation,
                        pos.rotation - mouseMovement.x * horizontalScale,
                        Mathf.Clamp
                        (
                            pos.elevation - mouseMovement.y * verticalScale,
                            0,
                            1
                        )
                    );

                stateStream.Push(_worldState);
            });

        // WASD moves character.

        float airaMovementSpeed = 60.7f;

        playerCanMoveUpdate
            .Map(_ =>
            {
                var input =
                    new Vector2(
                        Input.GetAxis("Horizontal"),
                        Input.GetAxis("Vertical")
                    );

                var rotation =
                    // Axises inverted on purpose.
                    // This makes moving forward rotation=0.
                    Mathf.Atan2(input.x, input.y);

                var speed =
                    airaMovementSpeed *
                        Mathf.Min(
                            1.0f,
                            Mathf.Sqrt(input.x * input.x + input.y * input.y)
                        );

                return new PolarVector2(speed, rotation);
            })
            .Get(adjustedMovement =>
            {
                _worldState.playerMovement =
                    adjustedMovement
                        .WithRotation(_worldState.cameraPosition.rotation - adjustedMovement.rotation);
                stateStream.Push(_worldState);
            });

        // --- APPROACH NPC ---

        var airaTrigger =
            Node.Query(this, "interaction-trigger").GetComponent<Collider>();

        var npc = Node.Query(this, "npc");

        var npcTrigger = npc.GetComponentInChildren<TriggerEvents>();

        npcTrigger.TriggerEnter
            .Filter(trigger => trigger == airaTrigger)
            .Get(_ =>
            {
                _worldState.interactState =
                    new InteractStates.InFrontOfNPC();

                stateStream.Push(_worldState);
            });

        // --- APPROACH MATCH 3 ---

        var match3 = Node.Query(this, "match3");
        var match3Trigger = match3.GetComponentInChildren<TriggerEvents>();

        match3Trigger.TriggerEnter
            .Filter(trigger => trigger == airaTrigger)
            .Get(_ =>
            {
                _worldState.interactState =
                    new InteractStates.InFrontOfMatch3();

                stateStream.Push(_worldState);
            });

        // --- Exit NPC/Match3 ---

        Stream.Merge(
            npcTrigger.TriggerExit,
            match3Trigger.TriggerExit
        )
            .Filter(trigger => trigger == airaTrigger)
            .Get(_ =>
            {
                _worldState.interactState =
                    new InteractStates.None();

                stateStream.Push(_worldState);
            });

        // --- INTERACT USING LETTER E ---

        update
            .Filter(_ => Input.GetKeyDown(KeyCode.E))
            .Get(_ =>
            {
                switch (_worldState.interactState)
                {
                    case InteractStates.InFrontOfNPC _:
                        _worldState.interactState =
                            new InteractStates.TalkingWithNPC();
                        break;
                    case InteractStates.InFrontOfMatch3 _:
                        _worldState.interactState =
                            new InteractStates.PlayingMatch3();
                        break;
                }

                stateStream.Push(_worldState);
            });

        // --- Exit interaction using Esc ---

        update
            .Filter(_ => Input.GetKeyDown(KeyCode.Escape))
            .Get(_ =>
            {
                switch (_worldState.interactState)
                {
                    case InteractStates.TalkingWithNPC _:
                        _worldState.interactState =
                            new InteractStates.None();
                        return;
                    case InteractStates.PlayingMatch3 _:
                        _worldState.interactState =
                            new InteractStates.None();
                        return;
                }

                stateStream.Push(_worldState);
            });
    }
}