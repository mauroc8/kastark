using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Hability : MonoBehaviour
{
    public abstract Task CastAsync(CancellationToken token);
}
