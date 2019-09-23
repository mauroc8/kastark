using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorController : MonoBehaviour
{
    CanvasGroup _canvasGroup = null;
    Material    _material    = null;
    Image       _image       = null;
    
    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _image       = GetComponent<Image>();
        var renderer = GetComponent<Renderer>();
        if (renderer != null) _material = renderer.material;
    }

    public void ChangeOpacity(float a)
    {
        if (_canvasGroup)
        {
            _canvasGroup.alpha = a;
            return;
        }
        if (_image)
        {
            var color = _image.color;
            color.a = a;
            _image.color = color;
            return;
        }
        if (_material)
        {
            var color = _material.color;
            color.a = a;
            _material.color = color;
            return;
        }
        throw new MissingComponentException($"Expected a Renderer, CanvasGroup or Image component in {name}.");
    }

    public float GetOpacity()
    {
        if (_canvasGroup)
            return _canvasGroup.alpha;
        if (_image)
            return _image.color.a;
        if (_material)
            return _material.color.a;

        throw new MissingComponentException($"Expected a Renderer, CanvasGroup or Image component in {name}.");
    }

    public void ChangeColor(Color color)
    {
        if (_image)
        {
            _image.color = color;
            return;
        }
        if (_material)
        {
            _material.color = color;
            return;
        }
        throw new MissingComponentException($"Expected a Renderer or Image component in {name}.");
    }

    public Color GetColor()
    {
        if (_image)
        {
            return _image.color;
        }
        if (_material)
        {
            return _material.color;
        }
        throw new MissingComponentException($"Expected a Renderer or Image component in {name}.");
    }
}
