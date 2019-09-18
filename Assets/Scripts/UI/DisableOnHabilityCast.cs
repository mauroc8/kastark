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
    
    void Start()
    {
        _button = GetComponent<Button>();
        _textMP = GetComponentInChildren<TextMeshProUGUI>();
        _disabledColor = _textMP.color;
    }

    void OnEnable() {
        EventController.AddListener<HabilityCastEvent>(OnStartCasting);

        _button.interactable = true;
        _textMP.color        = _disabledColor;
    }

    void OnDisable() {
        EventController.RemoveListener<HabilityCastEvent>(OnStartCasting);
    }

    void OnStartCasting(HabilityCastEvent evt) {
        _button.interactable = false;
        _textMP.color        = _disabledColor;
    }
}
