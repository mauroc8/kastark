using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailColorController : ColorController
{
    [SerializeField] TrailRenderer _trailRenderer;

    public override void ChangeColor(Color color)
    {
        _trailRenderer.startColor = color;
        _trailRenderer.endColor = color;
    }

    public override Color GetColor()
    {
        return _trailRenderer.startColor;
    }
}
