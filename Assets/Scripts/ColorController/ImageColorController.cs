using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageColorController : ColorController
{
    Image _image;

    void Awake()
    { _image = GetComponent<Image>(); }

    public override Color MyColor
    {
        get { return _image.color; }
        set { _image.color = value; }
    }
}
