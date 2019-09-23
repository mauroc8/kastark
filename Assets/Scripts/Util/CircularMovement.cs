using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularMovement : MonoBehaviour
{
    [Header("Movement values")]
    [SerializeField] Vector3 _speed = Vector3.zero;
    [SerializeField] Vector3 _displacement = Vector3.zero;

    [Header("Preserve static position")]
    [Tooltip("If true, the initial position will be cached and used as reference.")]
    [SerializeField] bool _isStaticPositionedElement = true;
    [Tooltip("Controlls weather displacement values are in Unity units or in Vh.")]
    [SerializeField] bool _useViewportHeight = false;

    public Vector3 Speed => _speed;
    public Vector3 Displacement => _displacement;

    Vector3 _initialPosition;
    float   _lastTime;
    Vector3 _lastDisplacement;

    void OnEnable()
    {
        _initialPosition = transform.position;
        _lastTime = Time.time;
        _lastDisplacement = GetDisplacementAtTime(_lastTime);

        if (_useViewportHeight)
            _displacement *= Screen.height;
    }

    Vector3 GetDisplacementAtTime(float t)
    {
        return new Vector3(
            Mathf.Cos(t * _speed.x) * _displacement.x,
            Mathf.Sin(t * _speed.y) * _displacement.y,
            -Mathf.Cos(t * _speed.z) * _displacement.z
        );
    }

    void Update()
    {
        var pos = transform.position;
        var displacement = GetDisplacementAtTime(Time.time);

        if (_isStaticPositionedElement)
        {
            pos = _initialPosition + displacement;
        }
        else
        {
            pos += displacement - _lastDisplacement;
            _lastDisplacement = displacement;
        }

        transform.position = pos;
    }
}
