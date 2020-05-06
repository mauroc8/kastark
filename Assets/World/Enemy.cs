using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyId
{
    FirstEnemy,
    SecondEnemy
};

public class Enemy : UpdateAsStream
{
    public EnemyId id =
        EnemyId.SecondEnemy;

    void Awake()
    {
        var worldScene =
            GetComponentInParent<WorldScene>();

        var worldState =
            worldScene.State;

        var isAboutToInteract =
            worldState
                .Map(interact =>
                    Functions.IsTypeOf<InteractState, InteractStates.InFrontOfEnemy>(interact)
                        && ((InteractStates.InFrontOfEnemy)interact).enemyId == id
                )
                .Lazy();


        var isInteracting =
            worldState
                .Map(interact =>
                    Functions.IsTypeOf<InteractState, InteractStates.TalkingWithEnemy>(interact)
                        && ((InteractStates.TalkingWithEnemy)interact).enemyId == id
                )
                .Lazy();

        var interactingUpdate =
            isInteracting
                .AndThen(value =>
                    value
                        ? update
                        : Stream.None<Void>()
                );

        // Interact text

        var interactText =
            Node.Query(this, "interact-text");

        isAboutToInteract
            .Get(interactText.SetActive);

        // Dialog

        var dialog =
            Query
                .From(this, "dialog")
                .Get();

        isInteracting
            .Get(dialog.SetActive);

        // Click on button

        var buttonClick =
            Query
                .From(this, "fight-button")
                .Get<HoverAndClickEventTrigger>()
                .click;

        buttonClick
            .Get(_ =>
            {
                if (id == EnemyId.FirstEnemy)
                    Globals.MoveToScene(1);
                else
                    Globals.MoveToScene(2);
            });


        // Save Checkpoint

        Stream.Merge(
            isAboutToInteract,
            isInteracting
        )
            .Get(_ =>
            {
                Globals.Save(
                    id == EnemyId.FirstEnemy
                        ? Checkpoint.FirstFight
                        : Checkpoint.SecondFight
                );
            });

        // ESC return text

        var escReturnButton =
            Query
                .From(transform.root, "esc-return")
                .Get<EscReturnButton>();

        isInteracting
            .Get(value =>
            {
                if (value)
                    escReturnButton.label.Value =
                        "Return";
            });

        // ESC button works

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
            interactingUpdate
                .Filter(_ => Input.GetKeyDown(KeyCode.Escape))
            ,
            escapeButtonClick
        )
            .Get(_ =>
            {
                worldScene.ExitInteractState();
            });

    }
}
