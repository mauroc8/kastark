using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class HealHability : Hability
{
    bool _cast = false;

    void Update() {
        if (_cast) return;

        if (Input.GetMouseButtonDown(0) && !Util.MouseIsOnUI()) {
            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(mRay, out hit)){
                var target = hit.transform.gameObject;
                if (target == GameState.actingCreature.gameObject) {
                    _cast = true;
                    EventController.TriggerEvent(new HabilityCastStartEvent());
                    
                    var targets = new Creature[1];
                    targets[0] = GameState.actingCreature;
                    var effectiveness = new float[1];
                    effectiveness[0] = 1;

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
