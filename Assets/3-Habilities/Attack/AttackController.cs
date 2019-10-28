using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackController : MonoBehaviour
{
    [SerializeField] GameObject _trail;
    [SerializeField] CountdownController _countdownController;
    [SerializeField] UnityEvent _castEndEvent;

    [Header("Countdown time")]
    [SerializeField] float _countdownTime = 2;

    void Start()
    {
        _countdownController.StartCountdown(_countdownTime);
    }

    AttackTrail _attackTrail;
    bool _cast;

    void Update()
    {
        if (_cast)
            return;

        if (!_countdownController.IsRunning)
        {
            _attackTrail?.Close();
            _castEndEvent.Invoke();
            _cast = true;
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
                _attackTrail.Close();
                _attackTrail = null;
            }
        }
    }
}
