using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SessionSampleController : MonoBehaviour
{
    SessionSample _sessionSample = new SessionSample();

    private static SessionSampleController _instance;
    public static SessionSampleController Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<SessionSampleController>() as SessionSampleController;
                if (_instance == null) {
                    _instance = new GameObject().AddComponent<SessionSampleController>();
                    _instance.name = "SessionSampleController";
                    DontDestroyOnLoad(_instance);
                }
            }
            return _instance;
        }
    }

    public static SessionSample SessionSample {
        get { return Instance._sessionSample; }
    }

    public static void Init()
    {
        if (SessionSample.sessionId != -1) return;

        int key = 0;

        while (key < int.MaxValue)
        {
            if (!PlayerPrefs.HasKey($"battle-scene-session-{key}"))
                break;
            key++;
        }

        if (key == int.MaxValue)
            Debug.LogWarning("Are PlayerPrefs saturated with samples?");

        SessionSample.sessionId = key;
        PlayerPrefs.SetString($"battle-scene-session-{key}", "{}");
    }

    public static void Save()
    {
        var jsonString = JsonUtility.ToJson(SessionSample);
        PlayerPrefs.SetString($"battle-scene-session-{SessionSample.sessionId}", jsonString);
        PlayerPrefs.Save();

        string path = $"Assets/Resources/SessionSamples/battle-scene-session-{SessionSample.sessionId}.txt";
        StreamWriter writer = new StreamWriter(path, false);
        writer.Write(jsonString);
        writer.Close();
    }

    public static SessionSample Load(int sessionId)
    {
        var jsonString = PlayerPrefs.GetString($"battle-scene-session-{SessionSample.sessionId}");
        return JsonUtility.FromJson<SessionSample>(jsonString);
    }
}
