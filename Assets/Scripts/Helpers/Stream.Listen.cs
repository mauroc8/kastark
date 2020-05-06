using UnityEngine;
using System;

public abstract partial class Stream<A>
{
    public void Listen(Component component, Action<A> listener)
    {
        var destroyStream =
            component.GetComponent<OnDestroyEvent>();

        if (destroyStream == null)
        {
            destroyStream =
                component.gameObject.AddComponent<OnDestroyEvent>();
        }

        this.AddListener(listener);

        destroyStream.AddListener(() =>
        {
            this.RemoveListener(listener);
        });
    }
}

public class OnDestroyEvent : MonoBehaviour
{
    public Action destroy =
        () => { };

    public void AddListener(Action listener)
    {
        destroy +=
            listener;
    }

    void OnDestroy()
    {
        destroy();
        destroy = null;
    }
}
