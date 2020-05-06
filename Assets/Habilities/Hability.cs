using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public enum HabilityId
{
    Attack,
    Magic,
    Shield,
    HealthPotion
}

public interface IHability
{
    HabilityId habilityId { get; }
}

public class Hability : MonoBehaviour, IHability
{
    public string habilityName;

    [SerializeField] HabilityId _habilityId;
    public HabilityId habilityId => _habilityId;

    TaskCompletionSourceWithAutoCancel<bool> _taskCompletionSource;

    public virtual Task CastAsync(CancellationToken token)
    {
        Debug.Log($"Casting {habilityName}");
        gameObject.SetActive(true);

        _taskCompletionSource = new TaskCompletionSourceWithAutoCancel<bool>(token);
        return _taskCompletionSource.Task;
    }

    public void OnCastEnd()
    {
        gameObject.SetActive(false);

        if (_taskCompletionSource != null)
        {
            _taskCompletionSource.SetResult(true);
            _taskCompletionSource = null;
        }
    }
}
