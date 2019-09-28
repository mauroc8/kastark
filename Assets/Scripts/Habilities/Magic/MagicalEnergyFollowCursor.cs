using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicalEnergyFollowCursor : MonoBehaviour
{
    [SerializeField] float _accelerationVh = 0.3f;
    [SerializeField] float _friction   = 0.01f;

    Vector2 _speed = Vector2.zero;
    public Vector2 Speed => _speed;

    float _accelerationPx;

    void Start()
    {
        _accelerationPx = _accelerationVh * Screen.height;
    }

    void Update() {
        Vector2 pos = transform.position;

        if (Input.GetMouseButton(0))
        {
            Vector2 cursor = Input.mousePosition;
            var diff = cursor - pos;
            
            _speed += (Vector2)Vector3.Normalize(diff) * _accelerationPx * Time.deltaTime;
        }

        _speed *= Mathf.Pow(1 - _friction, Time.deltaTime);
        pos += _speed * Time.deltaTime;

        transform.position = pos;
    }
}
