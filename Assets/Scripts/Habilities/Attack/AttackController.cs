using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class AttackController : HabilityController
{
    [SerializeField] AttackTrail _attackTrail;

    void Start() {
        _attackTrail = GetComponentInChildren<AttackTrail>();
    }

    void Update()
    {
        if (_cast) return;

        if (Input.GetMouseButton(0))
        {
            if (!_attackTrail.IsOpen)
            {
                _attackTrail.Open(Input.mousePosition);
            } else {
                _attackTrail.Move(Input.mousePosition);
            }

            if (_attackTrail.IsOutOfBounds())
            {
                CloseTrail();
            }
        } else if (_attackTrail.IsOpen)
        {
            CloseTrail();
        }
    }

    void CloseTrail()
    {
        _cast = true;

        if (!_attackTrail.IsAcceptable())
        {
            TryAgain();
            return;
        }

        _attackTrail.Close();

        var targets = _attackTrail.GetTargets();
        var targetsAsList = new List<CreatureController>(targets);

        if (targetsAsList.Exists(creature => !GameState.IsFromActingTeam(creature.gameObject)))
        {
            var effectivenessArray = _attackTrail.GetEffectiveness(difficulty);
            EventController.TriggerEvent(Util.NewHabilityCastEvent(targets, effectivenessArray));
        } else
        {
            // if (habilityLevel < 2) {
                TryAgain();
            //} else "Miss!"
            if (targetsAsList.Exists(creature => GameState.IsFromActingTeam(creature.gameObject)))
            {
                // [HitCreature] "Aw! Are you trying to hurt me?"
            }
        }
    }

    void TryAgain()
    {
        StartCoroutine(RestartTrail());
    }

    WaitForSeconds _briefWait = new WaitForSeconds(0.35f);

    IEnumerator RestartTrail()
    {
        yield return _briefWait;
        _attackTrail.Restart();
        _cast = false;
    }
}
