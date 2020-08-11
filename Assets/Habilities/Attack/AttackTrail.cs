using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrail : MonoBehaviour
{
    [SerializeField] TeamId _enemyTeam;

    private bool _open;
    float _effectiveness;

    public bool IsOpen => _open;
    public float Effectiveness => _effectiveness;

    public bool PerformedCut = false;

    float _distanceToCamera = 12;

    [Header("Configuration")]
    public float minLengthVh = 0.07f;
    public float maxLengthVh = 0.37f;
    public float maxLifetime = 3.87f;

    private List<Vector2> _screenPoints = new List<Vector2>(30);
    private float _length;
    private float _trailLifetime;

    TrailRenderer _trailRenderer;
    float _minVertexDistance = 0.08f;
    float _minLengthPx;
    float _maxLengthPx;
    Plane _rayCastPlane;
    float _openTime;

    public void Open(Vector2 screenPoint)
    {
        _trailRenderer = GetComponent<TrailRenderer>();
        _trailRenderer.widthMultiplier *= 300.0f / Screen.height;

        _minLengthPx = minLengthVh * Screen.height;
        _maxLengthPx = maxLengthVh * Screen.height;

        _rayCastPlane = new Plane(
            Camera.main.transform.forward * -1,
            Camera.main.transform.position + Camera.main.transform.forward * _distanceToCamera);

        _open = true;
        _length = 0;
        _openTime = Time.time;
        _lastScreenPoint = screenPoint;

        Move(screenPoint, false);

        _trailRenderer.Clear();
        gameObject.SetActive(true);
    }

    public bool IsOutOfBounds => _length > _maxLengthPx;

    public void Close()
    {
        // Pre: IsAcceptable()
        _trailLifetime = Time.time - _openTime;
        _open = false;
        _trailRenderer.autodestruct = true;
    }

    Vector2 _lastScreenPoint;

    public void Move(Vector2 screenPoint, bool hitLifePoints = true)
    {
        Ray mRay = Camera.main.ScreenPointToRay(screenPoint);

        float rayDistance;
        if (!_rayCastPlane.Raycast(mRay, out rayDistance))
        {
            Debug.Log($"Error raycasting to plane ({rayDistance}).");
            return;
        }

        Vector3 worldPoint = mRay.GetPoint(rayDistance);

        if (hitLifePoints)
        {
            float distance = Vector3.Distance(transform.position, worldPoint);

            if (distance < _minVertexDistance)
                return;

            // Search Intermediate Targets.
            for (float f = 0; f < distance; f += _minVertexDistance)
            {
                TryHitLifePoint(
                    Vector2.Lerp(screenPoint, _lastScreenPoint, f / distance)
                );
            }
        }

        _screenPoints.Add(screenPoint);
        _length += Vector2.Distance(_lastScreenPoint, screenPoint);
        _lastScreenPoint = screenPoint;
        transform.position = worldPoint;
    }

    void TryHitLifePoint(Vector2 screenPoint)
    {
        PerformedCut = true;

        var target = RaycastHelper.SphereCastAtScreenPoint(screenPoint, LayerMask.HabilityRaycast);

        if (target != null && target.CompareTag(_enemyTeam.ToString()))
        {
            var hittable = target.GetComponent<IHittable>();

            if (hittable != null)
                hittable.Hit(this, 0);
        }
    }
}
