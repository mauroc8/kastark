using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicEnergy : MonoBehaviour
{
    [SerializeField] float _attraction = 0.4f;
    [SerializeField] float _friction = 0.1f;
    [SerializeField] float _maxVhDistance = 0.2f;
    float _maxPxDistance;

    Vector2 _speed = Vector2.zero;
    public Vector2 Speed => _speed;

    [SerializeField] float _distanceToCamera = 6;
    Plane _rayCastPlane;

    public void Open(Vector2 screenPosition)
    {
        _maxPxDistance = _maxVhDistance * Screen.height;
        _rayCastPlane = new Plane(
            Camera.main.transform.forward * -1,
            Camera.main.transform.position + Camera.main.transform.forward * _distanceToCamera);

        Position = screenPosition;
        //Move(screenPosition);
    }

    [SerializeField] float _minTargetSeekDistanceVh = 0.1f;
    [SerializeField] TeamId _enemyTeam;

    public void Move(Vector2 screenPosition)
    {
        var distance = screenPosition - Position;

        if (distance.magnitude > _maxPxDistance)
            distance = distance / distance.magnitude * _maxPxDistance;

        var dt = Time.deltaTime;
        var friction = Mathf.Pow(1 - _friction, dt);
        var attraction = _attraction * dt;

        _speed += distance * attraction;
        _speed *= friction;

        Position += _speed * dt;

        for (float f = 0; f < 1; f += _minTargetSeekDistanceVh * Screen.height)
        {
            var target = RaycastHelper.SphereCastAtScreenPoint(Position, LayerMask.HabilityRaycast);

            if (target != null && target.CompareTag(_enemyTeam.ToString()))
            {
                var lifePoint = target.GetComponent<LifePointController>();

                if (lifePoint != null)
                    lifePoint.Hit();
            }
        }
    }

    Vector2 _position;

    public Vector2 Position
    {
        set
        {
            Ray mRay = Camera.main.ScreenPointToRay(value);

            float rayDistance;
            if (!_rayCastPlane.Raycast(mRay, out rayDistance))
            {
                Debug.Log($"Error raycasting to plane ({rayDistance}).");
                return;
            }

            Vector3 worldPoint = mRay.GetPoint(rayDistance);
            transform.position = worldPoint;

            _position = value;
        }
        get
        {
            // Importante! Llamar set antes que get.
            return _position;
        }
    }
}
