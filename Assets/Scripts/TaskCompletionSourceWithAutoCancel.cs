using System;
using System.Threading;
using System.Threading.Tasks;

public class TaskCompletionSourceWithAutoCancel<T>
{
    TaskCompletionSource<T> _taskCompletionSource;
    CancellationTokenRegistration _cancellationTokenRegistration;

    public Task<T> Task => _taskCompletionSource.Task;

    public TaskCompletionSourceWithAutoCancel(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        _taskCompletionSource = new TaskCompletionSource<T>();        
        _cancellationTokenRegistration = cancellationToken.Register(SetCanceled);
    }

    public void SetCanceled()
    {
        _taskCompletionSource.SetCanceled();
        _cancellationTokenRegistration.Dispose();
    }

    public void SetResult(T result)
    {
        _taskCompletionSource.SetResult(result);
        _cancellationTokenRegistration.Dispose();
    }

    public void SetException(Exception exception)
    {
        _taskCompletionSource.SetException(exception);
        _cancellationTokenRegistration.Dispose();
    }

    public void TrySetCanceled()
    {
        _taskCompletionSource.TrySetCanceled();
        _cancellationTokenRegistration.Dispose();
    }

    public void TrySetResult(T result)
    {
        _taskCompletionSource.TrySetResult(result);
        _cancellationTokenRegistration.Dispose();
    }

    public void TrySetException(Exception exception)
    {
        _taskCompletionSource.TrySetException(exception);
        _cancellationTokenRegistration.Dispose();
    }
}