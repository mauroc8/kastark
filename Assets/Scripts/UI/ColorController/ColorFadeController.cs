using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorFadeController : MonoBehaviour
{
    [SerializeField] ColorController _colorController = null;

    [Header("Fade Variables")]
    [SerializeField] float _fadeDuration = 1;
    [SerializeField] float _fadePower    = 2;

    bool _fading = false;

    Color _fadeFrom;
    Color _fadeTo;
    float _fadeStartTime;

    public void FadeTo(Color color)
    {
        _fading = true;
        _fadeFrom = _colorController.GetColor();
        _fadeTo = color;
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
