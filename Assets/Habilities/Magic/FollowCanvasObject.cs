using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCanvasObject : MonoBehaviour
{
    [SerializeField] float _distanceToCamera = 4;
    [SerializeField] RectTransform _followMe = null;

    Plane _rayCastPlane;

    void Start()
    {

        _rayCastPlane = new Plane(
            Camera.main.transform.forward * -1, Camera.main.transform.position + Camera.main.transform.forward * _distanceToCamera);
    }

    void Update()
    {
        Ray mRay = Camera.main.ScreenPointToRay(_followMe.position);
        float rayDistance;
        if (_rayCastPlane.Raycast(mRay, out rayDistance))
        {
            transform.position = mRay.GetPoint(rayDistance);
        }
    }
}
