using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class AI : MonoBehaviour
{
    [SerializeField] Hability _hability = null;
    [SerializeField] CreatureController _target = null;

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
        if (GameState.IsPlayersTurn()) return;

        StartCoroutine(EnemyTurn());
    }

    WaitForSeconds _briefWait = new WaitForSeconds(0.2f);

    IEnumerator EnemyTurn()
    {
        yield return _briefWait;
        EventController.TriggerEvent(new HabilitySelectEvent{ hability = _hability });
        EventController.TriggerEvent(new HabilityCastEvent(_target, 0.7f));
        EventController.TriggerEvent(new TurnEndEvent());
    }
}
