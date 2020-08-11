using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestScreen
{
    Arthur,
    Reward
};

public class Quest : UpdateAsStream
{
    public StateStream<QuestScreen> screen =
        new StateStream<QuestScreen>(QuestScreen.Arthur);

    void Awake()
    {
        var worldScene =
            GetComponent<WorldScene>();

        var worldState =
            worldScene.State;

        var isInteracting =
            worldState
                .Map(Functions.IsTypeOf<InteractState, InteractStates.ViewingQuest>)
                .Lazy();

        var interactUpdate =
            isInteracting
                .AndThen(value =>
                {
                    var time =
                        Time.time;

                    return
                        value
                            ? update.Filter(_ => Time.time - time >= 0.04f)
                            : Stream.None<Void>();
                });


        // --- View Quest ---

        var questArthur =
            Query
                .From(this, "quest-arthur")
                .Get();

        var questReward =
            Query
                .From(this, "quest-reward")
                .Get();

        worldState
            .Map(Functions.IsTypeOf<InteractState, InteractStates.ViewingQuest>)
            .Lazy()
            .InitializeWith(false)
            .Get(viewingQuest =>
            {
                questArthur.SetActive(
                    viewingQuest && Globals.quest == QuestStatus.FightArthur
                );

                questReward.SetActive(
                    viewingQuest && Globals.quest == QuestStatus.GetReward
                );
            });

        // Pres Esc to close quest

        var escReturnClickUnfiltered =
            Query
                .From(this, "esc-return")
                .Get<HoverAndClickEventTrigger>()
                .click
                .Always(new Void());

        var escReturnClick =
            isInteracting
                .AndThen(value =>
                    value
                        ? escReturnClickUnfiltered
                        : Stream.None<Void>()
                );

        Stream.Merge(
            interactUpdate
                .Filter(_ => Input.GetKeyDown(KeyCode.Escape))
            ,
            escReturnClick
        )
            .Get(_ =>
            {
                worldScene.ExitInteractState();
            });


        // Show screens

        /*
        var quest =
            Query
                .From(this, "quest quest")
                .Get();

        var inventoryButtonImage =
            Query
                .From(this, "quest inventory-button")
                .Get<UnityEngine.UI.Image>();

        var questButtonImage =
            Query
                .From(this, "quest quest-button")
                .Get<UnityEngine.UI.Image>();

        screen
            .Get(value =>
            {
                inventory.SetActive(value == QuestScreen.Inventory);
                quest.SetActive(value == QuestScreen.Quest);

                inventoryButtonImage.color =
                    value == QuestScreen.Inventory
                        ? Color.white
                        : Color.gray;

                questButtonImage.color =
                    value == QuestScreen.Quest
                        ? Color.white
                        : Color.gray;
            });

        */
        /*
        var inventoryButtonClick =
            Query
                .From(this, "quest inventory-button")
                .Get<HoverAndClickEventTrigger>()
                .click;

        var questButtonClick =
            Query
                .From(this, "quest quest-button")
                .Get<HoverAndClickEventTrigger>()
                .click;

        inventoryButtonClick
            .Get(_ =>
            {
                screen.Value =
                    QuestScreen.Inventory;
            });

        questButtonClick
            .Get(_ =>
            {
                screen.Value =
                    QuestScreen.Quest;
            });
        */
    }
}
