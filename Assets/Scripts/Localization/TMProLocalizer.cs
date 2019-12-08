using UnityEngine;
using TMPro;
using Events;

public class LanguageChangeEvent : GameEvent { }

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMProLocalizer : MonoBehaviour
{
    string _key;
    Localization _localization;

    void Start()
    {
        _localization = GetComponentInParent<LocalizationSource>()?.localization;

        Debug.Assert(_localization != null);

        _key = GetComponent<TextMeshProUGUI>().text;

        OnLanguageChange(null);
    }

    void OnEnable()
    { EventController.AddListener<LanguageChangeEvent>(OnLanguageChange); }

    void OnDisable()
    { EventController.RemoveListener<LanguageChangeEvent>(OnLanguageChange); }

    void OnLanguageChange(LanguageChangeEvent evt)
    {
        GetComponent<TextMeshProUGUI>().text =
            _localization.GetLocalizedString(_key);
    }
}