using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class UI_CastHabilityManager : MonoBehaviour
{
    [SerializeField]
    GameObject _cancelButton = null;

    void OnEnable() {
        EventController.AddListener<ConfirmSelectedHabilityEvent>(OnStartCasting);
    }

    void OnDisable() {
        EventController.RemoveListener<ConfirmSelectedHabilityEvent>(OnStartCasting);
        _cancelButton.SetActive(true);
    }

    void OnStartCasting(ConfirmSelectedHabilityEvent e) {
        _cancelButton.SetActive(false);
    }
}
