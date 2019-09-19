using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Kastark/Consumable")]
public class Consumable : Hability
{
    [Header("Consumable")]
    [SerializeField] int initialAmount = 0;

    [System.NonSerialized] public int amount;
    
    public void Init()
    {
        amount = initialAmount;
    }
}
