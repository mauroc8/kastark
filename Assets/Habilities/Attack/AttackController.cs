﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackController : StreamBehaviour
{
    [SerializeField] GameObject _trail;
    [SerializeField] CountdownController _countdownController;

    [Header("Event")]
    [SerializeField] UnityEvent _castEndEvent;

    [Header("Settings")]
    [SerializeField] float _countdownTime = 3.5f;
    [SerializeField] int _maxDamage = 5;

    int _damageMade;

    AttackTrail _attackTrail;
    bool _cast;

    protected override void Awake()
    {
        var creature = GetContext<Creature>();

        enableStream.Get(_ =>
        {
            _countdownController.StartCountdown(_countdownTime);
            _damageMade = 0;
            _cast = false;
        });

        creature.EventStream<CreatureEvts.ReceivedDamage>().Get(_ =>
        {
            _damageMade++;

            if (_damageMade >= _maxDamage)
            {
                Close();
                CastEnd();
            }
        });

        updateStream.Get(_ =>
        {
            if (_cast)
                return;

            if (!_countdownController.IsRunning)
            {
                Close();
                CastEnd();
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                _attackTrail = Instantiate(_trail).GetComponent<AttackTrail>();
                _attackTrail.Open(Input.mousePosition);
            }
            else if (_attackTrail != null)
            {
                if (Input.GetMouseButton(0) && !_attackTrail.IsOutOfBounds)
                {
                    _attackTrail.Move(Input.mousePosition);
                }
                else
                {
                    Close();
                }
            }
        });
    }

    void Close()
    {
        _attackTrail?.Close();
        _attackTrail = null;
    }

    void CastEnd()
    {
        _cast = true;
        _castEndEvent.Invoke();
    }

}
