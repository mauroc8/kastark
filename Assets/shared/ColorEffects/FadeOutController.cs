using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AlphaController))]
public class FadeOutController : MonoBehaviour
{
    [SerializeField] AlphaController _alphaController = null;

    [Header("Fade")]
    [SerializeField] float _fadeTime = 0.2f;
    [SerializeField] float _fadePower = 1.6f;
    [Header("Settings")]
    [SerializeField] bool _autoDisable = false;
    [SerializeField] bool _fadeOnEnable = false;

    bool _fading = false;
    float _time = 0;

    void OnEnable()
    {
        if (_fadeOnEnable)
        {
            FadeOut();
        }
    }

    Action _callback = null;

    public void FadeOut()
    {
        _time = _fadeTime;
        _fading = true;
    }

    public void FadeOut(Action callback)
    {
        FadeOut();
        _callback = callback;
    }

    void Update()
    {
        if (!_fading) return;

        _time -= Time.deltaTime;

        if (_time <= 0)
        {
            _time = 0;
            _fading = false;
            if (_callback != null)
                _callback.Invoke();
            if (_autoDisable)
                enabled = false;
        }

        var alpha = Mathf.Pow(_time / _fadeTime, _fadePower);
        _alphaController.ChangeAlpha(alpha);
    }
}
