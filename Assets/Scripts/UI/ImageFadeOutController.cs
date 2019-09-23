using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFadeOutController : MonoBehaviour
{
    [Tooltip("Leave null to look for an Image component in this GameObject.")]
    [SerializeField] Image _image = null;

    [Header("Fade settings")]
    [SerializeField] float _fadeTime = 0.2f;
    [SerializeField] float _fadePower = 1.6f;
    [SerializeField] bool _fadeOnStart = false;

    bool _fading;
    float _time = 0;

    void Start()
    {
        if (_image == null) _image = GetComponent<Image>();
        _fading = _fadeOnStart;
    }

    Action _callback = null;

    public void FadeOut(Action callback)
    {
        _time = _fadeTime;
        _callback = callback;
        _fading = true;
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
        }

        var color = _image.color;
        color.a = Mathf.Pow(_time / _fadeTime, _fadePower);
        _image.color = color;
    }
}
