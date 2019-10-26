using UnityEngine;
using TMPro;
using StringLocalization;
using Events;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMProLocalizer : MonoBehaviour
{
    string _key;

    void Awake()
    {
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
            Localization.GetLocalizedString(_key);
    }
}