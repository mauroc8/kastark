using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Renderer))]
public class MaterialColorController : ColorController
{
    [SerializeField] Material _material = null;
    [SerializeField] bool _useSelfMaterial = false;
    
    void Awake()
    {
        if (_useSelfMaterial)
        {
            _material = GetComponent<Renderer>().material;
        }
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
