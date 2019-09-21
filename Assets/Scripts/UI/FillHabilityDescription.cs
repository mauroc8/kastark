using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;
using TMPro;

public class FillHabilityDescription : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _title = null;
    [SerializeField] TextMeshProUGUI _description = null;
    [SerializeField] TextMeshProUGUI _tooltip = null;

    [System.NonSerialized] public Hability selectedHability;

    void OnEnable() {
       _title.text = selectedHability.LocalizedName;
       _description.text = selectedHability.LocalizedDescription;
       _tooltip.text = selectedHability.LocalizedTooltip;
    }
}
