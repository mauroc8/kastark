using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class HealHability : HabilityController
{
    void Update() {
        if (_cast) return;

        if (Input.GetMouseButtonDown(0) && !RaycastHelper.MouseIsOnUI()) {
            var target = RaycastHelper.GetHoveredGameObject();

            if (target == Global.actingCreature.gameObject)
            {
                _cast = true;
                Global.selectedHability.Cast(Global.actingCreature, 1);
                EventController.TriggerEvent(new HabilityCastEvent{});
            }
        }
    }
}
