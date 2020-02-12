
public interface StreamContext { }

public interface EventContext<Evt> : StreamContext
{
    Stream<E> EventStream<E>() where E : Evt;
}
