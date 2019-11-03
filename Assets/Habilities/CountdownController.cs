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
        StartCoroutine(CountdownCoroutine(duration));
        _isRunning = true;
    }

    IEnumerator CountdownCoroutine(float duration)
    {
        var startTime = Time.time;

        _progress = 0;
        yield return null;

        while (Time.time < startTime + duration)
        {
            _progress = (Time.time - startTime) / duration;
            yield return null;
        }

        _progress = 1;
        yield return null;
        
        _isRunning = false;
    }

    public void StopCountdown()
    {
        StopAllCoroutines();
        _isRunning = false;
    }
}
