using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MagicHability : Hability
{
    public override Task CastAsync(CancellationToken token)
    {
        Debug.Log("Casting Magic");

        return Task.Delay(1000);
    }
}
