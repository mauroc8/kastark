using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class SessionSampler : MonoBehaviour
{
    void OnEnable()
    {
        EventController.AddListener<HabilityCastEvent>(OnHabilityCast);

        SessionSampleController.Init();
    }

    void OnDisable()
    {
        EventController.RemoveListener<HabilityCastEvent>(OnHabilityCast);
        
        SessionSampleController.Save();
    }

    void OnHabilityCast(HabilityCastEvent evt)
    {
        float effectiveness = evt.effectiveness.Length > 0 ? evt.effectiveness[0] : -1;
        string targetName   = evt.targets.Length > 0 ? evt.targets[0].name : "";

        Debug.Log("Adding sessionSample castHability.");

        SessionSampleController.SessionSample.castHabilities.Add(
            new HabilityCastSample{
                damageType = evt.damageType,
                baseDamage = evt.damage,
                difficulty = GameState.selectedHability.Difficulty,
                effectiveness = effectiveness,
                targetName = targetName
            }
        );
    }
}
