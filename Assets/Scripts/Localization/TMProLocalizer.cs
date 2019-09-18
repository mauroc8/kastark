using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Events;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMProLocalizer : MonoBehaviour
{
    [SerializeField] string _key = "";

    void Awake()
    {
        var tmp = GetComponent<TextMeshProUGUI>();
        
        if (_key == "")
            _key = tmp.text;
        
        tmp.text = Localization.GetLocalizedString(_key);
    }

    void OnEnable() {
        EventController.AddListener<LanguageChangeEvent>(OnLanguageChange);
    }
    void OnDisable() {
        EventController.RemoveListener<LanguageChangeEvent>(OnLanguageChange);
    }

    void OnLanguageChange(LanguageChangeEvent e) {
        GetComponent<TextMeshProUGUI>().text = Localization.GetLocalizedString(_key);
    }
}
