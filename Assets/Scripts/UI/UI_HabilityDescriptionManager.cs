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
        EventController.AddListener<SelectedHabilityEvent>(OnSelectedHability);
    }
    void OnDisable() {
        EventController.RemoveListener<SelectedHabilityEvent>(OnSelectedHability);
    }

    void OnSelectedHability(SelectedHabilityEvent e) {
        var habilityId = e.habilityId;
        var habilityName = Util.HabilityNameFromId(habilityId);

       _title.text = Localizer.GetLocalizedString(habilityName);
       _description.text = Localizer.GetLocalizedString(habilityName + "_description");
    }
}
