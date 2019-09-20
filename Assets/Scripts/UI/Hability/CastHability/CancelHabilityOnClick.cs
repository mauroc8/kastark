using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Events;

public class CancelHabilityOnClick : MonoBehaviour
{
    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(() =>
            EventController.TriggerEvent(new HabilityCancelEvent()));
    }
}
