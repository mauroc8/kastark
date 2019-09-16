using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public static class TrailAnalyzer
{
    static float _effectivenessFactor = 1.5f;
    static float _effectivenessPower  = 0.5f;

    public static float StraightLineScore(AttackTrail trail) {
        var screenPoints = trail.screenPoints.ToArray();
        var N = screenPoints.Length;

        var xs     = new float[N];
        var ys     = new float[N];
        var slopes = new float[N - 1];

        float xSum = 0;
        float ySum = 0;
        float slopeSum = 0;


        for (int i = 0; i < N; i++) {
            xs[i] = screenPoints[i].x;
            ys[i] = screenPoints[i].y;

            xSum += xs[i];
            ySum += ys[i];

            if (i > 0) {
                float slope = (ys[i] - ys[i - 1]) / (xs[i] - xs[i - 1]);
                slopes[i - 1] = slope;

                slopeSum += slope;
            }
        }

        float slopeMean = slopeSum / (float)(N - 1);

        float slopeStdDev = 0;

        for (int i = 0; i < N - 1; i++) {
            float dev = slopeMean - slopes[i];
            dev *= dev;
            slopeStdDev += dev;
        }
        slopeStdDev /= N - 1;
        slopeStdDev = Mathf.Sqrt(slopeStdDev);

        var effectiveness = 1 - slopeStdDev * _effectivenessFactor;
        effectiveness = Mathf.Pow(effectiveness, _effectivenessPower);

        return Mathf.Max(0, effectiveness);
    }

}
