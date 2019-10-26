using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Events;

public class ConsumableButtonHandler : HabilityButtonHandler
{
    public void SetHandler(Consumable consumable)
    {
        var button = GetComponent<Button>();

        if (consumable.amount > 0)
        {
            button.onClick.AddListener(() => {
                // TODO
                //consumable.hability.Cast(Global.actingCreature, 1);
                consumable.amount--;
                EventController.TriggerEvent(new HabilityCastEvent{});
            });
        }
        else
        {
            button.interactable = false;
        }
    }
}
