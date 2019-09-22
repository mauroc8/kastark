using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Events;

public class ConsumableButtonOnClick : HabilityButtonOnClick
{
    public void SetHandler(Consumable consumable)
    {
        var button = GetComponent<Button>();

        if (consumable.amount > 0)
        {
            button.onClick.AddListener(() =>
                EventController.TriggerEvent(new HabilitySelectEvent{ hability = consumable.hability }));
            consumable.amount--; // TO DO: Esto haría que al cancelar se pierda!
        }
        else
        {
            button.interactable = false;
        }
    }
}
