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
                EventController.TriggerEvent(new HabilitySelectEvent{ hability = consumable.hability });
                EventController.TriggerEvent(new ConsumableSelectEvent{ consumable = consumable });
            });
        }
        else
        {
            button.interactable = false;
        }
    }
}
