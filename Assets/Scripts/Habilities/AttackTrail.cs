using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrail : MonoBehaviour
{
    TrailRenderer _trailRenderer;

    public float worstScenarioDirectionStdDev = 0.3f;

    [SerializeField] float _distanceToCamera = 4;


    [Header("Configuration")]
    public float minLengthVH = 0.1f;
    public float maxLengthVH = 0.3f;
    public float maxLifetime = 2.3f;

    [Header("Info for the Analyzer")]
    public List<Vector2> screenPoints = new List<Vector2>(50);
    public List<Creature> targets = new List<Creature>(50);
    public float lengthVH;
    public float trailLifetime;

    float _effectiveness;
    public float Effectiveness => _effectiveness;

    float _minVertexDistance;
    float _openTime;
    float _trailExtraLifetime;

    void Start()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
        _minVertexDistance = _trailRenderer.minVertexDistance;
        _trailExtraLifetime = _trailRenderer.time;
        _trailRenderer.time = float.PositiveInfinity;
        _trailRenderer.widthMultiplier *= (float) 300 / Camera.main.pixelHeight; // adjust over screen height
    }

    Plane _rayCastPlane;

    public void Open(Vector2 screenPoint) {
        _rayCastPlane = new Plane(
            Camera.main.transform.forward * -1,
            Camera.main.transform.position + Camera.main.transform.forward * _distanceToCamera);
        
        lengthVH = 0;
        _openTime = Time.time;

        AddPoint(screenPoint);

        _trailRenderer.Clear();
    }

    Vector2 _lastScreenPoint;

    public bool Move(Vector2 screenPoint) {

        float screenDistancePX = (screenPoint - _lastScreenPoint).magnitude;
        float screenDistanceVH = screenDistancePX / Camera.main.pixelHeight;

        // Trail is too long or too slow.
        if (lengthVH + screenDistanceVH > maxLengthVH ||
            Time.time - _openTime > maxLifetime) {
            return false;
        }

        lengthVH += screenDistanceVH;

        AddPoint(screenPoint);

        return true;
    }

    public bool Close() {
        trailLifetime = Time.time - _openTime;

        if (!IsAcceptable()) {
            return false;
        } else {
            _trailRenderer.time = trailLifetime + _trailExtraLifetime;
            TrailAnalysis trailAnalyzer = new TrailAnalysis(screenPoints.ToArray());
            _effectiveness = InterpretAnalysisResult(trailAnalyzer);
            return true;
        }
    }

    float InterpretAnalysisResult(TrailAnalysis trailAnalysis) {
        var stdDev = trailAnalysis.DirectionStdDev;
        var effectiveness = 1 - stdDev / worstScenarioDirectionStdDev;
        if (effectiveness < 0) effectiveness = 0;

        return effectiveness;
    }

    public void Restart() {
        _trailRenderer.time = float.PositiveInfinity;
        screenPoints.Clear();
        targets.Clear();
    }

    void AddPoint(Vector2 screenPoint) {
        Ray mRay = Camera.main.ScreenPointToRay(screenPoint);

        float rayDistance;
        if (_rayCastPlane.Raycast(mRay, out rayDistance))
        {
            Vector3 worldPoint = mRay.GetPoint(rayDistance);

            float worldDistance = (transform.position - worldPoint).magnitude;

            if (worldDistance < _minVertexDistance) {
                return;
            }

            screenPoints.Add(screenPoint);
            _lastScreenPoint = screenPoint;
            transform.position = worldPoint;

            RaycastHit hit;
            if (Physics.Raycast(mRay, out hit)){
                if (hit.transform.gameObject.CompareTag(GameState.EnemyTeamTag)) {
                    var target = hit.transform.gameObject.GetComponent<Creature>();
                    Debug.Assert(target);
                    if (!targets.Contains(target))
                        targets.Add(target);
                }
            }
        }
    }

    bool IsAcceptable() {
        return lengthVH >= minLengthVH && screenPoints.Count >= 3;
    }

    public Creature[] GetTargets() {
        return targets.ToArray();
    }

    public float[] GetEffectiveness() {
        var effectiveness = new float[targets.Count];

        for (int i = 0; i < effectiveness.Length; i++) {
            effectiveness[i] = _effectiveness;
        }
        
        return effectiveness;
    }
}
