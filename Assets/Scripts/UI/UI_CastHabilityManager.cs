using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Events;
using TMPro;

public class UI_CastHabilityManager : MonoBehaviour
{
    [SerializeField]
    GameObject _cancelButtonGameObject = null;

    Button _cancelButton = null;
    TextMeshProUGUI _cancelTMP = null;

    void Start() {
        _cancelButton = _cancelButtonGameObject.GetComponent<Button>();
        _cancelTMP    = _cancelButtonGameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    void OnEnable() {
        EventController.AddListener<ConfirmSelectedHabilityEvent>(OnStartCasting);
    }

    void OnDisable() {
        EventController.RemoveListener<ConfirmSelectedHabilityEvent>(OnStartCasting);
        _cancelButton.interactable = true;
        _cancelTMP.color = Color.white;
    }

    void OnStartCasting(ConfirmSelectedHabilityEvent e) {
        _cancelButton.interactable = false;
        _cancelTMP.color = Color.gray;
    }
}
