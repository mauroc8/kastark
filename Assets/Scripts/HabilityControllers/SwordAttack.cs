using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [Header("Trail configuration")]
    [SerializeField] float _minLength = 50;
    [SerializeField] float _maxLength = 300;
    [SerializeField] float _distanceToCamera = 4;
    
    bool _opened = false;
    bool _closed = false;

    public bool IsClosed() { return _closed; }

    Plane _rayCastPlane;
    TrailRenderer _trailRenderer = null;
    float _minVertexDistance;
    float _trailTime;
    Material _material;

    void Start()
    {
        _rayCastPlane = new Plane(
            Camera.main.transform.forward * -1,
            Camera.main.transform.position + Camera.main.transform.forward * _distanceToCamera);
        _trailRenderer = GetComponent<TrailRenderer>();
        _trailRenderer.enabled = false;
        _minVertexDistance = _trailRenderer.minVertexDistance;
        _trailTime = _trailRenderer.time;
        _material = _trailRenderer.materials[0];
    }

    List<Vector3> _points = new List<Vector3>(100);
    List<Vector2> _screenPoints = new List<Vector2>(100);
    List<GameObject> _targets = new List<GameObject>(100);

    float _openedTime = 0;

    void OpenTrail(Vector3 start) {
        AddPoint(start, true);
        _opened = true;
        _trailRenderer.emitting = _trailRenderer.enabled = true;
        _trailRenderer.time = float.PositiveInfinity;

        _length = 0;
        _openedTime = Time.time;
    }

    void MoveTrailTowards(Vector3 point) {
        float distance = (transform.position - point).magnitude;
        if (distance < _minVertexDistance) {
            return;
        }

        AddPoint(point);
    }

    Vector2 _lastScreenPoint;

    void AddPoint(Vector3 point, bool first = false) {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(point);

        if (!first) {
            float distance = (screenPoint - _lastScreenPoint).magnitude;
            
            if (_length + distance > _maxLength) {
                CloseTrail();
                return;
            }

            _length += distance;
        }
        
        _lastScreenPoint = screenPoint;

        _points.Add(point);
        _screenPoints.Add(screenPoint);
        transform.position = point;
    }

    void AddTarget(GameObject target) {
        _targets.Add(target);
    }

    void CloseTrail() {
        _opened = _trailRenderer.emitting = false;
        _closed = true;

        if (!IsAcceptable()) {
            Debug.Log($"Inacceptable trail, try again.");

            Restart();

        } else {
            _trailRenderer.time = Time.time - _openedTime + _trailTime;

            AnalyzeTrail();
        }
    }

    void Restart() {
        _closed = false;
        _opened = false;
        _trailRenderer.enabled = false;
        _trailRenderer.time = _trailTime;
        _points.Clear();
        _screenPoints.Clear();
        _targets.Clear();
    }

    void DebugDrawTrail() {
        Color[] colors = {Color.green, Color.blue, Color.red, Color.black};

        for (int i = 1; i < _points.Count; i++) {
            Debug.DrawLine(_points[i - 1], _points[i], colors[i % 4], 2.5f);
        }
    }
    
    void Update()
    {
        if (_closed) return;

        if (Input.GetMouseButton(0))
        {
            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            float rayDistance;
            if (_rayCastPlane.Raycast(mRay, out rayDistance))
            {
                Vector3 point = mRay.GetPoint(rayDistance);
                if (!_opened) {
                    OpenTrail(point);
                    return;
                } else {
                    MoveTrailTowards(point);
                }
                
                RaycastHit hit;
                if (Physics.Raycast(mRay, out hit)){
                    if (hit.transform.gameObject.CompareTag("Enemy")) {
                        AddTarget(hit.transform.gameObject);
                    }
                }
            }
        } else if (_opened) {
            CloseTrail();
        }
    }

    bool IsAcceptable() {
        return _length > _minLength && _points.Count >= 3;
    }

    [Header("Public Trail Analysis")]
    public bool horizontal;
    public bool straight;
    public bool vertical;
    public bool circular;
    public bool isLong;
    public bool isShort;
    public bool spike;
    public bool zigZag;

    void AnalyzeTrail() {
        MakeStatisticAnalysis();
        
        horizontal = _yStdDev < 3.2f;
        vertical = _xStdDev < 3.2f;
        straight = _directionStdDev < 0.09f && _spikes == 0;
        circular = _spikes == 0 && (
            _directionStdDev > 0.4f && _angleStdDev < 0.17f ||
            _directionStdDev > 0.9f && _angleStdDev < 0.28f
        );
        isLong = _length > _maxLength - _minLength && _spikes == 0;
        isShort = _length < _minLength * 2.4f;
        spike = _spikes == 1;
        zigZag = _spikes > 1;
    }

    [Header("Private Trail Statistics")]
    [SerializeField] float _length;
    [SerializeField] int _samples;
    [SerializeField] int _spikes;

    [SerializeField] float _xMean;
    [SerializeField] float _xMedian;
    [SerializeField] float _xStdDev = 0;

    [SerializeField] float _yMean;
    [SerializeField] float _yMedian;
    [SerializeField] float _yStdDev = 0;
    
    [SerializeField] float _directionMean;
    [SerializeField] float _directionMedian;
    [SerializeField] float _directionStdDev = 0;
    
    [SerializeField] float _angleMean;
    [SerializeField] float _angleMedian;
    [SerializeField] float _angleStdDev = 0;
    
    void MakeStatisticAnalysis() {
        Vector2[] screenPoints = _screenPoints.ToArray();

        int N = _samples = screenPoints.Length;

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
