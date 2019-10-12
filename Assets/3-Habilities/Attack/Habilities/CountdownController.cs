using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownController : MonoBehaviour
{
    float _countdownTime = 0;
    float _time;
    float _progress;
    bool _running = false;

    public bool Running => _running;
    public float Progress => _progress;

    void Update()
    {
        if (_running)
        {
            _time -= Time.deltaTime;
            if (_time < 0) _time = 0;

            _progress = _time / _countdownTime;

            _running = _time > 0;
        }
    }

    public void StartCountdown(float time)
    {
        _running = true;
        _countdownTime = _time = time;
    }

    public void StopCountdown()
    {
        _running = false;
        _progress = 0;
    }
}
