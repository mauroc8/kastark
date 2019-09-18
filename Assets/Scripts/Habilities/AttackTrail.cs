using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrail : MonoBehaviour
{
    private bool _open;
    public bool IsOpen => _open;


    public float worstScenarioDirectionStdDev = 0.3f;

    [SerializeField] float _distanceToCamera = 4;

    [Header("Configuration")]
    public float minLengthVH = 0.1f;
    public float maxLengthVH = 0.3f;
    public float maxLifetime = 2.3f;

    private List<Vector2> _screenPoints = new List<Vector2>(50);
    private List<Creature> _targetCreatures = new List<Creature>(50);
    private float _lengthVH;
    private float _trailLifetime;

    TrailRenderer _trailRenderer;
    float _trailExtraLifetime;
    float _minVertexDistance;

    void Start()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
        _minVertexDistance = _trailRenderer.minVertexDistance;
        _trailExtraLifetime = _trailRenderer.time;
        _trailRenderer.time = float.PositiveInfinity;
        _trailRenderer.widthMultiplier *= (float) 300 / Camera.main.pixelHeight; // adjust over screen height
    }

    Plane _rayCastPlane;
    float _openTime;

    public void Open(Vector2 screenPoint) {
        _rayCastPlane = new Plane(
            Camera.main.transform.forward * -1,
            Camera.main.transform.position + Camera.main.transform.forward * _distanceToCamera);
        
        _open = true;
        _lengthVH = 0;
        _openTime = Time.time;

        AddPoint(screenPoint);

        _trailRenderer.Clear();
    }

    Vector2 _lastScreenPoint;

    public void Move(Vector2 screenPoint)
    {
        float screenDistancePX = (screenPoint - _lastScreenPoint).magnitude;
        float screenDistanceVH = screenDistancePX / Camera.main.pixelHeight;

        _lengthVH += screenDistanceVH;

        AddPoint(screenPoint);
    }

    public bool IsOutOfBounds()
    {
        // Trail is too long or too slow.
        if (_lengthVH > maxLengthVH || Time.time - _openTime > maxLifetime) {
            return false;
        }
        return true;
    }

    public void Close() {
        _trailLifetime = Time.time - _openTime;

        if (!IsAcceptable()) {
            return;
        }

        _open = false;

        _trailRenderer.time = _trailLifetime + _trailExtraLifetime;

        TrailAnalysis trailAnalyzer = new TrailAnalysis(_screenPoints.ToArray());
        InterpretAnalysisResult(trailAnalyzer);
    }

    float _effectiveness;

    void InterpretAnalysisResult(TrailAnalysis trailAnalysis) {
        var stdDev = trailAnalysis.DirectionStdDev;
        _effectiveness = 1 - stdDev / worstScenarioDirectionStdDev;
        if (_effectiveness < 0) _effectiveness = 0;
    }

    public void Restart() {
        _trailRenderer.time = float.PositiveInfinity;
        _screenPoints.Clear();
        _targetCreatures.Clear();
    }

    void AddPoint(Vector2 screenPoint) {
        Ray mRay = Camera.main.ScreenPointToRay(screenPoint);

        float rayDistance;
        if (_rayCastPlane.Raycast(mRay, out rayDistance))
        {
            Vector3 worldPoint = mRay.GetPoint(rayDistance);

            if (Vector3.Distance(transform.position, worldPoint) < _minVertexDistance) {
                return;
            }

            _screenPoints.Add(screenPoint);
            _lastScreenPoint = screenPoint;
            transform.position = worldPoint;

            RaycastHit hit;
            if (Physics.Raycast(mRay, out hit)){
                var target = hit.transform.gameObject;
                if (!GameState.IsFromActingTeam(target)) {
                    var creature = target.GetComponent<Creature>();

                    if (creature && !_targetCreatures.Contains(creature))
                        _targetCreatures.Add(creature);
                }
            }
        }
    }

    bool IsAcceptable() {
        return _lengthVH >= minLengthVH && _screenPoints.Count >= 3;
    }

    public Creature[] GetTargets() {
        return _targetCreatures.ToArray();
    }

    public float[] GetEffectiveness(float difficulty) {
        var effectivenessArray = new float[_targetCreatures.Count];

        var adjusted = Mathf.Pow(_effectiveness, difficulty);

        for (int i = 0; i < effectivenessArray.Length; i++) {
            effectivenessArray[i] = adjusted;
        }
        
        return effectivenessArray;
    }
}
