using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackController : MonoBehaviour
{
    [SerializeField] GameObject _trail;
    [SerializeField] CountdownController _countdownController;

    [Header("Event")]
    [SerializeField] UnityEvent _castEndEvent;

    [Header("Countdown time")]
    [SerializeField] float _countdownTime = 3.5f;

    int _damageMade;

    void OnEnable()
    {
        _countdownController.StartCountdown(_countdownTime);
        _damageMade = 0;
    }

    AttackTrail _attackTrail;
    bool _cast;

    void Update()
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
    }

    public void OnLifePointHit(GameObject lifePoint)
    {
        _damageMade++;

        if (_damageMade >= 5)
        {
            Close();
            CastEnd();
        }
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
