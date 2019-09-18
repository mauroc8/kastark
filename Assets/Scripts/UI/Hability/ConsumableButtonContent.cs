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
        FillContent(consumable as Hability);
        _amount.text = $"x{consumable.amount}";
    }
}