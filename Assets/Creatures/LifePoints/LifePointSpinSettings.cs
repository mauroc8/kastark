using System;
using System.Collections.Generic;
using UnityEngine;

public class LifePointSpinSettings
{
    public float speed = 1;
    public float radius = 1;
    public float minHeight = 1;
    public float maxHeight = 1;
    public float amountOfTurns = 1;
    public float oscillationAmount = 0;
    public float oscillationSpeed = 1;

    public Func<float, Vector3> GetPosition(int index, int maxLifePoints, float creatureHeight)
    {
        // Constants:

        var percentage = (float)(index + 1) / (float)maxLifePoints;

        var Speed = this.speed / this.radius * Mathf.PI * 2;
        var BaseHeight = Mathf.Lerp(this.minHeight, this.maxHeight, percentage) * creatureHeight;
        var Offset = percentage * this.amountOfTurns * Mathf.PI * 2;
        var Radius = this.radius;

        var Speed0 = oscillationSpeed / this.radius;
        var Radius0 = oscillationAmount * creatureHeight;

        // Functions:

        Func<float, Vector3> position = time => new Vector3(
            Mathf.Sin(Offset + time * Speed) * Radius,
            BaseHeight + Mathf.Sin(Offset + time * Speed0) * Radius0,
            Mathf.Cos(Offset + time * Speed) * Radius
        );

        // integrate of position(t) = speed(t)

        Func<float, Vector3> speed = t => new Vector3(
            -Radius * Mathf.Cos(Offset + Speed * t) / Speed,
            BaseHeight * t - Radius0 * Mathf.Cos(Offset + Speed0 * t) / Speed0,
            Radius * Mathf.Sin(Offset + Speed * t) / Speed
        );

        // integrate of speed(t) = acceleration(t)

        Func<float, Vector3> acceleration = t => new Vector3(
            -Radius * Mathf.Sin(Offset + Speed * t) / (Speed * Speed),
            BaseHeight * t * t / 2 + Radius0 * Mathf.Sin(Offset + Speed0 * t) / (Speed0 * Speed0),
            -Radius * Mathf.Cos(Offset + Speed * t) / (Speed * Speed)
        );

        return position;
    }
}
