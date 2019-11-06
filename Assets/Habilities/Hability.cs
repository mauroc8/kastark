using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Hability : MonoBehaviour
{
    public string habilityName;

    TaskCompletionSourceWithAutoCancel<bool> _taskCompletionSource;

    public virtual Task CastAsync(CancellationToken token)
    {
        Debug.Log($"Casting {habilityName}");
        gameObject.SetActive(true);

        _taskCompletionSource = new TaskCompletionSourceWithAutoCancel<bool>(token)
        { taskName = "hability" };
        return _taskCompletionSource.Task;
    }

    public void OnCastEnd()
    {
        gameObject.SetActive(false);
        _taskCompletionSource.SetResult(true);
        _taskCompletionSource = null;
    }
}
