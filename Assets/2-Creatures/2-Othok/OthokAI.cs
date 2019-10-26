﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class OthokAI : MonoBehaviour
{
    [SerializeField] CreatureController _creatureController;

    [SerializeField] List<CreatureController> _enemies;

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
        if (evt.actingCreature == _creatureController)
        {
            StartCoroutine(ItsMyTurnCoroutine());
        }
    }

    WaitForSeconds _briefWait = new WaitForSeconds(1.12f);

    IEnumerator ItsMyTurnCoroutine()
    {
        yield return _briefWait;

        var hability = _creatureController.creature.habilities[0];
        var target = _enemies.Find(creature => !Global.IsFromActingTeam(creature));
        var effectiveness = 0.5f + Random.Range(0, 0.5f);

        EventController.TriggerEvent(new HabilitySelectEvent{ hability = hability });

        // TODO: AI.
        
        EventController.TriggerEvent(new HabilityCastEvent{});
    }
}
