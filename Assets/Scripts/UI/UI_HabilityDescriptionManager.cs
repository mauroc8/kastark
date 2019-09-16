using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;
using TMPro;

public class UI_HabilityDescriptionManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _title = null;
    [SerializeField]
    TextMeshProUGUI _description = null;

    void OnEnable() {
        EventController.AddListener<HabilitySelectEvent>(OnSelectHability);
    }
    void OnDisable() {
        EventController.RemoveListener<HabilitySelectEvent>(OnSelectHability);
    }

    void OnSelectHability(HabilitySelectEvent e) {
        var habilityName = e.habilityId.ToString().ToLower();

       _title.text = Localization.GetLocalizedString(habilityName);
       _description.text = Localization.GetLocalizedString(habilityName + "_description");
    }
}
