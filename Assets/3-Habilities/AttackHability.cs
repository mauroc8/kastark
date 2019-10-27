using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AttackHability : Hability
{
    [SerializeField] GameObject _controller;

    TaskCompletionSourceWithAutoCancel<bool> _taskCompletionSource;
    
    public override Task CastAsync(Creature owner, CancellationToken token)
    {
        Debug.Log("Casting Attack");
        _taskCompletionSource = new TaskCompletionSourceWithAutoCancel<bool>(token);
        _controller.SetActive(true);

        return _taskCompletionSource.Task;
    }

    public void OnCastEnd()
    {
        _taskCompletionSource.SetResult(true);
        _controller.SetActive(false);
        _taskCompletionSource = null;
    }
}
