using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class MagicController : HabilityController
{
    [Header("Config")]
    [SerializeField] float _castDistanceVh = 0.2f;
    [SerializeField] float _fullPowerSpeedVh = 23f;
    [SerializeField] float _countdownTime = 0.8f;

    [Header("Refs")]
    [SerializeField] Transform _bigParticleTransform = null;
    [SerializeField] PositionNextToCreature _bigParticlePositionNextToCreature = null;
    [SerializeField] MagicalEnergyFollowCursor _bigParticleFollowCursor = null;

    float _castDistancePx;

    void OnEnable()
    {
        _castDistancePx = _castDistanceVh * Screen.height;
    }

    bool _casting = false;
    bool _mouseIsWithinCastDistance = false;

    public bool Cast => _cast;
    public bool Casting => _casting;
    public bool MouseIsWithinCastDistance => _mouseIsWithinCastDistance;
    public float CountdownTime => _countdownTime;

    float _castStartTime;

    void Update()
    {
        if (_cast) return;

        var diff = Vector2.Distance(_bigParticleTransform.position, Input.mousePosition);
        
        _mouseIsWithinCastDistance = diff <= _castDistancePx;

        if (!_casting)
        {
            if (_mouseIsWithinCastDistance && Input.GetMouseButtonDown(0))
            {
                _bigParticleFollowCursor.enabled = true;
                _bigParticlePositionNextToCreature.updateEachFrame = false;
                _castStartTime = Time.time;
                _casting = true;
            }

            return;
        }

        if (Time.time - _castStartTime >= _countdownTime)
        {
            _bigParticleFollowCursor.enabled = false;
            _casting = false;
            _cast = true;
            // Do not cast. Just send the message.
            EventController.TriggerEvent(new HabilityCastEvent{});
        }

        var target = Util.GetGameObjectAtScreenPoint(_bigParticleTransform.position);

        if (Global.IsFromEnemyTeam(target))
        {
            var speed = _bigParticleFollowCursor.Speed;
            var unadjustedEffectiveness = speed / Screen.height / _fullPowerSpeedVh;

            if (unadjustedEffectiveness > 1) unadjustedEffectiveness = 1;

            _bigParticleFollowCursor.enabled = false;
            _casting = false;
            _cast = true;

            var targetCreature = target.GetComponent<CreatureController>();
            
            Global.selectedHability.Cast(targetCreature, unadjustedEffectiveness);
            EventController.TriggerEvent(new HabilityCastEvent{});
        }
    }
}
