using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Kastark/Consumable")]
public class Consumable : ScriptableObject
{
    public Hability hability;
    [SerializeField] int _initialAmount = 1;

    [System.NonSerialized] public int amount = 0;

    public void Init()
    {
        hability.Init();
        amount = _initialAmount;
    }
}
