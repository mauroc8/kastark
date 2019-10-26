using System;
using System.Collections;
using UnityEngine;

public abstract class ColorController : AlphaController
{
    public abstract Color MyColor { get; set; }

    public override float Alpha
    {
        get { return MyColor.a; }
        set
        {
            var color = MyColor;
            color.a = value;
            MyColor = color;
        }
    }

    protected Color _originalColor;

    protected virtual void Start()
    { _originalColor = MyColor; }

    public virtual void FadeTo(Color target, float duration, float power = 1, Action callback = null)
    {
        StartCoroutine(FadeCoroutine(MyColor, target, duration, power, callback));
    }

    public virtual void FadeToOriginal(float duration, float power = 1, Action callback = null)
    {
        FadeTo(_originalColor, duration, power, callback);
    }

    public virtual void FadeAndReturn(Color target, float fadeDuration, float returnDuration, float power = 1, Action callback = null)
    {
        FadeTo(target, fadeDuration, power, () =>
        {
            FadeToOriginal(returnDuration, power, callback);
        });
    }

    IEnumerator FadeCoroutine(Color startColor, Color endColor, float duration, float power, Action callback)
    {
        var startTime = Time.time;

        MyColor = startColor;
        yield return null;

        while (Time.time < startTime + duration)
        {
            float t = Mathf.Pow((Time.time - startTime) / duration, power);
            MyColor = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
        
        MyColor = endColor;

        if (callback != null)
            callback.Invoke();
    }
}