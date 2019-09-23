using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class HabilityCastController
{
    public CreatureController[] targets;
    public float[] effectiveness;
    public Hability hability;

    private HabilityCastEvent _habilityCastEvent = new HabilityCastEvent();

    public void Cast()
    {
        _habilityCastEvent.targets = targets;
        _habilityCastEvent.damage  = hability.Damage;
        _habilityCastEvent.damageType = hability.DamageType;
        _habilityCastEvent.effectiveness = effectiveness;

        EventController.TriggerEvent(_habilityCastEvent);
    }
}
