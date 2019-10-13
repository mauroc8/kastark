using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConsumableButtonContent : HabilityButtonContent
{
    [Header("Consumable")]
    [SerializeField] TextMeshProUGUI _amount = null;

    public void FillContent(Consumable consumable)
    {
        base.FillContent(consumable.hability);
        _amount.text = $"x{consumable.amount}";
    }
}