
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AttackHability : Hability
{
    public override Task CastAsync(Creature owner, CancellationToken token)
    {
        Debug.Log("Casting Attack");

        return Task.Delay(1000);
    }
}
