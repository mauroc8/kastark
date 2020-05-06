using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Renderer))]
public class MaterialColorController : ColorController
{
    Material _material = null;

    void Init()
    {
        if (_material == null)
            _material = GetComponent<Renderer>().material;
    }

    public override Color MyColor
    {
        get
        {
            Init();
            return _material.color;
        }
        set
        {
            Init();
            _material.color = value;
        }
    }
}
