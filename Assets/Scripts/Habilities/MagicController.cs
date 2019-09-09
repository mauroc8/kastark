using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicController : MonoBehaviour
{
    [SerializeField] Image _fullBarImage = null;

    [SerializeField] float _speed = 4;

    // The Full Bar Image has 5px of border that shouldn't be considered.
    float _borderPercentage = 0.013f;

    float _lastPowerPercentage;
    float _lastTime;

    void Update() {
        var currentTime = Time.time;
        var currentPowerPercentage = 1 - Mathf.Abs(Mathf.Sin(currentTime * _speed));
        _fullBarImage.fillAmount = _borderPercentage + currentPowerPercentage * (1 - 2 * _borderPercentage);

        if (Input.GetMouseButtonDown(0) && Input.mousePosition.y > 100) {
            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(mRay, out hit)){
                if (hit.transform.gameObject.CompareTag("Team 2")) {
                    var target = hit.transform.gameObject;
                    // We want to calculate the actual power as the max of the powerPercentage function between
                    // _lastTime and currentTime.
                    var maxPowerMult = Mathf.PI / _speed;

                    var lastPowerPhase = _lastTime / maxPowerMult;
                    var currentPowerPhase = currentTime / maxPowerMult;

                    float power;

                    // If fract(powerPhase) < 0.5 then power is growing

                    if (lastPowerPhase % 1 < 0.5) {
                        if (currentPowerPhase % 1 < 0.5) {
                            power = 1;
                        } else {
                            power = currentPowerPercentage;
                        }
                    } else { // Power is decreasing
                        power = (currentPowerPercentage + _lastPowerPercentage) / 2;
                    }

                    Debug.Log($"Hit {target.name} with {power} effectivity.");
                }
            }
        }

        _lastPowerPercentage = currentPowerPercentage;
        _lastTime = Time.time;
    }
}
