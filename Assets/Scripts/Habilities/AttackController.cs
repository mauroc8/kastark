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
        _attackTrail.Close();
        _cast = true;
        EventController.TriggerEvent(new HabilityCastEvent(
            _attackTrail.GetTargets(),
            _attackTrail.GetEffectiveness(difficulty)
        ));
    }
}
