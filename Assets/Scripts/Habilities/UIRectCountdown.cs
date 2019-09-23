using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(ImageFadeOutController))]
public class UIRectCountdown : MonoBehaviour
{
    float _countdownTime = 0;
    float _time;

    RectTransform _transform;
    ImageFadeOutController _fadeOutController;

    void Start()
    {
        _transform = GetComponent<RectTransform>();
        _fadeOutController = GetComponent<ImageFadeOutController>();
    }

    void Update()
    {
        if (_time > 0)
        {
            _time -= Time.deltaTime;
            if (_time < 0) _time = 0;

            var sizeDelta = _transform.sizeDelta;
            sizeDelta.x = _time / _countdownTime * 1920;
            _transform.sizeDelta = sizeDelta;
        }
    }

    public void StartCountdown(float time)
    {
        _countdownTime = _time = time;
    }

    public void StopCountdown()
    {
        _fadeOutController.FadeOut(() => { _time = 0; });
    }

    public bool Running()
    {
        return _time > 0;
    }
}
