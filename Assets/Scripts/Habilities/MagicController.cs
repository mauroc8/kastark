using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.UI;

public class MagicController : HabilityController
{
    [SerializeField] RectTransform _powerTransform = null;
    [SerializeField] float _fullPowerX = 0;
    [SerializeField] float _noPowerX   = 0;

    [SerializeField] float _speed = 5;

    void Update() {
        if (_cast) return;

        float t = Time.time * _speed * Mathf.Log(difficulty + 1);

        float effectiveness = Mathf.Floor(t) % 2 == 0 ? t % 1 : 1 - t % 1;

        effectiveness = Mathf.Pow(effectiveness, difficulty);

        var pos = _powerTransform.localPosition;
        pos.x = Mathf.Lerp(_noPowerX, _fullPowerX, effectiveness);
        
        _powerTransform.localPosition = pos;

        if (Input.GetMouseButtonDown(0) && !Util.MouseIsOnUI()) {
            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(mRay, out hit)){
                var target = hit.transform.gameObject;
                if (!GameState.IsFromActingTeam(target)) {
                    var creature = target.GetComponent<Creature>();
                    _cast = true;
                    EventController.TriggerEvent(new HabilityCastEvent(creature, effectiveness));
                }
            }
        }
    }
}
