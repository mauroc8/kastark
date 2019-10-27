using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownController : MonoBehaviour
{
    float _progress;
    bool _isRunning = false;

    public bool IsRunning => _isRunning;
    public float Progress => _progress;

    public void StartCountdown(float duration)
    {
        _isRunning = true;
        StartCoroutine(CountdownCoroutine(duration));
    }

    IEnumerator CountdownCoroutine(float duration)
    {
        var startTime = Time.time;

        _progress = 0;

        while (_isRunning && Time.time < startTime + duration)
        {
            _progress = (Time.time - startTime) / duration;
            yield return null;
        }

        if (_isRunning)
            _progress = 1;
        
        yield return null;
        
        _isRunning = false;
    }

    public void StopCountdown()
    {
        _isRunning = false;
    }
}
