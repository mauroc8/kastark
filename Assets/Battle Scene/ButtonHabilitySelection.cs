using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class ButtonHabilitySelection : HabilitySelection
{
    [SerializeField] UnityEvent _habilitySelectionStart;
    [SerializeField] UnityEvent _habilitySelectionEnd;
    
    TaskCompletionSourceWithAutoCancel<Hability> _taskCompletionSource;

    public override Task<Hability> SelectHabilityAsync(CancellationToken token)
    {
        _habilitySelectionStart.Invoke();
        _taskCompletionSource = new TaskCompletionSourceWithAutoCancel<Hability>(token);
        return _taskCompletionSource.Task;
    }

    public void SelectHabilityHandler(Hability hability)
    {
        if (_taskCompletionSource == null) return;

        _taskCompletionSource.SetResult(hability);
        _taskCompletionSource = null;
        _habilitySelectionEnd.Invoke();
    }
}