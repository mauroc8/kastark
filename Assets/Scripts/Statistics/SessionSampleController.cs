using System.Collections;
using System.Collections.Generic;
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
                _instance.Init();
            }
            return _instance;
        }
    }

    void Init()
    {
        if (_sessionSample.sessionId != -1) return;

        int key = 0;

        while (key < int.MaxValue)
        {
            if (!PlayerPrefs.HasKey($"battle-scene-session-{key}"))
                break;
            key++;
        }

        if (key == int.MaxValue)
            Debug.LogWarning("Are PlayerPrefs saturated with samples?");

        _sessionSample.sessionId = key;
    }

    public static void Save()
    {
        var jsonString = JsonUtility.ToJson(Instance._sessionSample);
        PlayerPrefs.SetString($"battle-scene-session-{Instance._sessionSample.sessionId}", jsonString);
        PlayerPrefs.Save();
    }

    public static SessionSample Load(int sessionId)
    {
        var jsonString = PlayerPrefs.GetString($"battle-scene-session-{Instance._sessionSample.sessionId}");
        return JsonUtility.FromJson<SessionSample>(jsonString);
    }

    protected virtual bool Equals(SessionSample other)
    {
        if (!base.Equals(other))
            return false;
        
        if (!SampleListEquality<LanguageSample>(Instance._sessionSample.chosenLanguages, other.chosenLanguages))
            return false;
        
        if (!SampleListEquality<HabilityCastSample>(Instance._sessionSample.castHabilities, other.castHabilities))
            return false;
        
        if (!SampleListEquality<TeamSample>(Instance._sessionSample.battleResults, other.battleResults))
            return false;

        return true;
    }

    static bool SampleListEquality<T>(List<T> samplesA, List<T> samplesB) where T : Sample
    {
        if (samplesA.Count != samplesB.Count)
            return false;

        for (int i = 0; i < samplesA.Count; i++)
        {
            if (!samplesA[i].Equals(samplesB[i]))
                return false;
        }

        return true;
    }

    public static void Test()
    {
        Save();

        bool result = Load(Instance._sessionSample.sessionId).Equals(Instance);

        if (result == false)
        {
            Debug.Log($"SessionSampleController test failed.");
        }
        else
        {
            Debug.Log($"SessionSampleController test succeeded.");
        }
    }
}
