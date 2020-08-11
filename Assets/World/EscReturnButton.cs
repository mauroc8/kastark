using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscReturnButton : MonoBehaviour
{
    public StateStream<string> label =
        new StateStream<string>("Return");

    void Awake()
    {
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
