using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Events;
using TMPro;

public class DisableOnHabilityCast : MonoBehaviour
{
    [SerializeField] Color _disabledColor = Color.gray;

    Button _button;
    TextMeshProUGUI _textMP;
    Color _enabledColor;
    
    void Awake()
    {
        _button = GetComponent<Button>();
        _textMP = GetComponentInChildren<TextMeshProUGUI>();
        _enabledColor = _textMP.color;
    }

    void OnEnable() {
        EventController.AddListener<HabilityCastEvent>(OnHabilityCast);

        _button.interactable = true;
        _textMP.color        = _enabledColor;
    }

    void OnDisable() {
        EventController.RemoveListener<HabilityCastEvent>(OnHabilityCast);
    }

    void OnHabilityCast(HabilityCastEvent evt) {
        _button.interactable = false;
        _textMP.color        = _disabledColor;
    }
}
