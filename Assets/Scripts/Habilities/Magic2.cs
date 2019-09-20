using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic2 : HabilityController
{
    [SerializeField] GameObject _particle = null;
    [SerializeField] GameObject _trail    = null;
 
    [SerializeField] float _duration = 4;

    [Header("Direction")]
    [SerializeField] Transform _from;
    [SerializeField] Transform _to;

    float _startTime;
    Vector3 _fromPoint;
    Vector3 _toPoint;

    void OnEnable()
    {
        _startTime = Time.time;
        _fromPoint = _from.position;
        _toPoint = _to.position;
    }

    int _particleAmount = 5;

    void Update()
    {
        if (_cast) return;

        var t = (Time.time - _startTime) / _duration;

        if (t > 1)
        {
            _cast = true;
            return;
        }
        
        int particleNumber = (int) t * _particleAmount;
        
    }
}
