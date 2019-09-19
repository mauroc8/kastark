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
        button.onClick.AddListener(() => TriggerSelectedHabilityEvent(consumable.hability));
        consumable.amount--;
    }
}
