
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Helpers
{
    public static
    List<T> Repeat<T>(T value, int times)
    { return Enumerable.Repeat(value, times).ToList(); }

    public static
    List<T> ListLerp<T>(List<T> from, List<T> to, float t, Func<T, T, float, T> lerp)
    {
        var result = new List<T>();

        for (int i = 0; i < from.Count; i++)
            result.Add(lerp(from[i], to[i], t));
        
        return result;
    }
    
}
