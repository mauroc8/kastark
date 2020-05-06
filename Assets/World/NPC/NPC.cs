using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DialogScreen
{
    Greeting,
    Quest,
    Shop
};

public class NPC : UpdateAsStream
{
    public StateStream<DialogScreen> dialogScreen =
        new StateStream<DialogScreen>(DialogScreen.Greeting);

    void Awake()
    {
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


        // Show/hide dialog panel

        var dialogPanel =
            Node.Query(this, "dialog-panel");

        var dialogPanelAlpha =
            dialogPanel.GetComponent<AlphaController>();

        isInteracting.Get(talking =>
        {
            dialogPanel.SetActive(talking);

            if (talking && dialogPanelAlpha != null)
                dialogPanelAlpha.FadeIn(0.6f, 2f);
        });

        // Hide Aira

        var aira =
            Node.Query(transform.root, "aira");

        var lerpedInteracting =
            isInteracting
                .Map(value => value ? 1.0f : 0.0f)
                .AndThen(Functions.LerpStreamOverTime(update, 0.8f));

        lerpedInteracting
            .Map(t => t == 0)
            .Lazy()
            .Get(value =>
            {
                aira.SetActive(value);
            });


        // NPC Move camera

        var npcCameraTransform =
            Node.Query(this, "camera-transform");

        var camera =
            Camera.main;

        /*
        interactingUpdate.Get(_ =>
        {
            camera.transform.position =
                npcCameraTransform.transform.position;

            camera.transform.rotation =
                npcCameraTransform.transform.rotation;
        });
        */

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

        var talkButtonTrigger =
            Node
                .Query(this, "talk-button")
                .GetComponent<HoverAndClickEventTrigger>();

        var shopButtonTrigger =
            Node
                .Query(this, "shop-button")
                .GetComponent<HoverAndClickEventTrigger>();


        talkButtonTrigger.click
            .Get(_ =>
            {
                dialogScreen.Push(DialogScreen.Quest);
            });

        shopButtonTrigger.click
            .Get(_ =>
            {
                dialogScreen.Push(DialogScreen.Shop);
            });

        // Press Escape to go back to the previous screen

        var lastDialogScreenChangeTime =
            Time.time;

        var acceptQuestClick =
            Query
                .From(this, "accept-button")
                .Get<HoverAndClickEventTrigger>()
                .click;

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
            acceptQuestClick.Always(new Void()),
            escapeClick
        )
            .Get(_ =>
            {
                // Avoid double press to change screens too fast!
                if (Time.time - lastDialogScreenChangeTime < 0.3f)
                    return;

                lastDialogScreenChangeTime =
                    Time.time;

                switch (dialogScreen.Value)
                {
                    case DialogScreen.Greeting:
                        worldScene.ExitInteractState();
                        return;

                    case DialogScreen.Quest:
                    case DialogScreen.Shop:

                        dialogScreen.Value =
                            DialogScreen.Greeting;

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
                    screen == DialogScreen.Greeting
                        ? "Return"
                        : "Back";
            });


        // Show SCREENS!

        var greetingScreen =
            Node.Query(this, "greeting-screen");

        var questScreen =
            Node.Query(this, "quest-screen");

        var shopScreen =
            Node.Query(this, "shop-screen");

        greetingScreen.SetActive(true);
        questScreen.SetActive(false);
        shopScreen.SetActive(false);

        Func<DialogScreen, GameObject> getScreenNode =
            screen =>
                screen == DialogScreen.Greeting ? greetingScreen :
                screen == DialogScreen.Quest ? questScreen :
                screen == DialogScreen.Shop ? shopScreen :
                greetingScreen;

        dialogScreen
            .WithLastValue(DialogScreen.Greeting)
            .Get((lastScreen, currentScreen) =>
            {
                getScreenNode(lastScreen).SetActive(false);
                getScreenNode(currentScreen).SetActive(true);
            });

        // Checkpoint

        isInteracting
            .Get(_ =>
            {
                Globals.Save(
                    Checkpoint.Dwarf
                );
            });

        // Play navigation click audio

        var navigationClickAudio =
            Query
                .From(this, "navigation-click-audio")
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

        // Show right quest message

        var dialogInitial =
            Query
                .From(this, "dialog-initial")
                .Get();

        var dialogDefeatedArthur =
            Query
                .From(this, "dialog-defeated-arthur")
                .Get();

        dialogInitial.SetActive(Globals.progress != GameProgress.DefeatedArthur);
        dialogDefeatedArthur.SetActive(Globals.progress == GameProgress.DefeatedArthur);

        // Set quest

        dialogScreen
            .Filter(screen => screen == DialogScreen.Quest)
            .Get(_ =>
            {
                Globals.hasQuest.Value =
                    true;
            });
    }
}
