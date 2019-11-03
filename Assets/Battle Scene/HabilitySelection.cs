using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class HabilitySelection : MonoBehaviour
{
    public abstract Task<Hability> SelectHabilityAsync(CancellationToken token);
}
