using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class MultiColorController : MultiAlphaController
{
    public override List<float> Alphas
    {
        get { return MyColors.ConvertAll(color => color.a); }
        set
        {
            Debug.Assert(value.Count == Alphas.Count);
            var newColors = new List<Color>();
            
            for (int i = 0; i < Alphas.Count; i++)
            {
                var newColor = MyColors[i];
                newColor.a = value[i];
                newColors.Add(newColor);
            }

            MyColors = newColors;
        }
    }
    
    public abstract List<Color> MyColors { get; set; }
    public virtual int ColorCount => MyColors.Count;

    public static List<Color> ColorListLerp(List<Color> from, List<Color> to, float t)
    {
        var result = new List<Color>();

        for (int i = 0; i < from.Count; i++)
            result.Add(Color.Lerp(from[i], to[i], t));
        
        return result;
    }

    protected List<Color> _originalColors;

    protected virtual void Start()
    { _originalColors = MyColors; }

    public virtual void FadeTo(
        List<Color> targets,
        float duration,
        float power = 1,
        Action callback = null)
    {
        StartCoroutine(FadeCoroutine(MyColors, targets, duration, power, callback));
    }

    public virtual void FadeTo(
        Color target,
        float duration,
        float power = 1,
        Action callback = null)
    {
        var targets = Enumerable.Repeat(target, ColorCount).ToList();
        StartCoroutine(FadeCoroutine(MyColors, targets, duration, power, callback));
    }

    public virtual void FadeToOriginal(
        float duration,
        float power = 1,
        Action callback = null)
    {
        FadeTo(_originalColors, duration, power, callback);
    }

    public virtual void FadeAndReturn(
        Color target,
        float fadeDuration,
        float returnDuration,
        float power = 1,
        Action callback = null)
    {
        FadeTo(target, fadeDuration, power, () =>
        {
            FadeToOriginal(returnDuration, power, callback);
        });
    }

    IEnumerator FadeCoroutine(
        List<Color> startColors,
        List<Color> endColors,
        float duration,
        float power,
        Action callback)
    {
        var startTime = Time.time;

        MyColors = startColors;
        yield return null;

        while (Time.time < startTime + duration)
        {
            float t = Mathf.Pow((Time.time - startTime) / duration, power);
            MyColors = Helpers.ListLerp<Color>(startColors, endColors, t, Color.Lerp);
            yield return null;
        }
        
        MyColors = endColors;

        if (callback != null)
            callback.Invoke();
    }
}