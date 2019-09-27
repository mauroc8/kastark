using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowGameObject : MonoBehaviour
{
    [Tooltip("Leave null to use acting creature's chest transform.")]
    [SerializeField] Transform _followMe = null;
    [SerializeField] float _attraction = 2;

    void Start()
    {
        if (_followMe == null)
            _followMe = Global.actingCreature.chest;
    }

    void Update()
    {
        var diff = _followMe.position - transform.position;
        var adjustedAttraction = Mathf.Min(1, _attraction * Time.deltaTime);
        
        transform.position += diff * adjustedAttraction;
    }
}
