using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    [SerializeField] float _attraction = 0.3f;
    [SerializeField] float _friction   = 0.99f;
    [SerializeField] float _attractionDistanceVh = 0.4f;

    public float AttractionDistanceVh {
        get => _attractionDistanceVh;
        set {
            _attractionDistanceVh = value;
            _attractionDistancePx = Screen.height * _attractionDistanceVh;
        }
    }

    Vector2 _speed = Vector2.zero;
    float _attractionDistancePx;

    public Vector2 Speed => _speed;
    
    void OnEnable()
    {
        _attractionDistancePx = Screen.height * _attractionDistanceVh;
    }

    void Update() {
        Vector2 pos = transform.position;

        _speed *= Mathf.Pow(1 - _friction, Time.deltaTime);
        pos += _speed * Time.deltaTime;

        if (Input.GetMouseButton(0))
        {
            Vector2 cursor = Input.mousePosition;
            var diff = cursor - pos;
            var distanceFactor = Mathf.Max(0, (_attractionDistancePx - diff.magnitude) / diff.magnitude);

            _speed += distanceFactor * diff * _attraction * Time.deltaTime;
        }

        transform.position = pos;
    }
}
