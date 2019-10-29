using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MagicHability : Hability
{
    TaskCompletionSourceWithAutoCancel<bool> _taskCompletionSource;
    
    public override Task CastAsync(CancellationToken token)
    {
        Debug.Log("Casting Magic");

        gameObject.SetActive(true);

        _taskCompletionSource = new TaskCompletionSourceWithAutoCancel<bool>(token);

        return _taskCompletionSource.Task;
    }

    public void OnCastEnd()
    {
        gameObject.SetActive(false);
        _taskCompletionSource.SetResult(true);
        _taskCompletionSource = null;
    }
}
