using System;
using System.Collections;
using UnityEngine;

public abstract class AlphaController : MonoBehaviour
{
    public abstract float Alpha { get; set; }
    
    public virtual void FadeOut(float duration, float power = 1, Action callback = null)
    {
        StartCoroutine(FadeCoroutine(1, 0, duration, power, callback));
    }

    public virtual void FadeIn(float duration, float power = 1, Action callback = null)
    {
        StartCoroutine(FadeCoroutine(0, 1, duration, power, callback));
    }

    IEnumerator FadeCoroutine(float startAlpha, float endAlpha, float duration, float power, Action callback)
    {
        var startTime = Time.time;

        Alpha = startAlpha;
        yield return null;

        while (Time.time < startTime + duration)
        {
            float t = Mathf.Pow((Time.time - startTime) / duration, power);
            Alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }
        
        Alpha = endAlpha;

        if (callback != null)
            callback.Invoke();
    }
}