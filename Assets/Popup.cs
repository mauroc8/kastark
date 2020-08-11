using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : UpdateAsStream
{
    void Awake()
    {
        var worldScene =
            GetComponentInParent<WorldScene>();

        var worldState =
            worldScene.State;

        var popupInfo =
            worldState
                .Map(Optional.FromCast<InteractState, InteractStates.Popup>)
                .Lazy();

        var isInteracting =
            popupInfo
                .Map(Optional.ToBool);

        //

        var popup =
            Query
                .From(this, "ui popup")
                .Get();

        isInteracting
            .InitializeWith(false)
            .Get(popup.SetActive);

        var title =
            Query
                .From(popup, "title")
                .Get<TMPro.TextMeshProUGUI>();

        var content =
            Query
                .From(popup, "content")
                .Get<TMPro.TextMeshProUGUI>();

        var acceptButton =
            Query
                .From(popup, "accept-button")
                .Get<TMPro.TextMeshProUGUI>();

        var acceptButtonTrigger =
            Query
                .From(popup, "accept-button")
                .Get<HoverAndClickEventTrigger>();

        popupInfo
            .FilterMap(a => a)
            .Get(info =>
            {
                title.text =
                    info.title;

                content.text =
                    info.content;

                acceptButton.text =
                    info.acceptButton;
            });

        // Accept

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

        popupInfo
            .AndThen(optionalInfo =>
                optionalInfo
                    .CaseOf(
                        info =>
                            Stream.Merge(
                                update
                                    .Filter(_ =>
                                        Input.GetKeyDown(KeyCode.Escape)
                                        || Input.GetKeyDown(KeyCode.Return)
                                    )
                                ,
                                escapeClick,
                                acceptButtonTrigger.click.Always(new Void())
                            )
                                .Always(info)
                        ,
                        Stream.None<InteractStates.Popup>
                    )
            )
            .Get(info =>
            {
                if (info.onAccept != null)
                {
                    info.onAccept();
                }

                worldScene.ExitInteractState();
            });
    }
}
