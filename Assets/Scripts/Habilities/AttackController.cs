using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class AttackController : MonoBehaviour
{
    [SerializeField] AttackTrail _attackTrail;

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
                    var effectiveness = _attackTrail.effectiveness;
                    Debug.Log(effectiveness);
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
