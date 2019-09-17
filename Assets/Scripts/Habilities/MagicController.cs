﻿using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.UI;

public class MagicController : Hability
{
    [SerializeField] RectTransform _powerTransform = null;
    [SerializeField] float _fullPowerX = 0;
    [SerializeField] float _noPowerX   = 0;

    [SerializeField] float _speed = 5;

    bool _cast = false;
    float _effectiveness;

    [SerializeField] float _xOffset = 0;

    void Update() {
        if (_cast) return;

        float t = Time.time * _speed;

        _effectiveness = Mathf.Floor(t) % 2 == 0 ? t%1 : 1 - t%1;
        _effectiveness = Mathf.Pow(_effectiveness, effectivenessPower);

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
                    
                    var targets = new Creature[1];
                    targets[0] = target.GetComponent<Creature>();
                    var effectiveness = new float[1];
                    effectiveness[0] = _effectiveness;

                    EventController.TriggerEvent(new HabilityCastEndEvent{
                        targets = targets,
                        effectiveness = effectiveness,
                        baseDamage = baseDamage,
                        damageType = damageType,
                    });
                }
            }
        }
    }
}
