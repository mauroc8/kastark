using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicalEnergyFollowCursor : MonoBehaviour
{
    public float Speed => _speed;

    float _speed = 0;
    int _sampleCount = 0;

    void Update() {
        Vector2 pos = transform.position;

        Vector2 cursor = Input.mousePosition;
        var diff = cursor - pos;
        
        var speedSample = diff.magnitude / Time.deltaTime;

        _speed = (_speed * _sampleCount + speedSample) / ++_sampleCount;
        
        transform.position = cursor;
    }
}
