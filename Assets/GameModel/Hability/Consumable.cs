using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable
{
    public Hability hability;
    public int      amount;

    public static List<Consumable> GetConsumableList(IEnumerable<Hability> habilities)
    {
        var list = new List<Consumable>();

        foreach (var hability in habilities)
        {
            AppendToConsumableList(list, hability);
        }

        return list;
    }

    public static void AppendToConsumableList(List<Consumable> list, Hability hability)
    {
        if (!list.Exists(consumable => consumable.hability == hability))
        {
            list.Add(new Consumable{ hability=hability, amount=1 });
        }
        else
        {
            var creature = list.Find(creat => creat.hability == hability);
            creature.amount++;
        } 
    }
}
