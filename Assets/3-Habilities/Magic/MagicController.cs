using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class MagicController : HabilityController
{
    [Header("Config")]
    [SerializeField] float _countdownTime = 0.8f;

    [Header("Refs")]
    [SerializeField] Transform _bigParticleTransform = null;
    [SerializeField] CountdownController _countdownController = null;

    public bool Cast => _cast;

    float _castStartTime;
    Vector3 _lastPosition;

    void Start()
    {
        _castStartTime = Time.time;
        _countdownController.StartCountdown(_countdownTime);
        _lastPosition = _bigParticleTransform.position;
    }

    void Update()
    {
        if (_cast) return;

        if (!_countdownController.Running)
        {
            _cast = true;
            
            EventController.TriggerEvent(new HabilityCastEvent{});
        }

        var diff = _bigParticleTransform.position - _lastPosition;
        var diffMagnitudeVh = diff.magnitude / Screen.height;

        var diffUnit = 0.03f / diffMagnitudeVh;
        float t = diffUnit;

        while (t < 1)
        {
            
            var intermediatePoint = Vector2.Lerp(_bigParticleTransform.position, _lastPosition, t);
            var target = RaycastHelper.SphereCastAtScreenPoint(intermediatePoint, LayerMask.HabilityRaycast);
            if (target != null && Global.IsFromEnemyTeam(target))
            {
                target.GetComponent<LifePointBehaviour>()?.GetsHit();
            }

            t += diffUnit;
        }

        {
            var target = RaycastHelper.SphereCastAtScreenPoint(_bigParticleTransform.position, LayerMask.HabilityRaycast);

            if (target != null && Global.IsFromEnemyTeam(target))
            {
                target.GetComponent<LifePointBehaviour>()?.GetsHit();
            }
        }
        
        _lastPosition = _bigParticleTransform.position;
    }
}
