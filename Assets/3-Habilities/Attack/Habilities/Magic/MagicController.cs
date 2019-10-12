using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class MagicController : HabilityController
{
    [Header("Config")]
    [SerializeField] float _castDistanceVh = 0.2f;
    [SerializeField] float _countdownTime = 0.8f;

    [Header("Refs")]
    [SerializeField] Transform _bigParticleTransform = null;
    [SerializeField] CountdownController _countdownController = null;

    float _castDistancePx;

    void OnEnable()
    {
        _castDistancePx = _castDistanceVh * Screen.height;
    }

    public bool Cast => _cast;

    float _castStartTime;

    void Start()
    {
        _castStartTime = Time.time;
        _countdownController.StartCountdown(_countdownTime);
    }

    void Update()
    {
        if (_cast) return;

        if (!_countdownController.Running)
        {
            _cast = true;
            
            EventController.TriggerEvent(new HabilityCastEvent{});
        }

        var target = RaycastHelper.GetGameObjectAtScreenPoint(_bigParticleTransform.position, LayerMask.HabilityRaycast);

        if (target != null && Global.IsFromEnemyTeam(target))
        {
            target.GetComponent<Behaviour>()?.GetsHit();
        }
    }
}
