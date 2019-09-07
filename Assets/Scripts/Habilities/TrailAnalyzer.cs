using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public struct TrailAnalysis {
    public bool horizontal;
    public bool straight;
    public bool vertical;
    public bool circular;
    public bool isLong;
    public bool isShort;
    public bool spike;
    public bool zigZag;
    public bool fast;
    public bool slow;
};

public class TrailAnalyzer
{
    Vector2[] _screenPoints;
    float _minLengthPx;
    float _maxLengthPx;
    float _timeOpened;

    public TrailAnalyzer(Vector2[] screenPoints, float length, float minLength, float maxLength, float timeOpened) {
        _screenPoints = screenPoints;
        _length = length;
        _minLengthPx = minLength;
        _maxLengthPx = maxLength;
        _timeOpened = timeOpened;
    }

    public TrailAnalysis AnalyzeTrail() {
        PerformLevel0Analysis();

        TrailAnalysis results;
        
        results.horizontal = _yStdDev < 2.2f;
        results.vertical = _xStdDev < 2.2f;
        results.straight = _directionStdDev < 0.07f && _spikes == 0;
        results.circular = _spikes == 0 && (
            _directionStdDev > 0.4f && _angleStdDev < 0.17f ||
            _directionStdDev > 0.9f && _angleStdDev < 0.28f
        );
        results.isLong = _length > _maxLengthPx - _minLengthPx && _spikes == 0;
        results.isShort = _length < _minLengthPx * 2.4f;
        results.spike = _spikes == 1;
        results.zigZag = _spikes > 1;
        results.fast = _speed > 1200;
        results.slow = _speed < 150;

        return results;
    }

    // "Level 0" analysis.
    float _length;
    int _samples;
    int _spikes;

    float _xMean;
    float _xMedian;
    float _xStdDev = 0;

    float _yMean;
    float _yMedian;
    float _yStdDev = 0;
    
    float _directionMean;
    float _directionMedian;
    float _directionStdDev = 0;
    
    float _angleMean;
    float _angleMedian;
    float _angleStdDev = 0;
    
    float _speed = 0;

    void PerformLevel0Analysis() {
        int N = _samples = _screenPoints.Length;

        float[] xValue = new float[N];
        float[] yValue = new float[N];
        float[] directionValue = new float[N - 1];
        float[] angleValue = new float[N - 2];

        float xSum = 0;
        float ySum = 0;
        float directionSum = 0;
        float angleSum = 0;

        for (int i = 0; i < N; i++) {
            xSum += xValue[i] = _screenPoints[i].x;
            ySum += yValue[i] = _screenPoints[i].y;
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

        _speed = _length / _timeOpened;
    }

}
