using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConsumableButtonContent : HabilityButtonContent
{
    [Header("Consumable")]
    [SerializeField] TextMeshProUGUI _amount = null;

    public override void FillContent(Hability consumable)
    {
        base.FillContent(consumable);
        _amount.text = $"x{((Consumable) consumable).amount}";
    }
}