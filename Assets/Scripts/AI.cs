using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class AI : MonoBehaviour
{
    void OnEnable()
    {
        EventController.AddListener<TurnStartEvent>(OnTurnStart);
    }
    void OnDisable()
    {
        EventController.RemoveListener<TurnStartEvent>(OnTurnStart);
    }
    void OnTurnStart(TurnStartEvent evt)
    {
        
    }
}
