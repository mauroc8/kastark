using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotionController : StreamBehaviour
{
    public int heal = 3;

    protected override void Awake()
    {
        var creature = GetComponentInParent<Creature>();

        var consumable = Functions.NullCheck(GetComponent<Consumable>());

        for (int i = 0; i < heal; i++)
            MountTimeStream(0.35f * (i + 1))
                .Get(_ =>
                {
                    creature.HealthPotionWasDrank(1);
                });

        MountTimeStream(0.35f * (heal + 2)).Get(_ =>
        {
            Debug.Log($"Cast end");
            consumable.OnCastEnd();
        });
    }
}
