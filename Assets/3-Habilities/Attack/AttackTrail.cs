using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrail : MonoBehaviour
{
    [SerializeField] TeamId _team;

    private bool _open;
    float _effectiveness;

    public bool IsOpen => _open;
    public float Effectiveness => _effectiveness;

    public float worstScenarioDirectionStdDev = 0.3f;

    [SerializeField] float _distanceToCamera = 4;

    [Header("Configuration")]
    public float minLengthVh = 0.07f;
    public float maxLengthVh = 0.37f;
    public float maxLifetime = 3.87f;

    private List<Vector2> _screenPoints = new List<Vector2>(30);
    private float _length;
    private float _trailLifetime;

    TrailRenderer _trailRenderer;
    float _minVertexDistance;

    float _minLengthPx;
    float _maxLengthPx;

    void Start()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
        _minVertexDistance = _trailRenderer.minVertexDistance;
        _trailRenderer.widthMultiplier *= 300.0f / Screen.height;

        _minLengthPx = minLengthVh * Screen.height;
        _maxLengthPx = maxLengthVh * Screen.height;
    }

    Plane _rayCastPlane;
    float _openTime;

    public void Open(Vector2 screenPoint) {
        _rayCastPlane = new Plane(
            Camera.main.transform.forward * -1,
            Camera.main.transform.position + Camera.main.transform.forward * _distanceToCamera);
        
        _open = true;
        _length = 0;
        _openTime = Time.time;
        _lastScreenPoint = screenPoint;

        Move(screenPoint);

        _trailRenderer.Clear();
    }


    public bool IsOutOfBounds()
    {
        // Trail is too long or too slow.
        if (_length > _maxLengthPx) {
            return true;
        }
        return false;
    }

    public void Close() {
        // Pre: IsAcceptable()
        _trailLifetime = Time.time - _openTime;
        _open = false;
        _trailRenderer.autodestruct = true;
    }

    Vector2 _lastScreenPoint;

    public void Move(Vector2 screenPoint) {
        Ray mRay = Camera.main.ScreenPointToRay(screenPoint);

        float rayDistance;
        if (!_rayCastPlane.Raycast(mRay, out rayDistance))
        {
            Debug.Log($"Error raycasting to plane ({rayDistance}).");
            return;
        }
        Debug.Log("Success raycasting to plane.");
    
        Vector3 worldPoint = mRay.GetPoint(rayDistance);
        float distance = Vector3.Distance(transform.position, worldPoint);

        if (distance < _minVertexDistance)
            return;

        var target = RaycastHelper.SphereCastAtScreenPoint(Input.mousePosition, LayerMask.HabilityRaycast);

        if (target != null && target.CompareTag(_team.ToString()))
        {
            HitLifePoint(target);
        }
        else
        {
            // Search Intermediate Targets.
            var vertexUnit = _minVertexDistance / distance; // 0 < vertexUnit <= 1
            float t = vertexUnit;
            while (t < 1)
            {
                var intermediatePoint = Vector2.Lerp(screenPoint, _lastScreenPoint, t);
                target = RaycastHelper.SphereCastAtScreenPoint(intermediatePoint, LayerMask.HabilityRaycast);
                if (target != null && target.CompareTag(_team.ToString()))
                    HitLifePoint(target);
                t += vertexUnit;
            }
        }

        float screenDistance = Vector2.Distance(screenPoint, _lastScreenPoint);
        _length += screenDistance;

        _screenPoints.Add(screenPoint);
        _lastScreenPoint = screenPoint;
        transform.position = worldPoint;
    }

    void HitLifePoint(GameObject target)
    {
        var lifePointController = target.GetComponent<LifePointController>();
        lifePointController?.GetsHit();
    }
}
