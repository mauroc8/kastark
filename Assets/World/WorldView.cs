using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3Extensions;

[RequireComponent(typeof(WorldScene))]
public class WorldView : StreamBehaviour
{
    protected override void Awake()
    {
        var worldScene =
            GetComponent<WorldScene>();

        var worldState =
            worldScene.State;

        var playerCanMove =
            worldState
                .Map(state => !state.LockPlayerControl);

        var playerCanMoveUpdate =
            playerCanMove
                .AndThen(canMove => canMove ? update : Stream.None<Void>());


        // Show "(E) Talk" text when Aira is in front of NPC.

        var npc = Node.Query(this, "npc");

        var npcInteractText = Node.Query(npc.transform, "interact-text");

        worldState
            .Map(Functions.IsTypeOf<InteractState, InteractStates.InFrontOfNPC>)
            .Lazy()
            .Get(inFrontOfNPC =>
            {
                npcInteractText.SetActive(inFrontOfNPC);
            });

        // --- Talk with NPC ---

        var isTalkingToNpc =
            worldState
                .Map(Functions.IsTypeOf<InteractState, InteractStates.TalkingWithNPC>)
                .Lazy();

        var talkingToNpcUpdate =
            isTalkingToNpc
                .AndThen(talking =>
                    talking ? update : Stream.None<Void>()
                );

        // --- View Journal ---

        var journal =
            Query
                .From(this, "journal")
                .Get();

        journal.SetActive(false);

        worldState
            .Map(Functions.IsTypeOf<InteractState, InteractStates.ViewingJournal>)
            .Lazy()
            .Get(isViewingJournal =>
            {
                journal.SetActive(isViewingJournal);
            });

        // --- Press TAB to open journal ---

        var pressTab =
            Query
                .From(this, "press-tab-tutorial")
                .Get<CanvasGroup>();

        pressTab.alpha =
            0.0f;

        var hasShownTabTutorial = false;

        worldState
            .WithLastValue(new InteractStates.None())
            .Get((lastState, currentState) =>
            {
                if (Functions.IsTypeOf<InteractState, InteractStates.TalkingWithNPC>(lastState))
                {
                    if (!hasShownTabTutorial)
                    {
                        StartCoroutine(ShowTabTutorial(pressTab));
                        hasShownTabTutorial = true;
                    }
                }
            });

        worldState
            .Map(state => state.LockPlayerControl)
            .Lazy()
            .Get(controlLocked =>
            {
                pressTab.gameObject.SetActive(!controlLocked);
            });


        // Esc Menu

        var escMenu =
            Query
                .From(this, "ui-scaled esc-menu")
                .Get();

        escMenu.SetActive(false);

        var escMenuOpacity =
            worldState
                .Map(Functions.IsTypeOf<InteractState, InteractStates.EscMenu>)
                .Lazy()
                .Map(value => value ? 1.0f : 0.0f)
                .AndThen(Functions.LerpStreamOverTime(update, 0.3f));

        escMenuOpacity
            .Map(opacity => opacity != 0.0f)
            .Lazy()
            .Get(escMenu.SetActive);

        var escMenuAlpha =
            Query
                .From(escMenu)
                .Get<CanvasGroup>();

        escMenuOpacity
            .Get(opacity =>
            {
                opacity = Functions.EaseInOut(opacity);

                escMenuAlpha.alpha = opacity;
            });

        var escMenuResume =
            Query
                .From(escMenu, "resume")
                .Get<HoverAndClickEventTrigger>();

        escMenuResume
            .click
            .Get(_ =>
            {
                worldScene.ExitInteractState();
            });

        var escMenuQuit =
            Query
                .From(escMenu, "quit")
                .Get<HoverAndClickEventTrigger>();

        escMenuQuit
            .click
            .Get(_ =>
            {
                Globals.Save(
                    Globals.checkpoint
                );

                Globals.SetScene(1);
            });
    }

    IEnumerator ShowTabTutorial(CanvasGroup pressTab)
    {
        var time = Time.time;
        var duration = 8.6f;

        float t;

        while ((t = (Time.time - time) / duration) < 1.0f)
        {
            pressTab.alpha =
                Mathf.Lerp(
                    0.0f, 1.0f,
                    Mathf.Sin(t * Angle.Turn / 2)
                );

            yield return null;
        }

        pressTab.alpha =
            0.0f;
    }
}
