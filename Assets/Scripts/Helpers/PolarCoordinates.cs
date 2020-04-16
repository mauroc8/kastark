using System;
using UnityEngine;

public static class Angle
{
    public static readonly float Turn = Mathf.PI * 2;

    public static float Normalize(float angle) =>
        angle > 0
            ? angle % Turn
            : angle % Turn + Turn;

    public static float Lerp(float from, float to, float amount)
    {
        from = Normalize(from);
        to = Normalize(to);

        if (from < to)
        {
            if (Mathf.Abs(to - from) < Mathf.Abs(from + Turn - to))
                return Mathf.Lerp(from, to, amount);
            else
                return Mathf.Lerp(from + Turn, to, amount);
        }
        else
        {
            if (Mathf.Abs(from - to) < Mathf.Abs(to + Turn - from))
                return Mathf.Lerp(from, to, amount);
            else
                return Mathf.Lerp(from, to + Turn, amount);
        }
    }

    public static float Distance(float from, float to)
    {
        from = Normalize(from);
        to = Normalize(to);

        return
            Mathf.Min(
                Mathf.Min(
                    Mathf.Abs(from - to),
                    Mathf.Abs((from + Turn) - to)
                ),
                Mathf.Abs(from - (to + Turn))
            );
    }

    public static float Minus(float a, float b)
    {
        a = Normalize(a);
        b = Normalize(b);

        return
            a - b > Turn / 2 || a - b < -Turn / 2
                ? b - a
                : a - b;
    }
}

public struct PolarVector2
{
    public float radius;
    public float rotation;

    public PolarVector2(float radius, float rotation)
    {
        this.radius = radius;
        this.rotation = rotation;
    }

    public static PolarVector2 Lerp(PolarVector2 from, PolarVector2 to, float amount)
    {
        return new PolarVector2(
            Mathf.Lerp(from.radius, to.radius, amount),
            Angle.Lerp(from.rotation, to.rotation, amount)
        );
    }

    public static PolarVector2 zero => new PolarVector2(0, 0);

    public PolarVector2 WithRadius(float radius)
    {
        return new PolarVector2(radius, rotation);
    }

    public PolarVector2 WithRotation(float rotation)
    {
        return new PolarVector2(radius, rotation);
    }

    public Vector2 ToVector2()
    {
        return new Vector2
        (
            x: Mathf.Cos(rotation) * radius,
            y: Mathf.Sin(rotation) * radius
        );
    }

    public static PolarVector2 FromVector2(Vector2 vector2)
    {
        return new PolarVector2
        (
            radius: Mathf.Sqrt(vector2.x * vector2.x + vector2.y * vector2.y),
            rotation: Mathf.Atan2(vector2.y, vector2.x)
        );
    }
}


/// <summary>
/// Spherical coordinates. Instead of x, y, z we use radius, rotation, elevation.
///
/// See https://blog.nobel-joergensen.com/2010/10/22/spherical-coordinates-in-unity/
/// </summary>
public struct PolarVector3
{
    public float radius;
    public float rotation;
    public float elevation;

    public PolarVector3(float radius, float rotation, float elevation)
    {
        this.radius = radius;
        this.rotation = rotation;
        this.elevation = elevation;
    }

    public static PolarVector3 Lerp(PolarVector3 from, PolarVector3 to, float amount)
    {
        return new PolarVector3(
            Mathf.Lerp(from.radius, to.radius, amount),
            Angle.Lerp(from.rotation, to.rotation, amount),
            Angle.Lerp(from.elevation, to.elevation, amount)
        );
    }

    public static PolarVector3 zero => new PolarVector3(0, 0, 0);

    public PolarVector3 WithRadius(float radius)
    {
        return new PolarVector3(radius, rotation, elevation);
    }

    public PolarVector3 WithRotation(float angle)
    {
        return new PolarVector3(radius, angle, elevation);
    }

    public PolarVector3 WithElevation(float elevation)
    {
        return new PolarVector3(radius, rotation, elevation);
    }

    public Vector3 ToVector3()
    {
        float a = radius * Mathf.Cos(elevation);

        return new Vector3
        (
            x: a * Mathf.Cos(rotation),
            y: radius * Mathf.Sin(elevation),
            z: a * Mathf.Sin(rotation)
        );
    }

    public static PolarVector3 FromVector3(Vector3 vector3)
    {
        var (x, y, z) = (vector3.x, vector3.y, vector3.z);

        var radius = Mathf.Sqrt(x * x + y * y + z * z);

        if (radius == 0)
            return new PolarVector3(0, 0, 0);

        return new PolarVector3
        (
            radius: radius,
            rotation: Mathf.Atan2(z, x),
            elevation: Mathf.Asin(y / radius)
        );
    }

    public PolarVector3 Minus(PolarVector3 other)
    {
        return new PolarVector3
        (
            radius: this.radius - other.radius,
            rotation: Angle.Minus(this.rotation, other.rotation),
            elevation: Angle.Minus(this.elevation, other.elevation)
        );
    }
}
