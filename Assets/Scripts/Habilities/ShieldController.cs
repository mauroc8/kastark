using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Events;
using UnityEngine.EventSystems;

public class ShieldController : MonoBehaviour
{
    [SerializeField] RectTransform _circleTransform = null;
    [SerializeField] RectTransform _backgroundTransform = null;
    [SerializeField] float _speed = 0;
    [SerializeField] float _effectivenessPower = 0;

    Vector2 _unitScreenPos;

    void OnEnable() {
        var unitWorldPos = GameState.actingUnit.transform.position;
        _unitScreenPos = Camera.main.WorldToScreenPoint(unitWorldPos);

        _backgroundTransform.position = _circleTransform.position = Util.ScreenPointToHDCoords(_unitScreenPos);
        _minCastDistance = Camera.main.pixelHeight * 0.35f;
    }

    bool _cast = false;
    float _effectiveness;
    float _minCastDistance;

    void Update() {
        if (_cast) return;

        var time = Time.time;

        var cycle = Mathf.Sin(time * _speed);
        var scale = 1 + cycle * 0.5f;

        _circleTransform.localScale = new Vector3(scale, scale, 1);

        if (Input.GetMouseButtonDown(0) &&
            Vector2.Distance(Input.mousePosition, _unitScreenPos) < _minCastDistance &&
            !Util.MouseIsOnUI()) {
                EventController.TriggerEvent(new HabilityCastStartEvent{});
                _cast = true;
                _effectiveness = 1 - 0.6f * Mathf.Abs(cycle);
                _effectiveness = Mathf.Pow(_effectiveness, _effectivenessPower);

                Debug.Log(_effectiveness);
        }
    }
}
