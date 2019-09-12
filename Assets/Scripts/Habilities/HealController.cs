using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealController : MonoBehaviour
{
    [SerializeField] Transform _targetTransform = null;
    [SerializeField] Transform _cursorTransform = null;

    [SerializeField] float _speedX = 3;
    [SerializeField] float _speedY = 3;

    [SerializeField] float _xDisplacement = 50;
    [SerializeField] float _yDisplacement = 50;

    Vector2 _unitPosition;

    void Start() {
        _unitPosition = Camera.main.WorldToScreenPoint(GameState.currentUnit.transform.position);
    }

    void Update() {
        var time = Time.time;

        var xCycle = Mathf.Sin(time * _speedX);
        var yCycle = Mathf.Cos(time * _speedY);

        var xDisplacement = xCycle * _xDisplacement;
        var yDisplacement = yCycle * _yDisplacement;

        _targetTransform.position = _unitPosition + new Vector2(xDisplacement, yDisplacement);

        if (!Util.MouseIsOnUI()) {
            _cursorTransform.position = Input.mousePosition;
        } else {
            _cursorTransform.position = Vector3.down * 9999;
        }
    }
}
