using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class ShieldController : HabilityController
{
    [Header("Refs")]
    [SerializeField] GameObject _shield = null;
    [SerializeField] AlphaController _shieldAlphaController = null;
    
    [Header("Settings")]
    [SerializeField] float _speed = 1;
    [SerializeField] float _maxOpacity = 0.5f;

    void Update()
    {
        if (_cast) return;

        var t = Time.time;
        var effectiveness = GetEffectiveness(t);

        _shieldAlphaController.ChangeAlpha(effectiveness * _maxOpacity);

        if (Input.GetMouseButtonDown(0) && Util.GetHoveredGameObject() == _shield && !Util.MouseIsOnUI())
        {
            _cast = true;
            GameState.selectedHability.Cast(GameState.actingCreature, effectiveness);
            EventController.TriggerEvent(new HabilityCastEvent{});
        }
    }

    float GetEffectiveness(float t)
    {
        var cycle = (Time.time * _speed) % 2;
        if (cycle > 1) cycle = 2 - cycle;

        return cycle;
    }
}
