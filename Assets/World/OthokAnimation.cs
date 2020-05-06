using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OthokAnimation : UpdateAsStream
{
    void Awake()
    {
        var isPopupOpen =
            new StateStream<bool>(false);

        var mesh =
            Query
                .From(this, "othok")
                .Get();

        var animator =
            Query
                .From(this)
                .Get<Animator>();

        var animatorEvents =
            Query
                .From(this)
                .Get<OthokAnimatorEvents>();

        if (Globals.ShowOthokDialog)
        {
            StartCoroutine(OthokRunAnimation(animator));
        }
        else
        {
            mesh.SetActive(false);
        }


        // Open popup

        var worldScene =
            GetComponentInParent<WorldScene>();

        animatorEvents
            .runEnd
            .Get(_ =>
            {
                worldScene.RequestPopupDialog();
                isPopupOpen.Value = true;
            });

        // Show dialog

        var dialog =
            Query
                .From(this, "dialog")
                .Get();

        dialog.SetActive(false);

        isPopupOpen.Get(dialog.SetActive);


        // Close popup

        var acceptButtonClick =
            Query
                .From(dialog)
                .Get<HoverAndClickEventTrigger>()
                .click;

        acceptButtonClick
            .Get(_ =>
            {
                worldScene.ExitInteractState();
                isPopupOpen.Value = false;

                Globals
                    .Save(Checkpoint.Dwarf);

                mesh.SetActive(false);
            });
    }

    IEnumerator OthokRunAnimation(Animator animator)
    {
        yield return new WaitForSeconds(0.5f);

        animator.SetFloat("horizontal_speed", 10.0f);
    }
}
