using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class AttackController : Hability
{
    [SerializeField] AttackTrail _attackTrail;

    float _effectiveness;

    void Start() {
        _attackTrail = GetComponentInChildren<AttackTrail>();
    }

    bool _casting = false;
    bool _cast = false;

    void Update()
    {
        if (_cast) return;

        bool clicking = Input.GetMouseButton(0);
            
        if (_casting) {
            bool stillOpen = clicking && _attackTrail.Move(Input.mousePosition);
            
            if (!stillOpen) {
                bool wasClosed = _attackTrail.Close();

                if (wasClosed) {
                    _cast = true;
                    EventController.TriggerEvent(new HabilityCastStartEvent());
                    _effectiveness = _attackTrail.Effectiveness;
                    _effectiveness = Mathf.Pow(_effectiveness, effectivenessPower);
                    
                    var targets = _attackTrail.GetTargets();
                    var effectiveness = _attackTrail.GetEffectiveness();

                    EventController.TriggerEvent(new HabilityCastEndEvent{
                        targets = targets,
                        effectiveness = effectiveness,
                        baseDamage = baseDamage,
                        damageType = damageType,
                    });
                    
                } else {
                    _attackTrail.Restart();
                }
            }
        } else if (clicking && !Util.MouseIsOnUI()) {
            _attackTrail.Open(Input.mousePosition);
            _casting = true;
        }
    }
}
