using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.UI;

public class MagicController : MonoBehaviour
{
    [SerializeField] RectTransform _powerTransform = null;
    [SerializeField] float _fullPowerX = 0;
    [SerializeField] float _noPowerX   = 0;

    [SerializeField] float _speed = 5;
    [SerializeField] float _effectivenessPower = 1;

    bool _cast = false;
    float _effectiveness;

    float _lastPower;
    float _lastTime;

    [SerializeField] float _xOffset = 0;

    void Update() {
        if (_cast) return;

        float currentTime = Time.time;
        float fract = currentTime * _speed;
        float _power = fract % 1;
        _effectiveness = Mathf.Pow(_power, _effectivenessPower);

        var pos = _powerTransform.localPosition;
        pos.x = _xOffset + _noPowerX + _effectiveness * (_fullPowerX - _noPowerX);
        _powerTransform.localPosition = pos;

        if (Input.GetMouseButtonDown(0) && !Util.MouseIsOnUI()) {
            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(mRay, out hit)){
                var target = hit.transform.gameObject;
                if (target.CompareTag(GameState.EnemyTeamTag)) {
                    _cast = true;
                    EventController.TriggerEvent(new HabilityCastStartEvent());
                    Debug.Log(_effectiveness);
                }
            }
        }

        _lastPower = _power;
        _lastTime = Time.time;
    }
}
