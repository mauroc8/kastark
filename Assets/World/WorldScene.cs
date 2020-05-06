using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


public class WorldScene : UpdateAsStream
{
    protected StateStream<InteractState> stateStream = new StateStream<InteractState>(new InteractStates.None());

    public Stream<InteractState> State => stateStream;

    void Start()
    {
        stateStream.Push(stateStream.Value);
    }

    void Awake()
    {
        // --- APPROACH NPC --- 

        var airaTrigger =
            Node.Query(this, "interaction-trigger").GetComponent<Collider>();

        var npc = Node.Query(this, "npc");

        var npcTrigger = npc.GetComponentInChildren<TriggerEvents>();

        npcTrigger.TriggerEnter
            .Filter(trigger => trigger == airaTrigger)
            .Get(_ =>
            {
                stateStream.Value =
                    new InteractStates.InFrontOfNPC();
            });

        // --- APPROACH ENEMY ---

        var enemies =
            new List<GameObject>
            {
                Query.From(this, "othok").Get(),
                Query.From(this, "arthur").Get()
            };

        foreach (var enemy in enemies)
        {
            var enemyTrigger =
                Query
                    .From(enemy)
                    .Get<TriggerEvents>();

            var enemyId =
                Query
                    .From(enemy)
                    .Get<Enemy>()
                    .id;

            enemyTrigger.TriggerEnter
                .Filter(trigger => trigger == airaTrigger)
                .Get(_ =>
                {
                    stateStream.Value =
                        new InteractStates.InFrontOfEnemy(enemyId);
                });

            enemyTrigger.TriggerExit
                .Filter(trigger => trigger == airaTrigger)
                .Do(ExitInteractState);
        }


        // --- APPROACH MATCH 3s ---

        var firstMatch3 = Node.Query(this, "match3-first");
        var secondMatch3 = Node.Query(this, "match3-second");

        var match3s = new List<GameObject> { firstMatch3, secondMatch3 };

        foreach (var match3 in match3s)
        {
            var triggerEvents = match3.GetComponentInChildren<TriggerEvents>();
            var board = match3.GetComponentInChildren<Board>();

            triggerEvents.TriggerEnter
                .Filter(trigger => trigger == airaTrigger)
                .Always(board)
                .Get(value =>
                {
                    if (value.isCleared.Value)
                        return;

                    stateStream.Value =
                        new InteractStates.InFrontOfMatch3
                        {
                            board = value
                        };
                });

            // End Match3 when board is cleared

            board.isCleared
                .Map(_ => Time.time)
                .AndThen(update.Always)
                .Filter(t =>
                    (Time.time - t) / 0.7f >= 1
                )
                .Lazy()
                .Get(_ =>
                {
                    stateStream.Value =
                        new InteractStates.None();
                });

            triggerEvents.TriggerExit
                .Filter(trigger => trigger == airaTrigger)
                .Do(ExitInteractState);
        }

        // --- Exit NPC ---

        npcTrigger.TriggerExit
            .Filter(trigger => trigger == airaTrigger)
            .Do(ExitInteractState);


        // --- INTERACT USING LETTER E ---

        update
            .Filter(_ => Input.GetKeyDown(KeyCode.E))
            .Get(_ =>
            {
                switch (stateStream.Value)
                {
                    case InteractStates.InFrontOfNPC _:
                        stateStream.Value =
                            new InteractStates.TalkingWithNPC();
                        break;
                    case InteractStates.InFrontOfMatch3 x:
                        stateStream.Value =
                            new InteractStates.PlayingMatch3 { board = x.board };
                        break;
                    case InteractStates.InFrontOfEnemy x:
                        stateStream.Value =
                            new InteractStates.TalkingWithEnemy(x.enemyId);
                        break;
                }
            });

        // --- Show Esc-Return button ---

        var escReturnButton =
            Query
                .From(this, "esc-return")
                .Get();

        State
            .Map(state => state.LockPlayerControl
                && !Functions.IsTypeOf<InteractState, InteractStates.EscMenu>(state)
                && !Functions.IsTypeOf<InteractState, InteractStates.Popup>(state)
            )
            .Lazy()
            .Get(escReturnButton.SetActive);

        // --- Open Journal ---

        Stream.Combine(
            Globals.hasQuest,
            State.Map(state => state.LockPlayerControl)
        )
            .AndThen((hasQuest, lockControl) =>
                hasQuest && !lockControl
                    ? update
                    : Stream.None<Void>()
            )
            .Filter(_ => Input.GetKeyDown(KeyCode.Tab))
            .Listen(this, _ =>
            {
                stateStream.Value =
                   new InteractStates.ViewingJournal { };
            });

        Globals.hasQuest.Value =
            Globals.hasQuest.Value;

        // --- Hide Othok if he's dead ---

        var othok =
            Query
                .From(this, "othok")
                .Get();

        othok.SetActive(Globals.progress == GameProgress.Initial);

        // --- Arthur is invisible until Othok is defeated ---

        var arthur =
            Query
                .From(this, "arthur")
                .Get();

        arthur.SetActive(Globals.progress == GameProgress.DefeatedOthok);

        // --- Escape menu ---

        var canPressEscUpdate =
            stateStream
                .Map(state => state.LockPlayerControl)
                .AndThen(value =>
                {
                    if (!value)
                    {
                        var time = Time.time;

                        return update
                            .Filter(_ => Time.time - time >= 0.6f);
                    }

                    return Stream.None<Void>();
                });

        canPressEscUpdate
            .Filter(_ => Input.GetKeyDown(KeyCode.Escape))
            .Get(_ =>
            {
                if (
                    !Functions.IsTypeOf<InteractState, InteractStates.EscMenu>(stateStream.Value)
                )
                {
                    stateStream.Value =
                        new InteractStates.EscMenu();
                }
            });

        stateStream
            .Map(Functions.IsTypeOf<InteractState, InteractStates.EscMenu>)
            .Lazy()
            .AndThen(value =>
                value
                    ? update
                    : Stream.None<Void>()
            );
    }

    public void RequestPopupDialog()
    {
        stateStream.Value =
            new InteractStates.Popup();
    }

    public void ExitInteractState()
    {
        stateStream.Value =
            new InteractStates.None();
    }
}