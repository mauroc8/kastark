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
                var habilityCastController = new HabilityCastController{
                    targets = new CreatureController[]{ GameState.actingCreature },
                    effectiveness = new float[]{ 1 },
                    hability = GameState.selectedHability
                };
                habilityCastController.Cast();            }
        }
    }
}
