using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCanvasObject : MonoBehaviour
{
    public Transform depthReference = null;

    [SerializeField] RectTransform _followMe = null;

    Plane _rayCastPlane;
    
    void Start()
    {
        _rayCastPlane = new Plane(
            Camera.main.transform.forward * -1,
            depthReference.position);
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
