using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class CreatureUIHandler : MonoBehaviour
{
    [SerializeField] CreatureController _creatureController = null;

    void OnEnable()
    {
        EventController.AddListener<HabilityCastEvent>(OnHabilityCast);
    }

    void OnDisable()
    {
        EventController.RemoveListener<HabilityCastEvent>(OnHabilityCast);
    }
    
    void OnHabilityCast(HabilityCastEvent evt)
    {
        foreach (var target in evt.targets) if (target == _creatureController)
        {

            return;
        }
    }
}
