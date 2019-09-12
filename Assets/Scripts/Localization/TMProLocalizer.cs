using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Events;

public class TMProLocalizer : MonoBehaviour
{
    string _key = null;

    void Start()
    {
        var tmp = GetComponent<TextMeshProUGUI>();
        
        if (_key == null)
            _key = tmp.text;
        
        tmp.text = Localization.GetLocalizedString(_key);
    }

    void OnEnable() {
        EventController.AddListener<ChangeLanguageEvent>(OnLanguageChange);
    }
    void OnDisable() {
        EventController.RemoveListener<ChangeLanguageEvent>(OnLanguageChange);
    }

    void OnLanguageChange(ChangeLanguageEvent e) {
        GetComponent<TextMeshProUGUI>().text = Localization.GetLocalizedString(_key);
    }
}
