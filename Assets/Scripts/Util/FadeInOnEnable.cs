using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOnEnable : ColorController
{
    [SerializeField] float _fadeInDuration = 1;
    [SerializeField] float _fadePower      = 2;
    [SerializeField] bool  _autoDisable    = true;

    float _fadeInStart;
    bool _animating;

    void OnEnable()
    {
        ChangeOpacity(0);
        
        _fadeInStart = Time.time;
        _animating = true;
    }

    void Update()
    {
        if (!_animating) return;

        var t = (Time.time - _fadeInStart) / _fadeInDuration;
        if (t < 1) {
            t = Mathf.Pow(t, _fadePower);
            ChangeOpacity(t);
        } else {
            ChangeOpacity(1);
            _animating = false;
            if (_autoDisable) enabled = false;
        }
    }
}
