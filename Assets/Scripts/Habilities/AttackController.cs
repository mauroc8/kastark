using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class AttackController : MonoBehaviour
{
    [Header("Trail configuration")]
    [SerializeField] float _minLengthPx = 50;
    [SerializeField] float _maxLengthPx = 500;
    [SerializeField] float _distanceToCamera = 4;
    [SerializeField] float _maxLifetime = 2.3f;

    public TrailAnalysis trailAnalysis;

    bool _opened = false;
    bool _closed = false;

    float _trailLength;

    Plane _rayCastPlane;
    TrailRenderer _trailRenderer = null;
    float _minVertexDistance;
    float _trailExtraLifetime;
    //Material _material;

    void Start()
    {
        _rayCastPlane = new Plane(
            Camera.main.transform.forward * -1,
            Camera.main.transform.position + Camera.main.transform.forward * _distanceToCamera);
        _trailRenderer = GetComponent<TrailRenderer>();
        _trailRenderer.enabled = false;
        _minVertexDistance = _trailRenderer.minVertexDistance;

        //_material = _trailRenderer.materials[0];

        _trailExtraLifetime = _trailRenderer.time;
        _trailRenderer.time = float.PositiveInfinity;
    }

    List<Vector2> _screenPoints = new List<Vector2>(50);
    List<GameObject> _targets = new List<GameObject>(50);

    float _openTime;
    float _trailLifetime;

    void OpenTrail(Vector3 worldPoint) {
        _opened = true;
        _trailLength = 0;
        _openTime = Time.time;

        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPoint);

        EventController.TriggerEvent(new ConfirmSelectedHabilityEvent());

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

        // Trail is too Long.
        if (_trailLength + screenDistance > _maxLengthPx) {
            CloseTrail();
            return;
        }

        if (Time.time - _openTime > _maxLifetime) {
            CloseTrail();
            return;
        }

        _trailLength += screenDistance;

        AddPoint(worldPoint, screenPoint);
    }

    void AddPoint(Vector3 point, Vector2 screenPoint) {
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
        _trailLifetime = Time.time - _openTime;

        if (!IsAcceptable()) {
            // Acá llamaríamos a una referencia de UI_Fx o algo así.
            Debug.Log("Retry!");
            RestartTrail();
        } else {
            _trailRenderer.time = _trailLifetime + _trailExtraLifetime;

            var trailAnalyzer = new TrailAnalyzer(
                _screenPoints.ToArray(),
                _trailLength,
                _minLengthPx,
                _maxLengthPx,
                _trailLifetime
            );

            trailAnalysis = trailAnalyzer.AnalyzeTrail();
        }
    }

    void RestartTrail() {
        _closed = false;
        _opened = false;
        _trailRenderer.enabled = false;
        _trailRenderer.time = float.PositiveInfinity;
        _screenPoints.Clear();
        _targets.Clear();
    }
    
    void Update()
    {
        if (Debug.isDebugBuild && Input.GetKeyDown(KeyCode.Home)) {
            RestartTrail();
            return;
        }

        if (_closed) return;

        bool clicking = Input.GetMouseButton(0);
        Vector2 clickPoint = Input.mousePosition;

        if (clicking) {
            if (!_opened && Util.MouseIsOnUI()) {
                return;
            }

            HandleMouseInput(clickPoint);
        } else if (_opened) {
            CloseTrail();
        }
    }

    // Esto está separado para poder simular un click de la AI.
    void HandleMouseInput(Vector2 clickPoint) {
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
                if (hit.transform.gameObject.CompareTag(GameState.EnemyTeamTag)) {
                    AddTarget(hit.transform.gameObject);
                }
            }
        }
    }

    bool IsAcceptable() {
        return _trailLength >= _minLengthPx && _screenPoints.Count >= 3;
    }
}
