

public abstract class _StreamBehaviour<TState> : StreamBehaviour
{
    // --- State ---

    protected StreamSource<TState> stateStream = new StreamSource<TState>();

    public StreamSource<TState> State => stateStream;

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

    protected StreamSource<TEvt> eventStream = new StreamSource<TEvt>();

    public Stream<E> EventStream<E>() where E : TEvt
    {
        return eventStream.FilterMap(Optional.FromCast<TEvt, E>);
    }
}
