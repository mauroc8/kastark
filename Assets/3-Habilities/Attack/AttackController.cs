using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class AttackController : MonoBehaviour
{
    bool _cast;

    [SerializeField] GameObject _attackTrailPrefab = null;
    [SerializeField] CountdownController _uiRectCountdown = null;

    [Header("Countdown time")]
    [SerializeField] float _countdownTime = 2;

    AttackTrail _attackTrail = null;

    void Start()
    {
        CloseAndCreateTrail();
        _uiRectCountdown.StartCountdown(_countdownTime);
    }

    void Update()
    {
        if (_cast) return;

        if (!_uiRectCountdown.Running)
        {
            FinishCasting();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!RaycastHelper.MouseIsOnUI())
            {
                _attackTrail.Open(Input.mousePosition);
            }
        }
        else if (Input.GetMouseButton(0))
        {
            _attackTrail.Move(Input.mousePosition);

            if (_attackTrail.IsOutOfBounds())
            {
                CloseAndCreateTrail();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            CloseAndCreateTrail();
        }
    }

    void CloseAndCreateTrail()
    {
        _attackTrail?.Close();
        var newAttackTrailGo = Instantiate(_attackTrailPrefab);
        _attackTrail = newAttackTrailGo.GetComponent<AttackTrail>();
    }

    void FinishCasting()
    {
        _attackTrail.Close();
        _cast = true;
        EventController.TriggerEvent(new HabilityCastEvent{});
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
