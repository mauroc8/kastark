

public abstract class _StreamBehaviour<TState> : StreamBehaviour
{
    // --- State ---

    protected EventStream<TState> stateStream = new EventStream<TState>();

    public EventStream<TState> State => stateStream;

    protected abstract TState Init { get; }

    protected override void Start()
    {
        base.Start();
        // Init should happen after all subscriptions were made (typically, during awake).
        //stateStream.Init(Init);
    }
}


public abstract class _StreamBehaviour<TState, TEvt> : _StreamBehaviour<TState>
{
    // --- Evt ---

    protected EventStream<TEvt> eventStream = new EventStream<TEvt>();

    public Stream<E> EventStream<E>() where E : TEvt
    {
        return eventStream.FilterMap(Optional.FromCast<TEvt, E>);
    }
}
