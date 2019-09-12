using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Events;
using UnityEngine.EventSystems;

public class ShieldController : MonoBehaviour
{
    [SerializeField] RectTransform _circleTransform = null;
    [SerializeField] float _speed = 3;

    Vector2 _unitScreenPos;

    void OnEnable() {
        var unitPos = GameState.currentUnit.transform.position;
        var cameraPos = Camera.main.transform.position;

        transform.position = Vector3.MoveTowards(unitPos, cameraPos, 3);
        transform.LookAt(Camera.main.transform.position, Vector3.up);

        _unitScreenPos = Camera.main.WorldToScreenPoint(unitPos);
    }

    bool _cast = false;
    float _effectiveness;
    float _minCastDistance;

    void Start() {
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
                EventController.TriggerEvent(new ConfirmSelectedHabilityEvent{});
                _cast = true;
                _effectiveness = 1 - 0.7f * Mathf.Abs(cycle);

                _effectiveness = Mathf.Lerp(
                    Mathf.Pow(_effectiveness, 0.7f),
                    Mathf.Pow(_effectiveness, 4),
                    _effectiveness);

                Debug.Log(_effectiveness);
        }
    }
}
