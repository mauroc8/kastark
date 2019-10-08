using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AlphaController))]
public class FadeInController : MonoBehaviour
{
    [SerializeField] AlphaController _alphaController = null;
    [Header("Fade")]
    [SerializeField] float _fadeInDuration = 1;
    [SerializeField] float _fadePower      = 2;
    [Header("Settings")]
    [SerializeField] bool  _autoDisable    = true;
    [SerializeField] bool  _fadeOnEnable  = true;
    
    float _fadeInStart;
    bool _animating = false;

    void OnEnable()
    {
        _alphaController.ChangeAlpha(0);
        if (_fadeOnEnable)
        {
            FadeIn();
        }
    }

    void Update()
    {
        if (!_animating) return;

        var t = (Time.time - _fadeInStart) / _fadeInDuration;
        if (t < 1) {
            t = Mathf.Pow(t, _fadePower);
            _alphaController.ChangeAlpha(t);
        } else {
            _alphaController.ChangeAlpha(1);
            _animating = false;
            if (_autoDisable) enabled = false;
        }
    }

    public void FadeIn()
    {
        _fadeInStart = Time.time;
        _animating = true;
    }
}
