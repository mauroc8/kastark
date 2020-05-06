using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ListExtensions;

public abstract class MultiAlphaController : MonoBehaviour
{
    public abstract List<float> Alphas { get; set; }

    public virtual int AlphasCount => Alphas.Count;

    protected List<float> _startAlphas;

    void Start()
    { _startAlphas = Alphas; }

    public virtual void FadeOut(float duration, float power = 1, Action callback = null)
    {
        StopAllCoroutines();
        StartCoroutine(FadeCoroutine(1, 0, duration, power, callback));
    }

    public virtual void FadeIn(float duration, float power = 1, Action callback = null)
    {
        StopAllCoroutines();
        StartCoroutine(FadeCoroutine(0, 1, duration, power, callback));
    }

    IEnumerator FadeCoroutine(float startAlpha, float endAlpha, float duration, float power, Action callback)
    {
        var startTime = Time.time;

        var startAlphas = ListExtension.Repeat<float>(startAlpha, AlphasCount);
        var endAlphas = ListExtension.Repeat<float>(endAlpha, AlphasCount);

        Alphas = startAlphas;
        yield return null;

        while (Time.time < startTime + duration)
        {
            float t = Mathf.Pow((Time.time - startTime) / duration, power);
            Alphas = ListExtension.Lerp(startAlphas, endAlphas, t, Mathf.Lerp);
            yield return null;
        }

        Alphas = endAlphas;

        if (callback != null)
            callback.Invoke();
    }
}