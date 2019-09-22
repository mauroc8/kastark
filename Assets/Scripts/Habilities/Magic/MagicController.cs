using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class MagicController : HabilityController
{
    [Header("Config")]
    [SerializeField] float _castDistanceVh = 0.2f;
    [Tooltip("Overrides bigParticle's FollowCursor settings.")]
    [SerializeField] float _attractionDistanceVh = 0.8f;
    [SerializeField] float _fullPowerSpeedVh = 0.017f;

    [Header("Refs")]
    [SerializeField] Transform _bigParticleTransform = null;
    [SerializeField] PositionNextToCreature _bigParticlePositionNextToCreature = null;
    [SerializeField] FollowCursor _bigParticleFollowCursor = null;

    float _castDistancePx;
    float _attractionDistancePx;

    void OnEnable()
    {
        _castDistancePx = _castDistanceVh * Camera.main.pixelHeight;
        _attractionDistancePx = _attractionDistanceVh * Camera.main.pixelHeight;
        _bigParticleFollowCursor.AttractionDistanceVh = _attractionDistanceVh;
    }

    bool _casting = false;
    bool _mouseIsWithinCastDistance = false;
    float _normalizedDistanceToAttractionCenter = 0;

    public bool Cast => _cast;
    public bool Casting => _casting;
    public bool MouseIsWithinCastDistance => _mouseIsWithinCastDistance;
    public float NormalizedDistanceToAttractionCenter => _normalizedDistanceToAttractionCenter;

    void Update()
    {
        if (_cast) return;

        var diff = Vector2.Distance(_bigParticleTransform.position, Input.mousePosition);
        
        _mouseIsWithinCastDistance = diff <= _castDistancePx;
        _normalizedDistanceToAttractionCenter = diff / _attractionDistancePx;

        if (!_casting)
        {
            if (_mouseIsWithinCastDistance && Input.GetMouseButtonDown(0))
            {
                _bigParticlePositionNextToCreature.updateEachFrame = false;
                _casting = true;
            }

            return;
        }

        var target = Util.GetGameObjectAtScreenPoint(_bigParticleTransform.position);

        if (GameState.IsFromEnemyTeam(target))
        {
            var speed = _bigParticleFollowCursor.Speed;
            var magnitude = speed.magnitude / Camera.main.pixelHeight / _fullPowerSpeedVh;
            _bigParticleFollowCursor.enabled = false;

            var effectiveness = magnitude > 1 ? 1 : Mathf.Pow(magnitude, difficulty);

            EventController.TriggerEvent(
                Util.NewHabilityCastEvent(target.GetComponent<CreatureController>(), effectiveness));
            
            _cast = true;
        }
    }
}
