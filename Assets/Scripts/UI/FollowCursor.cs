using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    [SerializeField] float _attraction = 0.3f;
    [SerializeField] float _friction   = 0.1f;

    Vector2 _speed = Vector2.zero;
    
    void Update() {
        Vector2 pos    = transform.position;

        _speed *= Mathf.Pow(_friction, Time.deltaTime);
        pos += _speed;

        if (Input.GetMouseButton(0))
        {
            Vector2 cursor = Input.mousePosition;
            var diff = cursor - pos;

            _speed += Mathf.Max(0, (300 - diff.magnitude) / diff.magnitude) * diff * _attraction * Time.deltaTime;
        }

        transform.position = pos;
    }
}
