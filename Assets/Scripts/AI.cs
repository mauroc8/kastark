using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class AI : MonoBehaviour
{
    [SerializeField] Hability _hability = null;

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

        var target = GameState.creaturesInBattle.Find(creature => !GameState.IsFromActingTeam(creature));

        EventController.TriggerEvent(new HabilitySelectEvent{ hability = _hability });
        EventController.TriggerEvent(Util.NewHabilityCastEvent(target, 0.7f));
        EventController.TriggerEvent(new TurnEndEvent());
    }
}
