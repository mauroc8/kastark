using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;
using TMPro;

public class FillHabilityDescription : MonoBehaviour
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

    void OnSelectHability(HabilitySelectEvent evt) {
       _title.text = evt.hability.LocalizedName;
       _description.text = evt.hability.LocalizedDescription;
    }
}
