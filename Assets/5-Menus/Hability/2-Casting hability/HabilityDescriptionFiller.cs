using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;
using TMPro;

public class HabilityDescriptionFiller : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _title = null;
    [SerializeField] TextMeshProUGUI _description = null;
    [SerializeField] TextMeshProUGUI _tooltip = null;

    public void FillWithHability(Hability hability) {
       _title.text = hability.LocalizedName;
       _description.text = RaycastHelper.GetParsedHabilityDescription(hability);
       _tooltip.text = hability.LocalizedTooltip;
    }
}
