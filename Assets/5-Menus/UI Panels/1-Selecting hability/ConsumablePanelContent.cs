using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumablePanelContent : HabilityPanelContent
{
    List<Consumable> _consumables;

    protected override void OnEnable()
    {
        _consumables = Global.actingCreature.creature.consumables;
        var N = _consumables.Count;

        for (int i = 0; i < N; i++)
        {
            var instance = Instantiate(_buttonPrefab);
            instance.transform.SetParent(transform, false);
            PositionInstance(instance, i, N);

            var consumable = _consumables[i];

            instance.GetComponent<ConsumableButtonContent>()?.FillContent(consumable);
            instance.GetComponent<ConsumableButtonHandler>()?.SetHandler(consumable);
        }
    }
}
