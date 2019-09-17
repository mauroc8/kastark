using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class TrailAnalysis
{
    // Results
    public float XMean => _xMean;
    public float YMean => _yMean;
    public float DirectionMean => _directionMean;
    public float AngleMean => _angleMean;
    public float XMedian => _xMedian;
    public float YMedian => _yMedian;
    public float DirectionMedian => _directionMedian;
    public float AngleMedian => _angleMedian;
    public float XStdDev => _xStdDev;
    public float YStdDev => _yStdDev;
    public float DirectionStdDev => _directionStdDev;
    public float AngleStdDev => _angleStdDev;
    public int SampleCount => _sampleCount;
    public int Spikes => _spikes;

    // Statistics
    private float _xMean;
    private float _yMean;
    private float _directionMean;
    private float _angleMean;
    private float _xMedian;
    private float _yMedian;
    private float _directionMedian;
    private float _angleMedian;
    private float _xStdDev = 0;
    private float _yStdDev = 0;
    private float _directionStdDev = 0;
    private float _angleStdDev = 0;
    
    // Other
    private int _sampleCount;
    private int _spikes;

    public TrailAnalysis(Vector2[] screenPoints) {
        int N = _sampleCount = screenPoints.Length;

        float[] xValue = new float[N];
        float[] yValue = new float[N];
        float[] directionValue = new float[N - 1];
        float[] angleValue = new float[N - 2];

        float xSum = 0;
        float ySum = 0;
        float directionSum = 0;
        float angleSum = 0;

        for (int i = 0; i < N; i++) {
            xSum += xValue[i] = screenPoints[i].x;
            ySum += yValue[i] = screenPoints[i].y;
        }
        for (int i = 0; i < N - 1; i++) {
            float dir = Mathf.Atan2(yValue[i + 1] - yValue[i],
                                       xValue[i + 1] - xValue[i]);
            
            if (i > 0) {
                float lastDir = directionValue[i - 1];
                float twoPi = Mathf.PI * 2;
                while (i > 0 && Mathf.Abs(dir + twoPi - lastDir) < Mathf.Abs(dir - lastDir)) {
                    dir += twoPi;
                }
                while (i > 0 && Mathf.Abs(dir - twoPi - lastDir) < Mathf.Abs(dir - lastDir)) {
                    dir -= twoPi;
                }
            }

            directionSum += directionValue[i] = dir;
        }
        for (int i = 0; i < N - 2; i++) {
            angleSum += angleValue[i] = Mathf.PI - (directionValue[i + 1] - directionValue[i]);
        }

        _xMean = xSum / N;
        _yMean = ySum / N;
        _directionMean = directionSum / (N - 1);
        _angleMean = angleSum / (N - 2);

        _xStdDev = 0;
        _yStdDev = 0;
        _directionStdDev = 0;
        _angleStdDev = 0;

        for (int i = 0; i < N; i++) {
            _xStdDev += Mathf.Pow(_xMean - xValue[i], 2);
            _yStdDev += Mathf.Pow(_yMean - yValue[i], 2);
            if (i < N - 1) {
                _directionStdDev += Mathf.Pow(_directionMean - directionValue[i], 2);
                if (i < N - 2) {
                    _angleStdDev += Mathf.Pow(_angleMean - angleValue[i], 2);
                }
            }
        }

        _xStdDev = Mathf.Sqrt(_xStdDev / N);
        _yStdDev = Mathf.Sqrt(_yStdDev / N);
        _directionStdDev = Mathf.Sqrt(_directionStdDev / (N - 1));
        _angleStdDev = Mathf.Sqrt(_angleStdDev / (N - 2));

        _spikes = 0;
        
        float spikeThreshold = Mathf.PI / 2;
        int frac = 4;

        for (int i = (N - 2)/frac; i <= (frac-1)*(N - 2)/frac; i++) {
            if (angleValue[i] < spikeThreshold || angleValue[i] > Mathf.PI*2 - spikeThreshold) {
                _spikes++;
            }
        }

        Array.Sort(xValue);
        Array.Sort(yValue);
        Array.Sort(directionValue);
        Array.Sort(angleValue);

        _xMedian = xValue[N / 2];
        _yMedian = yValue[N / 2];
        _directionMedian = directionValue[(N - 1) / 2];
        _angleMedian = angleValue[(N - 2) / 2];
    }
}
