using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Events;
using UnityEngine.EventSystems;

public class ShieldController : HabilityController
{
    [Header("Shield settings")]
    [SerializeField] float _duration = 0;
    [SerializeField] float _maxCastDistanceVh = 0.35f;

    [Header("Refs")]
    [SerializeField] RectTransform   _circleTransform = null;
    [SerializeField] ColorController     _circleColor = null;
    [SerializeField] ColorController _backgroundColor = null;

    [Header("View settings")]
    [SerializeField] float _changeCircleColorThreshold = 0.9f;
    [SerializeField] Color _alternativeCircleColor = Color.yellow;

    Vector2 _unitScreenPos;
    float _maxCastDistancePx;
    Color _defaultCircleColor;

    void OnEnable() {
        var unitWorldPos = GameState.actingCreature.transform.position;
        _unitScreenPos = Camera.main.WorldToScreenPoint(unitWorldPos);

        _maxCastDistancePx = Camera.main.pixelHeight * _maxCastDistanceVh;

        _defaultCircleColor = _circleColor.GetColor();
    }

    void Update() {
        if (_cast) return;

        var time = Time.time;

        var scale = 0.5f + Mathf.Abs(1 - 2 * ((time / _duration) % 1));
        var effectiveness = Mathf.Pow(Mathf.Abs(1 - scale), difficulty);

        _circleTransform.localScale = new Vector3(scale, scale, 1);

        _backgroundColor.ChangeOpacity(effectiveness);

        if (effectiveness >= _changeCircleColorThreshold)
            _circleColor.ChangeColor(_alternativeCircleColor);
        else
            _circleColor.ChangeColor(_defaultCircleColor);

        if (Input.GetMouseButtonDown(0) &&
            Vector2.Distance(Input.mousePosition, _unitScreenPos) < _maxCastDistancePx &&
            !Util.MouseIsOnUI()) {
                _cast = true;
                EventController.TriggerEvent(new HabilityCastEvent(GameState.actingCreature, effectiveness));
        }
    }
}
