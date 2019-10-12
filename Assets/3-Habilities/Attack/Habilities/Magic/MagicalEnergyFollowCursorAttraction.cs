using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicalEnergyFollowCursorAttraction : MonoBehaviour
{
    [SerializeField] float _attraction = 0.4f;
    [SerializeField] float _friction   = 0.1f;
    [SerializeField] float _maxVhDistance = 0.2f;

    Vector2 _speed = Vector2.zero;
    public Vector2 Speed => _speed;

    float _maxPxDistance;

    void Start()
    {
        _maxPxDistance = _maxVhDistance * Screen.height;
    }

    void Update() {
        Vector2 pos = transform.position;
        Vector2 mousePos = Input.mousePosition;
        var dt = Time.deltaTime;

        var friction = Mathf.Pow(1 - _friction, dt);
        var attraction = _attraction * dt;
        var distance = mousePos - pos;

        if (distance.magnitude > _maxPxDistance)
        {
            distance = distance / distance.magnitude *  _maxPxDistance;
        }

        _speed += distance * attraction;
        _speed *= friction;

        pos += _speed * dt;

        transform.position = pos;
    }
}
