
/// <summary>
/// `StreamSource&lt;A&gt;` is the only way to create streams were you can write values.
///
/// A `StreamSource` is a `Stream` with read-write access. See `Push` to write
/// values to the Stream.
///
/// In this example we're creating a StreamSource, but exposing it as a read-only Stream:
/// <code>
/// private StreamSource&lt;A&gt; myStream = new StreamSource&lt;A&gt;();
/// public Stream&lt;A&gt; MyStream => myStream;
/// </code>
/// </summary>
public class EventStream<A> : Stream<A>
{
    protected override void HasListener() { }
    protected override void DoesntHaveListener() { }

    public void Push(A value)
    {
        PushToListeners(value);
    }
}

public class StreamSource<A, B> : Stream<A, B>
{
    protected override void HasListener() { }
    protected override void DoesntHaveListener() { }

    public void Push(A a, B b)
    {
        PushToListeners((a, b));
    }
}
