using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Events;

public class HabilityButtonHandler : MonoBehaviour
{
    Hability _hability = null;

    public void SetHandler(Hability hability)
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(() =>
            EventController.TriggerEvent(new HabilitySelectEvent{ hability = hability }));
        
        _hability = hability;
    }

    void Update()
    {
        if (_hability != null)
        {
            if (Input.GetKeyDown(_hability.hotkey))
            {
                EventController.TriggerEvent(new HabilitySelectEvent{ hability = _hability });
            }
        }
    }
}
