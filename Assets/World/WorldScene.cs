using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


public class WorldScene : UpdateAsStream
{
    protected StateStream<InteractState> stateStream = new StateStream<InteractState>(new InteractStates.None());

    public Stream<InteractState> State => stateStream;

    void Start()
    {
        stateStream.Value =
            stateStream.Value;
    }
    void Awake()
    {
        // --- Return from Battle ---

        var (battleResult, battleId) =
            Scenes.ConsumeBattleResult();

        // Finish Quest

        if (battleResult == BattleResult.Win && battleId == 4)
        {
            var inventory =
                Globals.inventory.Value;

            for (int i = 0; i < 6; i++)
            {
                if (Items.Empty.IsEmpty(inventory[i]))
                {
                    inventory[i] =
                        new Items.Clover();

                    break;
                }
            }

            Globals.inventory.Value =
                inventory;

            Globals.quest =
                QuestStatus.GetReward;
        }

        // Update checkpoint

        if (battleResult != BattleResult.None)
        {
            Globals.checkpoint =
                battleId == 0 ? Checkpoint.Enemy0 :
                battleId == 1 ? Checkpoint.Enemy1 :
                battleId == 2 ? Checkpoint.Enemy2 :
                battleId == 3 ? Checkpoint.Enemy3 :
                battleId == 4 ? Checkpoint.Arthur :
                Globals.checkpoint;
        }

        // Kill enemy

        if (battleResult == BattleResult.Win)
        {
            Globals.enemiesAreAlive[battleId] =
                false;
        }

        // Earn resources

        var earnedPlastic =
            battleId == 0 ? 5 :
            battleId == 1 ? 5 :
            battleId == 2 ? 6 :
            battleId == 3 ? 8 :
            battleId == 4 ? 12 :
            0;

        Action earnResources = () =>
        {
            Globals.playerResources.Value =
                Globals.playerResources.Value
                    .Add(new PlayerResources(earnedPlastic, 0, 0));
        };

        // Win battle

        if (battleResult == BattleResult.Win)
        {
            stateStream.Value =
                new InteractStates.Popup
                {
                    title =
                        "You win",
                    content =
                        $"You earned <color=#D67A7A>{earnedPlastic} runes</color>.\n"
                            + (
                                battleId == 4
                                    ? $"You earned a <color=#84CA90>four-leaf clover</color>.\n"
                                        + $"Talk to <b>Dhendé</b> to finish the quest.\n"
                                    : ""
                              )
                            ,
                    acceptButton =
                        "Accept",
                    onAccept =
                        earnResources
                };
        }

        // Lose battle

        if (battleResult == BattleResult.Lose)
        {
            stateStream.Value =
                new InteractStates.Popup
                {
                    title =
                        "You lose",
                    content =
                        "You can try again anytime.",
                    acceptButton =
                        "Accept",
                    onAccept =
                        null
                };
        }

        // Play resources audio

        var winBattleAudio =
            Query
                .From(this, "audio win-battle")
                .Get<AudioSource>();

        earnResources +=
            () =>
            {
                winBattleAudio.Play();
            };

        // Save all changes

        Globals.Save();

        earnResources +=
            Globals.Save;

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
            new Enemy[]
            {
                Query.From(this, "enemy-0").Get<Enemy>(),
                Query.From(this, "enemy-1").Get<Enemy>(),
                Query.From(this, "enemy-2").Get<Enemy>(),
                Query.From(this, "enemy-3").Get<Enemy>(),
                Query.From(this, "enemy-boss").Get<Enemy>()
            };

        foreach (var enemy in enemies)
        {
            var enemyTrigger =
                Query
                    .From(enemy)
                    .Get<TriggerEvents>();

            enemyTrigger.TriggerEnter
                .Filter(trigger => trigger == airaTrigger)
                .Get(_ =>
                {
                    stateStream.Value =
                        new InteractStates.InFrontOfEnemy(enemy);
                });

            enemyTrigger.TriggerExit
                .Filter(trigger => trigger == airaTrigger)
                .Do(ExitInteractState);
        }

        // --- Hide dead enemies ---

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].gameObject.SetActive(
                Globals.enemiesAreAlive[i]
            );
        }


        // --- APPROACH MATCH 3s ---

        AwakeMinigame(
            Query
                .From(this, "garbage")
                .Get<Minigame>(),
            airaTrigger
        );

        AwakeMinigame(
            Query
                .From(this, "puzzle-a")
                .Get<Minigame>(),
            airaTrigger
        );

        AwakeMinigame(
            Query
                .From(this, "puzzle-b2")
                .Get<Minigame>(),
            airaTrigger
        );

        AwakeMinigame(
            Query
                .From(this, "puzzle-c")
                .Get<Minigame>(),
            airaTrigger
        );

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
                    case InteractStates.InFrontOfMinigame x:
                        stateStream.Value =
                            new InteractStates.PlayingMinigame { minigameTag = x.minigameTag };
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

        State.Map(state => state.LockPlayerControl)
            .AndThen(lockControl =>
                !lockControl && Globals.quest != QuestStatus.None
                    ? update
                    : Stream.None<Void>()
            )
            .Filter(_ => Input.GetKeyDown(KeyCode.Q))
            .Bind(this)
            .Get(_ =>
            {
                stateStream.Value =
                   new InteractStates.ViewingQuest { };
            });

        // --- Open Inventory ---

        State
            .Map(state => state.LockPlayerControl)
            .AndThen(lockControl =>
                lockControl
                    ? Stream.None<Void>()
                    : update
            )
            .Filter(_ => Input.GetKeyDown(KeyCode.I))
            .Get(_ =>
            {
                stateStream.Value =
                    new InteractStates.ViewingInventory();
            });

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

        // --- Show ui window for dialogs and inventory and quest ---

        var uiWindow =
            Query
                .From(this, "ui window")
                .Get<CanvasGroup>();

        var lerpedShowWindow =
            stateStream
                .Map(state => state.ShowPopupWindow)
                .Map(value => value ? 1.0f : 0.0f)
                .AndThen(Functions.LerpStreamOverTime(update, 0.3f));

        lerpedShowWindow
            .Map(t => t != 0)
            .Lazy()
            .InitializeWith(false)
            .Get(uiWindow.gameObject.SetActive);

        lerpedShowWindow
            .Get(t =>
            {
                uiWindow.alpha =
                    Mathf.Pow(t, 2.0f);
            });
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

    private void AwakeMinigame(Minigame minigame, Collider airaTrigger)
    {
        var triggerEvents = minigame.GetComponentInChildren<TriggerEvents>();
        var board = minigame.GetComponentInChildren<Board>();

        triggerEvents.TriggerEnter
            .Filter(trigger => trigger == airaTrigger)
            .Get(_ =>
            {
                stateStream.Value =
                    new InteractStates.InFrontOfMinigame
                    {
                        minigameTag = minigame.minigameTag
                    };
            });

        triggerEvents.TriggerExit
            .Filter(trigger => trigger == airaTrigger)
            .Do(ExitInteractState);
    }
}