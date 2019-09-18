using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Events;

public class MagicController : HabilityController
{
    public float duration = 1;
    public float selfHarmPower = 0.2f;

    [SerializeField] Image _powerImage = null;
    [SerializeField] float _emptyPercentage = 0;
    [SerializeField] float _selfHarmThreshold = 0;

    bool _stopAnimation;
    float _startTime;

    void OnEnable()
    {
        _stopAnimation = false;
        _startTime = Time.time;
    }

    void Update() {
        if (_stopAnimation) return;

        var t = (Time.time - _startTime) / duration;

        var power = Mathf.Pow(t, difficulty);

        if (power >= 1)
        {
            _stopAnimation = true;
            return;
        }

        _powerImage.fillAmount = _emptyPercentage + power * (1 - 2 * _emptyPercentage);

        if (_cast) return;

        if (power > _selfHarmThreshold)
        {
            _cast = true;
            EventController.TriggerEvent(new HabilityCastEvent(GameState.actingCreature, selfHarmPower));
            return;
        }
        

        var effectiveness = power / _selfHarmThreshold;

        if (Input.GetMouseButtonDown(0) && !Util.MouseIsOnUI()) {
            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(mRay, out hit)){
                var target = hit.transform.gameObject;
                if (!GameState.IsFromActingTeam(target)) {
                    var creature = target.GetComponent<Creature>();
                    _cast = true;
                    _stopAnimation = true;
                    EventController.TriggerEvent(new HabilityCastEvent(creature, effectiveness));
                }
            }
        }
    }
}
