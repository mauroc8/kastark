using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiColorFadeController : ColorFadeController
{
    [SerializeField] MultiColorController _multiColorController = null;

    bool _fading = false;

    Color[] _fadeFrom;
    Color[] _fadeTo;
    Color[] _intermediateColors;
    float _fadeStartTime;

    public override void FadeTo(Color color)
    {
        _fading = true;
        _fadeStartTime = Time.time;

        _fadeFrom = _multiColorController.GetColors();
        _fadeTo = new Color[_fadeFrom.Length];
        _intermediateColors = new Color[_fadeTo.Length];

        for (int i = 0; i < _fadeFrom.Length; i++)
        {
            _fadeTo[i] = color;
        }
    }

    public override void FadeFrom(Color color)
    {
        _fading = true;
        _fadeStartTime = Time.time;

        _fadeTo = _multiColorController.GetColors();
        _fadeFrom = new Color[_fadeTo.Length];
        _intermediateColors = new Color[_fadeTo.Length];

        for (int i = 0; i < _fadeTo.Length; i++)
        {
            _fadeFrom[i] = color;
        }
    }

    void Update()
    {
        if (!_fading) return;

        var t = (Time.time - _fadeStartTime) / _fadeDuration;

        if (t >= 1)
        {
            for (int i = 0; i < _fadeTo.Length; i++)
            {
                _multiColorController.ChangeColors(_fadeTo);
            }
            _fading = false;
            // Cleanup
            _fadeFrom = null;
            _fadeTo = null;
            _intermediateColors = null;
            return;
        }

        t = Mathf.Pow(t, _fadePower);

        for (int i = 0; i < _fadeTo.Length; i++)
        {
            _intermediateColors[i] = Color.Lerp(_fadeFrom[i], _fadeTo[i], t);
        }

        _multiColorController.ChangeColors(_intermediateColors);
    }
}
