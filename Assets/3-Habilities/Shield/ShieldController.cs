using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class ShieldController : MonoBehaviour
{
    bool _cast;
    
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

        _shieldAlphaController.Alpha = effectiveness * _maxOpacity;

        if (Input.GetMouseButtonDown(0) && RaycastHelper.GetHoveredGameObject() == _shield && !RaycastHelper.MouseIsOnUI())
        {
            _cast = true;
            // TODO
            //Global.selectedHability.Cast(Global.actingCreature, effectiveness);
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
