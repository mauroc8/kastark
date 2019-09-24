using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(FadeOutController))]
public class UIRectCountdown : MonoBehaviour
{
    float _countdownTime = 0;
    float _time;

    RectTransform _transform;
    FadeOutController _fadeOutController;
    bool _running;

    public bool Running => _running;

    void Start()
    {
        _transform = GetComponent<RectTransform>();
        _fadeOutController = GetComponent<FadeOutController>();
        _running = false;
    }

    void Update()
    {
        if (_running)
        {
            _time -= Time.deltaTime;
            if (_time < 0) _time = 0;

            var sizeDelta = _transform.sizeDelta;
            sizeDelta.x = _time / _countdownTime * 1920;
            _transform.sizeDelta = sizeDelta;

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
        _fadeOutController.FadeOut();
    }

}
