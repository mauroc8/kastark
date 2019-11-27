using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class OthokHabilitySelection : HabilitySelection
{
    [SerializeField] Hability _hability;

    public override async Task<Hability> SelectHabilityAsync(CancellationToken token)
    {
        await Task.Delay(400, token);
        return _hability;
    }
}
