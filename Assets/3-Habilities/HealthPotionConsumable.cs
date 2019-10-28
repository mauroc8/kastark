using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class HealthPotionConsumable : Consumable
{
    public override Task CastAsync(CancellationToken token)
    {
        Debug.Log("Casting Health");

        return Task.Delay(1000);
    }
}