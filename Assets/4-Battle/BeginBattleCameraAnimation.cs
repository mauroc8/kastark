using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;

public class BeginBattleCameraAnimation : MonoBehaviour
{
    Transform _transform;
    Vector3 _initialPosition;
    float _startTime;

    [SerializeField] float _duration = 0;
    [SerializeField] float _displacement = 0;
    [SerializeField] float _easePower = 0;

    void Start()
    {
        _transform = GetComponent<Transform>();

        _initialPosition = _transform.position;

        _startTime = Time.time;
    }


    void Update()
    {
        var t = (Time.time - _startTime) / _duration;

        if (t < 1)
        {
            var displacement = Camera.main.transform.forward * _displacement;
            var envelope = 1 - t;
            envelope = Mathf.Pow(envelope, _easePower);
            var pos = _initialPosition - envelope * displacement;

            _transform.position = pos;
        }
        else
        {
            _transform.position = _initialPosition;
            this.enabled = false;
        }
    }
}
