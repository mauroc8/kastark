using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumablePanelContent : HabilityPanelContent
{
    [SerializeField] Consumable[] _consumables = {};

    protected override void Start()
    {
        for (int i = _consumables.Length - 1; i >= 0; i--)
        {
            var instance = Instantiate(_prefab);
            instance.transform.SetParent(transform, false);
            PositionInstance(instance, i);

            var consumable = _consumables[i];
            instance.GetComponent<ConsumableButtonContent>()?.FillContent(consumable);
            instance.GetComponent<ConsumableButtonOnClick>()?.SetHandler(consumable);
        }
    }
}
