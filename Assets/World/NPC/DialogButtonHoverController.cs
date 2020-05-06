using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HoverAndClickEventTrigger))]
public class DialogButtonHoverController : UpdateAsStream
{
    public Sprite backgroundImage = null;
    public Sprite hoverBackgroundImage = null;

    void Awake()
    {
        var eventTrigger =
            GetComponent<HoverAndClickEventTrigger>();

        var isHovering =
            eventTrigger.isHovering;

        var image =
            Query
                .From(this, "background")
                .Get<Image>();

        isHovering
            .Get(value =>
            {
                image.sprite =
                    value
                        ? hoverBackgroundImage
                        : backgroundImage;
            });
    }
}
