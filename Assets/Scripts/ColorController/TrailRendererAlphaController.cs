using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TrailRendererAlphaController : AlphaController
{
    TrailRenderer _trailRenderer;

    void Awake()
    { _trailRenderer = GetComponent<TrailRenderer>(); }

    public override float Alpha
    {
        get { return _trailRenderer.emitting ? 1 : 0; }
        set
        {
            /*
            // No funciona y perdí la paciencia
            for (int i = 0; i < _trailRenderer.colorGradient.colorKeys.Length; i++)
            {
                _trailRenderer.colorGradient.colorKeys[i].color = value;
                _trailRenderer.colorGradient.alphaKeys[i].alpha = value.a;
            }
             */
            _trailRenderer.emitting = value == 1;
        }
    }
}
