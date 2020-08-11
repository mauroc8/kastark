using UnityEngine;
using System;

public abstract partial class Stream<A>
{
    public Stream<A> Bind(Component component)
    {
        var destroyStream =
            component.GetComponent<OnDestroyEvent>();

        if (destroyStream == null)
        {
            destroyStream =
                component.gameObject.AddComponent<OnDestroyEvent>();
        }

        return new StreamOperations.StreamBind<A>
        {
            source = this,
            destroyEvent = destroyStream
        };
    }
}

namespace StreamOperations
{
    public class StreamBind<A> : Stream<A>
    {
        public Stream<A> source;
        public OnDestroyEvent destroyEvent;

        protected override void Awake()
        {
            source.AddListener(PushToListeners);
            destroyEvent.AddListener(Sleep);
        }

        protected override void Sleep()
        {
            source.RemoveListener(PushToListeners);
            destroyEvent.RemoveListener(Sleep);
        }

        public override Optional<A> lastValue =>
            source.lastValue;
    }
}

public class OnDestroyEvent : MonoBehaviour
{
    private Action destroy =
        () => { };

    public void AddListener(Action listener)
    {
        destroy +=
            listener;
    }

    public void RemoveListener(Action listener)
    {
        destroy -=
            listener;
    }

    void OnDestroy()
    {
        destroy();
        destroy = null;
    }
}
