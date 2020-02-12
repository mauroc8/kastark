using UnityEngine;
using TMPro;
using GlobalEvents;
using System;

public class LanguageChangeEvent : GlobalEvent { }

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMProLocalizer : StreamBehaviour
{
    protected override void Awake()
    {
        var localizationSource = GetContext<LocalizationSource>();

        var localization = localizationSource.localization;

        var key = GetComponent<TextMeshProUGUI>().text;

        Action changeLanguage = () =>
        {
            if (localization == null) return;

            GetComponent<TextMeshProUGUI>().text =
                localization.GetLocalizedString(key);
        };

        startStream.Do(changeLanguage);
        GlobalEventStream<LanguageChangeEvent>().Do(changeLanguage);
    }
}