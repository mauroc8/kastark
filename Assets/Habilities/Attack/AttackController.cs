using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class AttackController : StreamBehaviour
{
    [SerializeField] GameObject _trail;
    [SerializeField] CountdownController _countdownController;
    [SerializeField] Animator _animator = null;

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
        var creature = GetComponentInParent<Creature>();

        enable.Get(_ =>
        {
            _countdownController.StartCountdown(_countdownTime);
            _damageMade = 0;
            _cast = false;
        });

        creature.Events
            .FilterMap(Optional.FromCast<CreatureEvt, CreatureEvts.ReceivedDamage>)
            .Get(_ =>
            {
                _damageMade++;

                if (_damageMade >= _maxDamage)
                {
                    Close();
                    CastEnd();
                }
            });

        update.Get(_ =>
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
                Open();
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

        // Audio source when open

        var swooshSound =
            GetComponent<AudioSource>();

        open
            .Get(_ =>
            {
                Functions.PlaySwooshSound(swooshSound);
            });
    }

    EventStream<Void> open =
        new EventStream<Void>();

    void Open()
    {
        _attackTrail = Instantiate(_trail).GetComponent<AttackTrail>();
        _attackTrail.Open(Input.mousePosition);

        if (_animator != null)
            _animator.SetTrigger("attack_sword");

        open.Push(new Void());
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
