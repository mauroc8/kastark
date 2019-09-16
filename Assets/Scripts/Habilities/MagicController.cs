using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.UI;

public class MagicController : MonoBehaviour
{
    [SerializeField] Image _fullBarImage = null;

    [SerializeField] float _speed = 5;

    float _borderPercentage = 0.013f; // The Full Bar Image has 5px of border that shouldn't be considered.

    bool _hit = false;
    float _power;

    float _lastPower;
    float _lastTime;

    void Update() {
        if (_hit) return;

        var currentTime = Time.time;
        _power = 1 - Mathf.Abs(Mathf.Sin(currentTime * _speed));
        _fullBarImage.fillAmount = _borderPercentage + _power * (1 - 2 * _borderPercentage);

        if (Input.GetMouseButtonDown(0) && !Util.MouseIsOnUI()) {
            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(mRay, out hit)){
                var target = hit.transform.gameObject;
                if (target.CompareTag(GameState.Instance.EnemyTeamTag)) {
                    // We want to calculate the actual power as the max of the powerPercentage function between
                    // _lastTime and currentTime.
                    var maxPowerMult = Mathf.PI / _speed;

                    var lastPowerPhase = _lastTime / maxPowerMult;
                    var currentPowerPhase = currentTime / maxPowerMult;

                    // If fract(powerPhase) < 0.5 then power is growing
                    if (lastPowerPhase % 1 < 0.5) {
                        if (currentPowerPhase % 1 >= 0.5) { // it was growing and now its decreasing: we reached the summit in this frame!
                            _power = 1;
                        }
                    } else { // Power is decreasing
                        _power = (_power + _lastPower) / 2;
                    }

                    _hit = true;
                    EventController.TriggerEvent(new HabilityCastStartEvent());
                }
            }

            _fullBarImage.fillAmount = _borderPercentage + _power * (1 - 2 * _borderPercentage);
        }

        _lastPower = _power;
        _lastTime = Time.time;
    }
}
