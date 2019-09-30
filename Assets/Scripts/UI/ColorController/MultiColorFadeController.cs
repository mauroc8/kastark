using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiColorFadeController : ColorFadeController
{
    [SerializeField] List<ColorController> _colorControllers = null;

    bool _fading = false;

    List<Color> _fadeFrom;
    List<Color> _fadeTo;
    float _fadeStartTime;

    void SetupFade()
    {
        _fading = true;
        _fadeStartTime = Time.time;
        _fadeFrom = new List<Color>(_colorControllers.Count);
        _fadeTo = new List<Color>(_colorControllers.Count);
    }

    public override void FadeTo(Color color)
    {
        SetupFade();

        for (int i = 0; i < _colorControllers.Count; i++)
        {
            _fadeTo.Add(color);
            _fadeFrom.Add(_colorControllers[i].GetColor());
        }
    }

    public override void FadeFrom(Color color)
    {
        SetupFade();


        for (int i = 0; i < _colorControllers.Count; i++)
        {
            _fadeFrom.Add(color);
            _fadeTo.Add(_colorControllers[i].GetColor());
        }
    }

    void Update()
    {
        if (!_fading) return;

        var t = (Time.time - _fadeStartTime) / _fadeDuration;

        if (t >= 1)
        {
            for (int i = 0; i < _colorControllers.Count; i++)
            {
                _colorControllers[i].ChangeColor(_fadeTo[i]);
            }
            _fading = false;
            return;
        }

        t = Mathf.Pow(t, _fadePower);
        
        for (int i = 0; i < _colorControllers.Count; i++)
        {
            var color = Color.Lerp(_fadeFrom[i], _fadeTo[i], t);
            _colorControllers[i].ChangeColor(color);
        }
    }
}
