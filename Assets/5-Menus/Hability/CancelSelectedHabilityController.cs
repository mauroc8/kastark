using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class CancelSelectedHabilityController : MonoBehaviour
{
    public void CancelSelectedHability()
    {
        EventController.TriggerEvent(new HabilityCancelEvent{});
    }
}
