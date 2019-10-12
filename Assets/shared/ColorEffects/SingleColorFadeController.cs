using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleColorFadeController : ColorFadeController
{
    [SerializeField] ColorController _colorController = null;

    bool _fading = false;

    Color _fadeFrom;
    Color _fadeTo;
    float _fadeStartTime;

    public override void FadeTo(Color color)
    {
        _fading = true;
        _fadeFrom = _colorController.GetColor();
        _fadeTo = color;
        _fadeStartTime = Time.time;
    }

    public override void FadeFrom(Color color)
    {
        _fading = true;
        _fadeTo = _colorController.GetColor();
        _fadeFrom = color;
        _fadeStartTime = Time.time;
    }

    void Update()
    {
        if (!_fading) return;

        var t = (Time.time - _fadeStartTime) / _fadeDuration;

        if (t >= 1)
        {
            _colorController.ChangeColor(_fadeTo);
            _fading = false;
            return;
        }

        t = Mathf.Pow(t, _fadePower);
        
        var color = Color.Lerp(_fadeFrom, _fadeTo, t);
        _colorController.ChangeColor(color);
    }
}
