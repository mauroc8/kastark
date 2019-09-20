using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Events;

public class HabilityButtonOnClick : MonoBehaviour
{
    public void SetHandler(Hability hability)
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(() => TriggerHabilitySelectEvent(hability));
    }

    protected void TriggerHabilitySelectEvent(Hability hability)
    {
        EventController.TriggerEvent(new HabilitySelectEvent{ hability = hability });
    }
}
