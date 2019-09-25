using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Events;
using UnityEngine.EventSystems;

public class ShieldController2D : HabilityController
{

    [Header("Refs")]
    [SerializeField] RectTransform   _circleTransform = null;

    [Header("Animation")]
    [SerializeField] float _speed = 1;

    [Header("Cast")]
    [SerializeField] float _maxCastDistanceVh = 0.1f;

    float _effectiveness;
    bool _hovering;

    public float Effectiveness => _effectiveness;
    public bool Hovering => _hovering;
    public bool Cast => _cast;

    Vector2 _unitScreenPos;
    float _maxCastDistancePx;

    void Start() {
        _maxCastDistancePx = Screen.height * _maxCastDistanceVh;
    }

    void Update() {
        if (_cast) return;

        _unitScreenPos = Camera.main.WorldToScreenPoint(GameState.actingCreature.chest.position);
        
        _effectiveness = GetEffectiveness(Time.time);
        
        _effectiveness = Mathf.Pow(_effectiveness, difficulty);

        var scale         = 0.5f + 0.5f * _effectiveness;

        _circleTransform.localScale = new Vector3(scale, scale, 1);

        _hovering = Vector2.Distance(Input.mousePosition, _unitScreenPos) < _maxCastDistancePx;

        if (Input.GetMouseButtonDown(0) && _hovering && !Util.MouseIsOnUI())
        {
            _cast = true;
            var habilityCastController = new HabilityCastController{
                targets = new CreatureController[]{ GameState.actingCreature },
                effectiveness = new float[]{ _effectiveness },
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
