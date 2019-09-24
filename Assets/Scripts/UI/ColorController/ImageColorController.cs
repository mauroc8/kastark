using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageColorController : ColorController
{
    [SerializeField] Image _image = null;

    public override void ChangeColor(Color color)
    {
        _image.color = color;
    }

    public override Color GetColor()
    {
        return _image.color;
    }
}
