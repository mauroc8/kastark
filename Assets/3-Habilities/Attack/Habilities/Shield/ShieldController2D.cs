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

        _unitScreenPos = Camera.main.WorldToScreenPoint(Global.actingCreature.chest.position);
        
        var unadjustedEffectiveness = GetEffectiveness(Time.time);
        
        _effectiveness = Mathf.Pow(unadjustedEffectiveness, Difficulty);

        var scale = 0.5f + 0.5f * _effectiveness;

        _circleTransform.localScale = new Vector3(scale, scale, 1);

        _hovering = Vector2.Distance(Input.mousePosition, _unitScreenPos) < _maxCastDistancePx;

        if (Input.GetMouseButtonDown(0) && _hovering && !RaycastHelper.MouseIsOnUI())
        {
            _cast = true;
            Global.selectedHability.Cast(Global.actingCreature, unadjustedEffectiveness);
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
