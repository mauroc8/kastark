﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class AttackController : HabilityController
{
    [SerializeField] AttackTrail _attackTrail = null;
    [SerializeField] UIRectCountdown _uiRectCountdown = null;

    void Update()
    {
        if (_cast) return;

        if (Input.GetMouseButton(0))
        {
            if (!_attackTrail.IsOpen)
            {
                _attackTrail.Open(Input.mousePosition);
                _uiRectCountdown.StartCountdown(_attackTrail.maxLifetime);
            } else {
                _attackTrail.Move(Input.mousePosition);
            }

            if (_attackTrail.IsOutOfBounds() || !_uiRectCountdown.Running)
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

        _uiRectCountdown.StopCountdown();

        if (!_attackTrail.IsAcceptable())
        {
            TryAgain();
            return;
        }

        _attackTrail.Close();

        var targets = _attackTrail.GetTargets();

        if (targets.Length > 0)
        {
            var effectivenessArray = _attackTrail.GetEffectiveness(difficulty);
            var habilityCastController = new HabilityCastController{
                targets = targets,
                effectiveness = effectivenessArray,
                hability = GameState.selectedHability
            };
            habilityCastController.Cast();
        }
        else
        {
            Debug.Log($"Attack trail has no enemy targets.");
            TryAgain();
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
        Debug.Log("Try Again!");
        _attackTrail.Restart();
        _cast = false;
    }
}
