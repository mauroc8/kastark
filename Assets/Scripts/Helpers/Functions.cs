using System;
using System.Collections;
using UnityEngine;

public static partial class Functions
{
    public static bool IsTypeOf<A, B>(A value) where B : A
        =>
            typeof(B).IsAssignableFrom(value.GetType());

    public static bool Eq<A>(A a, A b)
    {
        return
            a.GetType().IsValueType
                ? StructuralComparisons.StructuralEqualityComparer.Equals(a, b)
                : (object)a == (object)b
            ;
    }

    public static T NullCheck<T>(T val)
    {
        UnityEngine.Debug.Assert(val != null);

        return val;
    }

    public static
    A Log<A>(A value)
    {
        UnityEngine.Debug.Log($"{value}");
        return value;
    }

    // https://math.stackexchange.com/questions/121720/ease-in-out-function

    public static
    float EaseInOutNth(float t, float n)
    {
        if (t < 0) return 0;
        if (t > 1) return 1;

        return
            Mathf.Pow(t, n) /
            (Mathf.Pow(t, n) + Mathf.Pow(1 - t, n));
    }

    public static
    float EaseInOut(float t)
    {
        return EaseInOutNth(t, 2.0f);
    }

    public static
    float SmoothClamp(float x, float min, float max, float n)
    {
        if (x < min)
            return min;

        if (x > max)
            return max;

        var diff = max - min;
        var norm = (x - min) / diff;
        var eased = EaseInOutNth(norm, n);

        return eased * diff + min;
    }


    /// Generates a random float between 0 and 1 from seed.
    public static
    float Random(float seed)
    {
        // https://thebookofshaders.com/10/
        return (Mathf.Sin(seed * 127.9898f) * 478.5423f) % 1;
    }

    public static
    Func<float, Stream<float>> LerpStreamOverTime(Stream<Void> update, float duration)
    {
        var lastValue = 0.0f;
        var currentValue = 0.0f;
        var isInit = false;

        return (float value) =>
        {
            var time = Time.time;

            lastValue = isInit ? currentValue : value;

            isInit = true;

            return update
                .Map(_ =>
                    currentValue =
                        Mathf.Lerp(
                            lastValue,
                            value,
                            (Time.time - time) / duration
                        )
                )
                .Lazy();
        };
    }

    public static
    void PlaySwooshSound(AudioSource swooshSource)
    {
        swooshSource.volume =
            0.4f + UnityEngine.Random.Range(-0.15f, 0.15f);

        swooshSource.pitch =
            1.0f + UnityEngine.Random.Range(-0.15f, 0.15f);

        swooshSource.panStereo =
            UnityEngine.Random.Range(-0.5f, 0.5f);

        swooshSource.Play();
    }
}
