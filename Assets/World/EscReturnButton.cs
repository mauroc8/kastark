using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HoverAndClickEventTrigger))]
[RequireComponent(typeof(AlphaController))]
public class EscReturnButton : MonoBehaviour
{
    public StateStream<string> label =
        new StateStream<string>("Return");

    void Awake()
    {
        // Hover effect

        var eventTrigger =
            GetComponent<HoverAndClickEventTrigger>();

        var alphaController =
            GetComponent<AlphaController>();

        alphaController.Alpha =
            eventTrigger.isHovering.Value ? 1.0f : 0.8f;

        eventTrigger.isHovering
            .Get(value =>
            {
                alphaController.Alpha =
                    value ? 1.0f : 0.8f;
            });

        // Change text
        var labelText =
            Query
                .From(this, "label")
                .Get<TMPro.TextMeshProUGUI>();

        var shadowText =
            Query
                .From(this, "shadow")
                .Get<TMPro.TextMeshProUGUI>();

        this.label.Value =
            labelText.text;

        var localization =
            GetComponentInParent<LocalizationSource>()
                .localization;

        this.label
            .Get(value =>
            {
                value =
                    localization.GetLocalizedString(value);

                labelText.text = value;
                shadowText.text = value;
            });
    }
}
