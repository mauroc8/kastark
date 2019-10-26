using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TrailRendererColorController : ColorController
{
    TrailRenderer _trailRenderer;

    void Awake()
    { _trailRenderer = GetComponent<TrailRenderer>(); }

    public override Color MyColor
    {
        get { return _trailRenderer.colorGradient.colorKeys[0].color; }
        set
        {
            for (int i = 0; i < _trailRenderer.colorGradient.colorKeys.Length; i++)
            {
                _trailRenderer.colorGradient.colorKeys[i].color = value;
                _trailRenderer.colorGradient.alphaKeys[i].alpha = value.a;
            }
        }
    }
}
