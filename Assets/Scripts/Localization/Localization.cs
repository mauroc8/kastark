using System;
using System.Collections.Generic;
using UnityEngine;
using ListExtensions;

public enum LanguageId
{
    English,
    Spanish
};

[CreateAssetMenu(menuName = "Kastark/Localization")]
public class Localization : ScriptableObject
{
    public static List<string> languageNames =
        new List<string>
        {
            "English",
            "Español"
        };

    public static LanguageId currentLanguage = LanguageId.English;

    public string GetLocalizedString(string key)
    {
        foreach (var entry in entries)
            if (entry.key == key)
                return entry.data[(int)currentLanguage];

        Debug.Log($"Could not find key {key} in localization {name}.");
        return $"Unknown key {key}";
    }

    [SerializeField]
    public List<LocalizationEntry> entries = new List<LocalizationEntry>();

    public void NewEntry() => entries.Add(new LocalizationEntry());

    List<Action> _queue = new List<Action>(1);

    public bool HasQueuedActions => _queue.Count > 0;

    public void ExecuteQueuedActions()
    {
        _queue.ForEach(action => action.Invoke());
        _queue.Clear();
    }

    public void QueueDelete(LocalizationEntry entry)
    {
        _queue.Add(() => entries.Remove(entry));
    }

    public void QueueMoveDown(LocalizationEntry entry)
    {
        _queue.Add(() => entries.MoveElementDown(entry));
    }
}

[Serializable]
public class LocalizationEntry
{
    public LocalizationEntry()
    {
        data = ListExtension.Repeat("", Localization.languageNames.Count);
    }

    public string key;
    public List<string> data;
}
