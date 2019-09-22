using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class HealHability : HabilityController
{
    void Update() {
        if (_cast) return;

        if (Input.GetMouseButtonDown(0) && !Util.MouseIsOnUI()) {
            var target = Util.GetHoveredGameObject();

            if (target == GameState.actingCreature.gameObject)
            {
                _cast = true;
                EventController.TriggerEvent(Util.NewHabilityCastEvent(GameState.actingCreature, 1));
            }
        }
    }
}
