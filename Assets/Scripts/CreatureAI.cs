using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class CreatureAI : MonoBehaviour
{
    [SerializeField] CreatureController _creatureController = null;

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
        if (GameState.actingCreature == _creatureController)
        {
            StartCoroutine(ItsMyTurnCoroutine());
        }
    }

    WaitForSeconds _briefWait = new WaitForSeconds(1.12f);

    IEnumerator ItsMyTurnCoroutine()
    {
        yield return _briefWait;

        var target = GameState.creaturesInBattle.Find(creature => !GameState.IsFromActingTeam(creature));
        var hability = _creatureController.creature.habilities[0];

        EventController.TriggerEvent(new HabilitySelectEvent{ hability = hability });
        EventController.TriggerEvent(new HabilityCastEvent{
            targets = new CreatureController[]{target},
            effectiveness = new float[]{0.5f + Random.Range(0, 0.5f)},
            damageType = hability.DamageType,
            damage = hability.Damage
        });
    }
}
