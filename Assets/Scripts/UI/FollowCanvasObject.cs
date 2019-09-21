using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCanvasObject : MonoBehaviour
{
    [Tooltip("If null, the current active creature will be used.")]
    [SerializeField] Transform _depthReference = null;
    [SerializeField] RectTransform _followMe = null;

    Plane _rayCastPlane;

    void Start()
    {
        if (_depthReference == null) _depthReference = GameState.actingCreature.transform;

        _rayCastPlane = new Plane(
            Camera.main.transform.forward * -1, _depthReference.position);
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
