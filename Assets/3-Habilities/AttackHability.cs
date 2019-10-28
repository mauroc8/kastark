using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AttackHability : Hability
{
    TaskCompletionSourceWithAutoCancel<bool> _taskCompletionSource;

    public override Task CastAsync(Creature owner, CancellationToken token)
    {
        Debug.Log("Casting Attack");
        _taskCompletionSource = new TaskCompletionSourceWithAutoCancel<bool>(token);
        gameObject.SetActive(true);

        return _taskCompletionSource.Task;
    }

    public void OnCastEnd()
    {
        _taskCompletionSource.TrySetResult(true);
        gameObject.SetActive(false);
    }
}
