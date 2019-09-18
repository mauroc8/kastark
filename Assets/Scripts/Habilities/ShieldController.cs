using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Events;
using UnityEngine.EventSystems;

public class ShieldController : HabilityController
{
    [SerializeField] GameObject _circle = null;
    [SerializeField] RectTransform _backgroundTransform = null;
    [SerializeField] float _duration = 0;

    RectTransform _circleTransform = null;
    CanvasGroup   _circleGroup     = null;

    Vector2 _unitScreenPos;
    float _minCastDistance;

    void OnEnable() {
        var unitWorldPos = GameState.actingCreature.transform.position;
        _unitScreenPos = Camera.main.WorldToScreenPoint(unitWorldPos);

        _circleTransform = _circle.GetComponent<RectTransform>();
        _circleGroup     = _circle.GetComponent<CanvasGroup>();
        
        // TODO: Fix bug where shield isn't well position'd
        _backgroundTransform.position = _circleTransform.position = _unitScreenPos;
        _minCastDistance = Camera.main.pixelHeight * 0.35f;
    }

    void Update() {
        if (_cast) return;

        var time = Time.time;

        var effectiveness = Mathf.Abs(1 - 2 * ((time / _duration) % 1));
        effectiveness = Mathf.Pow(effectiveness, difficulty);

        _circleTransform.localScale = new Vector3(effectiveness, effectiveness, 1);
        _circleGroup.alpha = effectiveness;

        if (Input.GetMouseButtonDown(0) &&
            Vector2.Distance(Input.mousePosition, _unitScreenPos) < _minCastDistance &&
            !Util.MouseIsOnUI()) {
                _cast = true;
                EventController.TriggerEvent(new HabilityCastEvent(GameState.actingCreature, effectiveness));
        }
    }
}
