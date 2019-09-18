using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class HealHability : HabilityController
{
    void Update() {
        if (_cast) return;

        if (Input.GetMouseButtonDown(0) && !Util.MouseIsOnUI()) {
            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(mRay, out hit)){
                var target = hit.transform.gameObject;
                if (target == GameState.actingCreature.gameObject) {
                    _cast = true;
                    EventController.TriggerEvent(new HabilityCastEvent(GameState.actingCreature, 1));
                }
            }
        }
    }
}
