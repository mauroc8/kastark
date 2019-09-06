using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class UI_CastHabilityManager : MonoBehaviour
{
    [SerializeField]
    GameObject _cancelButton = null;

    void OnEnable() {
        EventController.AddListener<StartCastingEvent>(OnStartCasting);
    }

    void OnDisable() {
        EventController.RemoveListener<StartCastingEvent>(OnStartCasting);
        _cancelButton.SetActive(true);
    }

    void OnStartCasting(StartCastingEvent e) {
        _cancelButton.SetActive(false);
    }
}
