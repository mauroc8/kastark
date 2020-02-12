using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotionController : StreamBehaviour
{
    public int heal = 3;

    protected override void Awake()
    {
        var creature = GetContext<Creature>();

        var consumable = _.NullCheck(GetComponent<Consumable>());

        for (int i = 0; i < heal; i++)
            AtMountTime(0.35f * (i + 1))
                .Get(_ =>
                {
                    creature.HealthPotionWasDrank(1);
                });

        AtMountTime(0.35f * (heal + 2)).Get(_ =>
        {
            Debug.Log($"Cast end");
            consumable.OnCastEnd();
        });
    }
}
