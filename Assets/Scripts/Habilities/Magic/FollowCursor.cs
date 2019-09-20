using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    [SerializeField] float _attraction = 0.3f;
    [SerializeField] float _friction   = 0.99f;
    [SerializeField] float _attractionDistanceVH = 0.4f;

    Vector2 _speed = Vector2.zero;
    float _attractionDistancePX;
    
    void OnEnable()
    {
        _attractionDistancePX = Camera.main.pixelHeight * _attractionDistanceVH;
    }

    void Update() {
        Vector2 pos = transform.position;

        _speed *= Mathf.Pow(1 - _friction, Time.deltaTime);
        pos += _speed;

        if (Input.GetMouseButton(0))
        {
            Vector2 cursor = Input.mousePosition;
            var diff = cursor - pos;
            var distanceFactor = Mathf.Max(0, (_attractionDistancePX - diff.magnitude) / diff.magnitude);

            _speed += distanceFactor * diff * _attraction * Time.deltaTime;
        }

        transform.position = pos;
    }
}
