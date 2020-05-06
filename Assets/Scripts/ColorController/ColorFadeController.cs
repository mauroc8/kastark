using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ColorFadeController : MonoBehaviour
{
    [Header("Fade Variables")]
    [SerializeField] protected float _fadeDuration = 1;
    [SerializeField] protected float _fadePower    = 2;
    
    public float FadeDuration => _fadeDuration;

    public abstract void FadeTo(Color color);
    public abstract void FadeFrom(Color color);
}
