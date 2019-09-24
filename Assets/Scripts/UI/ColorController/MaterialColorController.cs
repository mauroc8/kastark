using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Renderer))]
public class MaterialColorController : ColorController
{
    Material _material = null;
    
    void Awake()
    {
        _material = GetComponent<Renderer>().material;
    }

    public override void ChangeColor(Color color)
    {
        _material.color = color;
    }

    public override Color GetColor()
    {
        return _material.color;
    }
}
