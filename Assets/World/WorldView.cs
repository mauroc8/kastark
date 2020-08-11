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

        // --- Esc Menu ---

        var escMenu =
            Query
                .From(this, "ui esc-menu")
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
                Globals.Save();
                Scenes.LoadMainMenu();
            });

        // --- Interact text ---

        var interact =
            Query
                .From(this, "ui e-interact")
                .Get();

        worldScene
            .State
            .Map(state => state.ShowInteractPrompt)
            .Get(interact.SetActive);
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
