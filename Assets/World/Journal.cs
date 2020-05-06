using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JournalScreen
{
    Inventory,
    Quest
};

public class Journal : UpdateAsStream
{
    public StateStream<JournalScreen> screen =
        new StateStream<JournalScreen>(JournalScreen.Inventory);

    void Awake()
    {
        var worldScene =
            GetComponent<WorldScene>();

        var worldState =
            worldScene.State;

        var isInteracting =
            worldState
                .Map(Functions.IsTypeOf<InteractState, InteractStates.ViewingJournal>)
                .Lazy();

        var interactUpdate =
            isInteracting
                .AndThen(value =>
                    value
                        ? update
                        : Stream.None<Void>()
                );

        interactUpdate
            .Filter(_ => Input.GetKeyDown(KeyCode.Tab))
            .Get(_ =>
            {
                screen.Value =
                    screen.Value == JournalScreen.Inventory
                        ? JournalScreen.Quest
                        : JournalScreen.Inventory;
            });

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

        isInteracting
            .Filter(a => a)
            .Get(_ =>
            {
                screen.Value =
                    JournalScreen.Quest;
            });


        // Show screens

        var inventory =
            Query
                .From(this, "journal inventory")
                .Get();

        var quest =
            Query
                .From(this, "journal quest")
                .Get();

        var inventoryButtonImage =
            Query
                .From(this, "journal inventory-button")
                .Get<UnityEngine.UI.Image>();

        var questButtonImage =
            Query
                .From(this, "journal quest-button")
                .Get<UnityEngine.UI.Image>();

        screen
            .Get(value =>
            {
                inventory.SetActive(value == JournalScreen.Inventory);
                quest.SetActive(value == JournalScreen.Quest);

                inventoryButtonImage.color =
                    value == JournalScreen.Inventory
                        ? Color.white
                        : Color.gray;

                questButtonImage.color =
                    value == JournalScreen.Quest
                        ? Color.white
                        : Color.gray;
            });

        var inventoryButtonClick =
            Query
                .From(this, "journal inventory-button")
                .Get<HoverAndClickEventTrigger>()
                .click;

        var questButtonClick =
            Query
                .From(this, "journal quest-button")
                .Get<HoverAndClickEventTrigger>()
                .click;

        inventoryButtonClick
            .Get(_ =>
            {
                screen.Value =
                    JournalScreen.Inventory;
            });

        questButtonClick
            .Get(_ =>
            {
                screen.Value =
                    JournalScreen.Quest;
            });
    }
}
