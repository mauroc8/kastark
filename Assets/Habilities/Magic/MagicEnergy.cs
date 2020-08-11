using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicEnergy : MonoBehaviour
{
    float _distanceToCamera = 6;

    Plane _rayCastPlane;
    Camera _camera;

    public void CastStart()
    {
        _camera =
            Camera.main;

        _rayCastPlane = new Plane(
            _camera.transform.forward * -1,
            _camera.transform.position + _camera.transform.forward * _distanceToCamera
        );
    }

    public Vector2 Position
    {
        set
        {
            Ray mRay =
                _camera.ScreenPointToRay(value);

            float rayDistance;
            if (!_rayCastPlane.Raycast(mRay, out rayDistance))
            {
                Debug.Log($"Error raycasting to plane ({rayDistance}).");
                return;
            }

            Vector3 worldPoint = mRay.GetPoint(rayDistance);
            transform.position = worldPoint;
        }
    }
}
