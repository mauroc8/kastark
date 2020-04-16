using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Renderer))]
public class EmissionColorController : ColorController
{
    Material _material = null;

    void Awake()
    { _material = GetComponent<Renderer>().material; }

    public override Color MyColor
    {
        get { return _material.GetColor("_EmissionColor"); }
        set { _material.SetColor("_EmissionColor", value); }
    }
}
