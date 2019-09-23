using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : HabilityController
{
    [Header("Refs")]
    [SerializeField] GameObject _shield = null;
    [SerializeField] ColorController _shieldColorController = null;
    
    [Header("Settings")]
    [SerializeField] float _speed = 1;
    [SerializeField] float _maxOpacity = 0.5f;

    void Update()
    {
        if (_cast) return;

        var t = Time.time;
        var effectiveness = GetEffectiveness(t);
        effectiveness = Mathf.Pow(effectiveness, difficulty);

        _shieldColorController.ChangeOpacity(effectiveness * _maxOpacity);

        if (Input.GetMouseButtonDown(0) && Util.GetHoveredGameObject() == _shield && !Util.MouseIsOnUI())
        {
            _cast = true;
            var habilityCastController = new HabilityCastController{
                targets = new CreatureController[]{ GameState.actingCreature },
                effectiveness = new float[]{ effectiveness },
                hability = GameState.selectedHability
            };
            habilityCastController.Cast();
        }
    }

    float GetEffectiveness(float t)
    {
        var cycle = (Time.time * _speed) % 2;
        if (cycle > 1) cycle = 2 - cycle;

        return cycle;
    }
}
