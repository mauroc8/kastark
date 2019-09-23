using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Events;
using UnityEngine.EventSystems;

public class ShieldControllerOld : HabilityController
{
    [Header("Shield settings")]
    [SerializeField] float _duration = 0;
    [SerializeField] float _maxCastDistanceVh = 0.35f;

    [Header("Refs")]
    [SerializeField] RectTransform   _circleTransform = null;
    [SerializeField] ColorController     _circleColor = null;
    //[SerializeField] ColorController _backgroundColor = null;

    [Header("View settings")]
    [SerializeField] float _changeCircleColorThreshold = 0.9f;
    [SerializeField] float _colorBlendThreshold = 0.08f;
    [SerializeField] Color _alternativeCircleColor = Color.yellow;

    Vector2 _unitScreenPos;
    float _maxCastDistancePx;
    Color _defaultCircleColor;

    void Start() {
        var unitWorldPos = GameState.actingCreature.transform.position;
        _unitScreenPos = Camera.main.WorldToScreenPoint(unitWorldPos);
        _maxCastDistancePx = Screen.height * _maxCastDistanceVh;
        _defaultCircleColor = _circleColor.GetColor();
    }

    void Update() {
        if (_cast) return;

        var time = Time.time;

        var scale = 0.5f + Mathf.Abs(1 - 2 * ((time / _duration) % 1));
        var effectiveness = Mathf.Pow(1 - 2 * Mathf.Abs(1 - scale), difficulty);
        var opacity = 0.3f + 0.7f * effectiveness;

        _circleTransform.localScale = new Vector3(scale, scale, 1);

        //_backgroundColor.ChangeOpacity(opacity);

        if (effectiveness >= _changeCircleColorThreshold)
            _circleColor.ChangeColor(_alternativeCircleColor);
        else
        {
            var diff = _changeCircleColorThreshold - effectiveness;
            Color color;
            if (diff <= _colorBlendThreshold)
                color = Color.Lerp(_alternativeCircleColor, _defaultCircleColor, diff / _colorBlendThreshold);
            else
                _circleColor.ChangeColor(_defaultCircleColor);
        }
        
        _circleColor.ChangeOpacity(opacity);

        if (Input.GetMouseButtonDown(0) &&
            Vector2.Distance(Input.mousePosition, _unitScreenPos) < _maxCastDistancePx &&
            !Util.MouseIsOnUI()) {
                _cast = true;
                var habilityCastController = new HabilityCastController{
                    targets = new CreatureController[]{ GameState.actingCreature },
                    effectiveness = new float[]{ effectiveness },
                    hability = GameState.selectedHability
                };
                habilityCastController.Cast();
        }
    }
}
