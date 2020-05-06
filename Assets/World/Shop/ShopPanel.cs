using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPanel : MonoBehaviour
{
    public StateStream<bool> disableButton =
        new StateStream<bool>(false);

    void Awake()
    {
        var eventTrigger =
            GetComponentInChildren<HoverAndClickEventTrigger>();

        var fondoTranslucido =
            Node.Query(this, "fondo-translucido");

        var fondoHover =
            Node.Query(this, "fondo-hover");

        var fondoNormal =
            Node.Query(this, "fondo-normal");

        Action<bool> cambiarFondo = hovering =>
        {
            fondoTranslucido.SetActive(disableButton.Value);

            fondoNormal.SetActive(!disableButton.Value && !hovering);
            fondoHover.SetActive(!disableButton.Value && hovering);
        };

        cambiarFondo(eventTrigger.isHovering.Value);

        eventTrigger
            .isHovering
            .Get(cambiarFondo);

        disableButton
            .Get(_ => cambiarFondo(false));

        disableButton
            .Get(disabled =>
            {
                eventTrigger.enabled = !disabled;
            });
    }
}
