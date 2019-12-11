using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Localization))]
public class LocalizationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var translation = (Localization)target;
        var count = translation.entries.Count;
        GUILayout.Space(10);
        if (GUILayout.Button($"Edit in external window ({count} {(count == 1 ? "entry" : "entries")})"))
        {
            var window = (LocalizationWindow)EditorWindow.GetWindow(
                typeof(LocalizationWindow),
                true,
                $"{translation.name} - Localization editor"
            );
            window.localization = translation;
        }
    }
}

public class LocalizationWindow : EditorWindow
{
    public Localization localization;

    void OnGUI()
    {
        if (localization == null) return;

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Key", GUILayout.Width(110));

            foreach (var language in Localization.languageNames)
                GUILayout.Label(language);

            // Reserve space for "Delete" and "Down" buttons
            GUILayout.Space(110);
        }
        GUILayout.EndHorizontal();

        foreach (var entry in localization.entries)
        {
            GUILayout.BeginHorizontal();

            entry.key = EditorGUILayout.TextField(entry.key, GUILayout.Width(110));

            for (var i = 0; i < entry.data.Count; i++)
            {
                entry.data[i] = EditorGUILayout.TextField(entry.data[i]);
            }

            if (GUILayout.Button("Delete", GUILayout.MaxWidth(50)))
            {
                localization.QueueDelete(entry);
            }

            if (GUILayout.Button("Down", GUILayout.MaxWidth(50)))
            {
                localization.QueueMoveDown(entry);
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("New entry", GUILayout.MaxWidth(110)))
            {
                localization.NewEntry();
            }

            if (GUILayout.Button("Save", GUILayout.MaxWidth(110)))
            {
                EditorUtility.SetDirty(localization);
            }
        }
        GUILayout.EndHorizontal();

        if (localization.HasQueuedActions)
        {
            // If we edit an input while focused, its content doesn't get updated.
            // We need to move focus out.
            EditorGUI.FocusTextInControl("parent");
            localization.ExecuteQueuedActions();
        }

        GUILayout.Space(10);
    }
}
