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
    bool _AIDrawing = false;

    public bool IsClosed() { return _closed; }

    Plane _rayCastPlane;
    TrailRenderer _trailRenderer = null;
    float _minVertexDistance;
    float _trailExtraLifeTime;
    Material _material;

    void Start()
    {
        _rayCastPlane = new Plane(
            Camera.main.transform.forward * -1,
            Camera.main.transform.position + Camera.main.transform.forward * _distanceToCamera);
        _trailRenderer = GetComponent<TrailRenderer>();
        _trailRenderer.enabled = false;
        _minVertexDistance = _trailRenderer.minVertexDistance;
        
        _material = _trailRenderer.materials[0];

        _trailExtraLifeTime = _trailRenderer.time;
        _trailRenderer.time = float.PositiveInfinity;
    }

    List<Vector3> _worldPoints = new List<Vector3>(50);
    List<Vector2> _screenPoints = new List<Vector2>(50);
    List<GameObject> _targets = new List<GameObject>(50);

    float _openedTime;
    float _timeOpened; // ellapsed time from open to close

    void OpenTrail(Vector3 worldPoint) {
        _opened = true;
        _length = 0;
        _openedTime = Time.time;
        
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPoint);

        AddPoint(worldPoint, screenPoint);
        
        _trailRenderer.emitting = _trailRenderer.enabled = true;
    }

    Vector2 _lastScreenPoint;

    void MoveTrailTowards(Vector3 worldPoint) {
        float worldDistance = (transform.position - worldPoint).magnitude;

        if (worldDistance < _minVertexDistance) {
            return;
        }

        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPoint);

        float screenDistance = (screenPoint - _lastScreenPoint).magnitude;

        if (_length + screenDistance > _maxLength) {
            CloseTrail();
            return;
        }

        _length += screenDistance;

        AddPoint(worldPoint, screenPoint);
    }

    void AddPoint(Vector3 point, Vector2 screenPoint) {
        _worldPoints.Add(point);
        _screenPoints.Add(screenPoint);

        transform.position = point;
        _lastScreenPoint = screenPoint;
    }

    void AddTarget(GameObject target) {
        _targets.Add(target);
    }

    void CloseTrail() {
        _opened = _trailRenderer.emitting = false;
        _closed = true;
        _timeOpened = Time.time - _openedTime;

        if (!IsAcceptable()) {
            if (!_AIDrawing) {
                AIDrawTrailStart();
            } else {
                Debug.Log("AI Drawn Trail is not acceptable.");
            }
        } else {
            _trailRenderer.time = _trailExtraLifeTime;

            AnalyzeTrail();
        }
    }

    WaitForSeconds _briefWait = new WaitForSeconds(0.05f);

    void AIDrawTrailStart() {
        _AIDrawing = true;

        if (_targets.Count < 1) {
            Debug.Log("Please try again.");
            return;
        }

        Debug.Log("Manual Trail");

        Vector2[] screenPoints = _screenPoints.ToArray();

        int N = screenPoints.Length;

        float[] xValues = new float[N];
        float[] yValues = new float[N];
        
        for (int i = 0; i < N; i++) {
            Vector2 p = screenPoints[i];
            xValues[i] = p.x;
            yValues[i] = p.y;
        }

        Array.Sort(xValues);
        Array.Sort(yValues);

        float xMedian = xValues[(int) N / 2];
        float yMedian = yValues[(int) N / 2];

        RestartTrail();
        N = 20;
        _AIGeneratedTrail = new Vector2[N];
        float xSize = 100;
        float ySize = 16;

        for (int i = 0; i < N; i++) {
            float progress = (float) i / N;
            float x = xMedian + (-1 + 2 * progress) * xSize;
            float y = yMedian + (-1 + 2 * progress) * ySize;
            Vector2 generatedPoint = new Vector2(x, y);

            _AIGeneratedTrail[i] = generatedPoint;
        }

        _AITrailIdx = 0;
    }

    Vector2[] _AIGeneratedTrail;
    int _AITrailIdx;

    void AIDrawTrailUpdate() {
        if (_AITrailIdx >= _AIGeneratedTrail.Length) {
            CloseTrail();
        } else {
            HandleClick(_AIGeneratedTrail[_AITrailIdx]);
        }
        
        _AITrailIdx++;
    }

    void RestartTrail() {
        _closed = false;
        _opened = false;
        _trailRenderer.enabled = false;
        _trailRenderer.time = float.PositiveInfinity;
        _worldPoints.Clear();
        _screenPoints.Clear();
        _targets.Clear();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            RestartTrail();
            return;
        }

        if (_closed) return;

        if (_AIDrawing) {
            AIDrawTrailUpdate();
            return;
        }
        
        bool clicking = Input.GetMouseButton(0);
        Vector2 clickPoint = Input.mousePosition;

        if (clicking) {
            HandleClick(clickPoint);
        } else if (_opened) {
            CloseTrail();
        }
    }

    void HandleClick(Vector2 clickPoint) {
        Ray mRay = Camera.main.ScreenPointToRay(clickPoint);

        float rayDistance;
        if (_rayCastPlane.Raycast(mRay, out rayDistance))
        {
            Vector3 point = mRay.GetPoint(rayDistance);
            if (!_opened) {
                OpenTrail(point);
            } else {
                MoveTrailTowards(point);
            }
            
            RaycastHit hit;
            if (Physics.Raycast(mRay, out hit)){
                if (hit.transform.gameObject.CompareTag("Team 2")) {
                    AddTarget(hit.transform.gameObject);
                }
            }
        }
    }

    bool IsAcceptable() {
        return _length >= _minLength && _worldPoints.Count >= 3;
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
    public bool fast;
    public bool slow;

    void AnalyzeTrail() {
        PerformLevel0Analysis();
        
        horizontal = _yStdDev < 2.2f;
        vertical = _xStdDev < 2.2f;
        straight = _directionStdDev < 0.07f && _spikes == 0;
        circular = _spikes == 0 && (
            _directionStdDev > 0.4f && _angleStdDev < 0.17f ||
            _directionStdDev > 0.9f && _angleStdDev < 0.28f
        );
        isLong = _length > _maxLength - _minLength && _spikes == 0;
        isShort = _length < _minLength * 2.4f;
        spike = _spikes == 1;
        zigZag = _spikes > 1;
        fast = _speed > 1200;
        slow = _speed < 150;
    }

    [Header("Level 0 Analysis (private)")]
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
    
    [SerializeField] float _speed = 0;
    
    void PerformLevel0Analysis() {
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

        _speed = _length / _timeOpened;
    }
}
