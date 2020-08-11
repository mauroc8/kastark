using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DialogScreen
{
    Greeting,
    Quest,
    Accepted,
    Shop,
    Reward
};

public class NPC : UpdateAsStream
{
    public StateStream<DialogScreen> dialogScreen =
        new StateStream<DialogScreen>(DialogScreen.Greeting);

    void Awake()
    {
        // Initial screen

        dialogScreen.Value =
            Globals.quest == QuestStatus.FightArthur
                ? DialogScreen.Accepted
                : Globals.quest == QuestStatus.GetReward
                ? DialogScreen.Reward
                : DialogScreen.Greeting
            ;

        var worldScene =
            GetComponentInParent<WorldScene>();

        var worldState =
            worldScene.State;

        var isInteracting =
            worldState
                .Map(state =>
                    Functions.IsTypeOf<InteractState, InteractStates.TalkingWithNPC>(state)
                )
                .Lazy();

        var interactingUpdate =
            isInteracting
                .AndThen(value =>
                    value
                        ? update
                        : Stream.None<Void>()
                );

        var canvas =
            Query
                .From(worldScene, "ui dhende")
                .Get();

        isInteracting
            .InitializeWith(false)
            .Get(canvas.SetActive);

        var lerpedInteracting =
            isInteracting
                .Map(value => value ? 1.0f : 0.0f)
                .AndThen(Functions.LerpStreamOverTime(update, 0.8f));

        // NPC Move camera

        var npcCameraTransform =
            Node.Query(this, "camera-transform");

        var camera =
            Camera.main;

        Stream.Combine(
            isInteracting
                .Filter(t => t)
                .Map(_ => (camera.transform.position, camera.transform.rotation))
            ,
            lerpedInteracting
                .Filter(t => t > 0)
        )
            .Get((cam, t) =>
            {
                t = Functions.EaseInOut(t);

                camera.transform.position =
                    Vector3.Lerp(
                        cam.Item1,
                        npcCameraTransform.transform.position,
                        t
                    );

                camera.transform.rotation =
                    Quaternion.Lerp(
                        cam.Item2,
                        npcCameraTransform.transform.rotation,
                        t
                    );
            });

        // Change Dialog Screen

        var questButton =
            Node
                .Query(canvas, "greeting-screen quest-button")
                .GetComponent<HoverAndClickEventTrigger>();

        var acceptQuestButton =
            Node
                .Query(canvas, "quest-screen accept-button")
                .GetComponent<HoverAndClickEventTrigger>();

        var shopButton =
            Node
                .Query(canvas, "accepted-screen shop-button")
                .GetComponent<HoverAndClickEventTrigger>();

        var shopButton2 =
            Query
                .From(canvas, "reward-screen shop-button")
                .Get<HoverAndClickEventTrigger>();

        questButton.click
            .Get(_ =>
            {
                dialogScreen.Value =
                    DialogScreen.Quest;
            });

        acceptQuestButton.click
            .Get(_ =>
            {
                dialogScreen.Value =
                    DialogScreen.Accepted;

                Globals.quest =
                    QuestStatus.FightArthur;
            });

        Stream
            .Merge(shopButton.click, shopButton2.click)
            .Get(_ =>
            {
                dialogScreen.Value =
                    DialogScreen.Shop;
            });

        // Press Escape to go back to the previous screen

        var lastDialogScreenChangeTime =
            Time.time;

        var escapeClickUnfiltered =
            Query
                .From(transform.root, "esc-return")
                .Get<HoverAndClickEventTrigger>()
                .click
                .Always(new Void());

        var escapeClick =
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
            escapeClick
        )
            .Get(_ =>
            {
                // Avoid double press to change screens too fast!
                if (Time.time - lastDialogScreenChangeTime < 0.1f)
                    return;

                lastDialogScreenChangeTime =
                    Time.time;

                switch (dialogScreen.Value)
                {
                    case DialogScreen.Greeting:
                    case DialogScreen.Accepted:
                    case DialogScreen.Quest:
                    case DialogScreen.Reward:

                        worldScene.ExitInteractState();

                        return;

                    case DialogScreen.Shop:

                        dialogScreen.Value =
                            Globals.quest == QuestStatus.FightArthur
                                ? DialogScreen.Accepted
                                : DialogScreen.Reward;

                        return;
                }
            });

        // Esc return text

        var returnButton =
            Query
                .From(transform.root, "esc-return")
                .Get<EscReturnButton>();

        Stream
            .Combine(isInteracting, dialogScreen)
            .Get((interacting, screen) =>
            {
                if (!interacting)
                    return;

                returnButton.label.Value =
                    screen == DialogScreen.Shop
                        ? "Back"
                        : "Return"
                    ;
            });


        // Show SCREENS!

        var greetingScreen =
            Node.Query(canvas, "greeting-screen");

        var questScreen =
            Node.Query(canvas, "quest-screen");

        var acceptedScreen =
            Node.Query(canvas, "accepted-screen");

        var shopScreen =
            Node.Query(canvas, "shop-screen");

        var rewardScreen =
            Query
                .From(canvas, "reward-screen")
                .Get();


        dialogScreen
            .Initialized
            .Get(screen =>
            {
                greetingScreen.SetActive(screen == DialogScreen.Greeting);
                questScreen.SetActive(screen == DialogScreen.Quest);
                acceptedScreen.SetActive(screen == DialogScreen.Accepted);
                shopScreen.SetActive(screen == DialogScreen.Shop);
                rewardScreen.SetActive(screen == DialogScreen.Reward);
            });

        // Checkpoint

        isInteracting
            .Get(_ =>
            {
                Globals.checkpoint =
                    Checkpoint.Dhende;

                Globals.Save();
            });

        // Play navigation click audio

        var navigationClickAudio =
            Query
                .From(canvas, "audio navigation-click")
                .Get<AudioSource>();

        dialogScreen
            .Lazy()
            .Get(_ =>
            {
                navigationClickAudio.pitch =
                    0.9f + UnityEngine.Random.Range(0.0f, 0.2f);

                navigationClickAudio.volume =
                    0.6f + UnityEngine.Random.Range(-0.08f, 0.08f);

                navigationClickAudio.Play();
            });


        // Play animations

        var animator =
            GetComponentInChildren<Animator>();

        isInteracting
            .Get(value =>
            {
                animator.SetBool("is_talking", value);
            });
    }
}
