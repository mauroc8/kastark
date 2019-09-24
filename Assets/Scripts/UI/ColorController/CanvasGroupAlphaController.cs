using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupAlphaController : AlphaController
{
    [SerializeField] CanvasGroup _canvasGroup = null;

    public override void ChangeAlpha(float a)
    {
        _canvasGroup.alpha = a;
        return;
    }

    public override float GetAlpha()
    {
        return _canvasGroup.alpha;
    }
}
