
public class EventStream<A> : Stream<A>
{
    protected override void Awake() { }
    protected override void Sleep() { }

    public void Push(A value)
    {
        PushToListeners(value);

        _lastValue =
            Optional.Some(value);
    }

    Optional<A> _lastValue =
        Optional.None<A>();

    public override Optional<A> lastValue =>
        _lastValue;
}
