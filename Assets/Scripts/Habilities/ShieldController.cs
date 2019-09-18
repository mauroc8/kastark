using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Events;
using UnityEngine.EventSystems;

public class ShieldController : HabilityController
{
    [SerializeField] RectTransform _circleTransform = null;
    [SerializeField] RectTransform _backgroundTransform = null;
    [SerializeField] float _speed = 0;

    Vector2 _unitScreenPos;
    float _minCastDistance;

    void OnEnable() {
        var unitWorldPos = GameState.actingCreature.transform.position;
        _unitScreenPos = Camera.main.WorldToScreenPoint(unitWorldPos);

        // TODO: Fix bug where shield isn't well position'd
        _backgroundTransform.position = _circleTransform.position = Util.ScreenPointToHDCoords(_unitScreenPos);
        _minCastDistance = Camera.main.pixelHeight * 0.35f;
    }

    void Update() {
        if (_cast) return;

        var time = Time.time;

        var cycle = Mathf.Sin(time * _speed);
        var scale = 1 + cycle * 0.5f;

        _circleTransform.localScale = new Vector3(scale, scale, 1);

        if (Input.GetMouseButtonDown(0) &&
            Vector2.Distance(Input.mousePosition, _unitScreenPos) < _minCastDistance &&
            !Util.MouseIsOnUI()) {
                _cast = true;
                var effectiveness = 1 - 0.6f * Mathf.Abs(cycle);
                EventController.TriggerEvent(new HabilityCastEvent(GameState.actingCreature, effectiveness));
        }
    }
}
