using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ButtonHabilitySelection : HabilitySelection
{
    TaskCompletionSourceWithAutoCancel<Hability> _taskCompletionSource;
    
    public override Task<Hability> SelectHabilityAsync(CancellationToken token)
    {
        _taskCompletionSource = new TaskCompletionSourceWithAutoCancel<Hability>(token);
        return _taskCompletionSource.Task;
    }

    public void SelectHabilityHandler(Hability hability)
    {
        if (_taskCompletionSource == null) return;
        
        _taskCompletionSource.SetResult(hability);
        _taskCompletionSource = null;
    }
}