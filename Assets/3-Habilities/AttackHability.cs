using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AttackHability : Hability
{
    TaskCompletionSourceWithAutoCancel<bool> _taskCompletionSource;

    public override Task CastAsync(CancellationToken token)
    {
        Debug.Log("Casting Attack");
        gameObject.SetActive(true);

        _taskCompletionSource = new TaskCompletionSourceWithAutoCancel<bool>(token);
        return _taskCompletionSource.Task;
    }

    public void OnCastEnd()
    {
        _taskCompletionSource.TrySetResult(true);
        gameObject.SetActive(false);
    }
}
