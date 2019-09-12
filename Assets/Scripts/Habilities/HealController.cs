using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.UI;

public class HealController : MonoBehaviour
{
    [SerializeField] Transform _targetTransform = null;
    [SerializeField] Transform _cursorTransform = null;

    [SerializeField] float _speedX = 3.7f;
    [SerializeField] float _speedY = 2.6f;

    [SerializeField] float _xDisplacement = 50;
    [SerializeField] float _yDisplacement = 50;

    float _minCastDistance = 250;
    Vector2 _unitPosition;

    void Start() {
        _unitPosition = Camera.main.WorldToScreenPoint(GameState.currentUnit.transform.position);
        _minCastDistance = Camera.main.pixelHeight * 0.35f;
}

    bool  _cast = false;
    float _effectiveness;

    void Update() {
        if (_cast) return;

        var time = Time.time;

        var xCycle = Mathf.Sin(time * _speedX);
        var yCycle = Mathf.Cos(time * _speedY);

        var xDisplacement = xCycle * _xDisplacement;
        var yDisplacement = yCycle * _yDisplacement;

        _targetTransform.position = _unitPosition + new Vector2(xDisplacement, yDisplacement);

        if (!Util.MouseIsOnUI()) {
            _cursorTransform.position = Input.mousePosition;

            if (Input.GetMouseButtonDown(0)) {
                var distance = Vector2.Distance(_targetTransform.position, _cursorTransform.position);
                if (distance < _minCastDistance) {
                    _effectiveness = 1 - 0.7f * distance / _minCastDistance;
                    _effectiveness = Mathf.Lerp(
                        Mathf.Pow(_effectiveness, 0.6f),
                        Mathf.Pow(_effectiveness, 3),
                        _effectiveness
                    );
                    _cast = true;

                    EventController.TriggerEvent(new ConfirmSelectedHabilityEvent{});
                    Debug.Log(_effectiveness);
                }
            }
        } else {
            _cursorTransform.position = Vector3.down * 9999;
        }
    }
}
