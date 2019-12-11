using UnityEngine;
using TMPro;
using Events;

public class LanguageChangeEvent : GameEvent { }

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMProLocalizer : MonoBehaviour
{
    [SerializeField] Localization _localization;
    string _key;

    void Start()
    {
        if (_localization == null)
            _localization = GetComponentInParent<LocalizationSource>()?.localization;

        if (_localization == null)
            Debug.LogError(
                "TMProLocalizer needs to have a _localization, or an available LocalizationSource" +
                " in a parent."
            );

        _key = GetComponent<TextMeshProUGUI>().text;

        OnLanguageChange(null);
    }

    void OnEnable()
    { EventController.AddListener<LanguageChangeEvent>(OnLanguageChange); }

    void OnDisable()
    { EventController.RemoveListener<LanguageChangeEvent>(OnLanguageChange); }

    void OnLanguageChange(LanguageChangeEvent evt)
    {
        if (_localization == null) return;

        GetComponent<TextMeshProUGUI>().text =
            _localization.GetLocalizedString(_key);
    }
}