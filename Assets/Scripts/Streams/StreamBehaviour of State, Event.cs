using System;
using System.Collections.Generic;

public abstract class StreamBehaviour<T_State, T_Event> :
    StreamBehaviour,
    StreamContext,
    EventContext<T_Event>
{
    protected StreamSource<T_State> stateStream = new StreamSource<T_State>();
    public Stream<T_State> StateStream => stateStream;

    protected StreamSource<T_Event> eventStream = new StreamSource<T_Event>();

    public Stream<E> EventStream<E>() where E : T_Event
    {
        return eventStream.FilterMap(Maybe.FromCast<T_Event, E>);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        stateStream.Destroy();
        eventStream.Destroy();
    }
}
