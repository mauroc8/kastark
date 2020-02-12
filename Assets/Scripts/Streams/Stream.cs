using System;
using System.Collections.Generic;

public abstract class Stream<A>
{
    private List<Action<A>> _listeners = new List<Action<A>> { };

    public void AddListener(Action<A> listener)
    {
        _listeners.Add(listener);

        if (_listeners.Count == 1)
        {
            // "Someone's listening me, I must begin
            // listening my source."

            TurnOn();
        }
    }

    public void RemoveListener(Action<A> listener)
    {
        _listeners.Remove(listener);

        if (_listeners.Count == 0)
        {
            // "I can stop listening my source now
            // that no one's watching ..."

            TurnOff();
        }
    }

    protected void PushToListeners(A value)
    {
        foreach (var listener in _listeners.ToArray())
            listener(value);
    }

    protected void ClearListeners()
    {
        if (_listeners.Count > 0)
        {
            _listeners.Clear();

            TurnOff();
        }
    }

    protected abstract void TurnOn();
    protected abstract void TurnOff();


    public abstract Maybe<A> LastValue { get; }

    /// <summary>
    /// `Get` the value of the stream. If the stream was initialized,
    /// the listener is called rightaway. Use this instead of `AddListener`.
    /// </summary>
    public void Get(Action<A> listener)
    {
        AddListener(listener);
        LastValue.Get(listener);
    }

    /// <summary>
    /// Perform an action each time this stream receives an update.
    /// If the stream has been initialized, the action is performed rightaway.
    /// </summary>
    public void Do(Action action)
    {
        Action<A> listener = _ => action();

        AddListener(listener);
        LastValue.Get(listener);
    }


    // Create a new stream from this source.


    public Stream<B> Map<B>(Func<A, B> map)
    {
        return new StreamMap<A, B>
        {
            source = this,
            map = map
        };
    }

    public Stream<C> Map2<B, C>(Stream<B> other, Func<A, B, C> map)
    {
        return new StreamMap2<A, B, C>
        {
            sourceA = this,
            sourceB = other,
            map = map
        };
    }

    /// <summary>
    /// Equivalent to `Map(_ => value)`.
    /// </summary>
    public Stream<B> Always<B>(B value)
    {
        return new StreamMap<A, B>
        {
            source = this,
            map = _ => value
        };
    }

    public Stream<A> Filter(Func<A, bool> filter)
    {
        return new StreamFilter<A>
        {
            source = this,
            filter = filter
        };
    }

    public Stream<B> Fold<B>(B accumulator, Func<B, A, B> fold)
    {
        return new StreamFold<A, B>
        {
            source = this,
            fold = fold,
            accumulator = accumulator
        };
    }

    public StreamTuple<B, C> FoldTuple<B, C>((B, C) accum, Func<B, C, A, (B, C)> fold)
    {
        return new StreamTuple<B, C>
        {
            source = Fold(accum, (tuple, value) =>
                fold(tuple.Item1, tuple.Item2, value))
        };
    }

    /// <summary>
    /// Returns a stream initialized to `value`.
    /// </summary>
    public Stream<A> StartWith(A value)
    {
        return Fold(value, (_, val) => val);
    }

    /// <summary>
    /// Returns the stream created with `andThen(value)` where `value` is `this` stream's value.
    /// </summary>
    public Stream<B> AndThen<B>(Func<A, Stream<B>> andThen)
    {
        return new StreamAndThen<A, B>
        {
            source = this,
            andThen = andThen
        };
    }

    /// <summary>
    /// Returns the `apply` stream where each function is applied using `this` stream's value.
    /// <code>
    public Stream<B> Apply<B>(Stream<Func<A, B>> apply)
    {
        return AndThen(value => apply.Map(f => f(value)));
    }

    public Stream<B> FilterMap<B>(Func<A, Maybe<B>> filterMap)
    {
        return new StreamFilterMap<A, B>
        {
            source = this,
            filterMap = filterMap
        };
    }

    /// <summary>
    /// Returns a new stream that only `Get`s a new value when it's different
    /// from the previous one.
    /// </summary>
    public Stream<A> Lazy()
    {
        return new StreamLazy<A>
        {
            source = this
        };
    }

    /// <summary>
    /// Returns a new `StreamTuple` that `Get`s both `this` and `streamB` values.
    /// </summary>
    public StreamTuple<A, B> Combine<B>(Stream<B> streamB)
    {
        return Map2Tuple(streamB, ValueTuple.Create);
    }

    public Stream<A> Merge(Stream<A> stream1)
    {
        return new StreamMerge<A>
        {
            stream0 = this,
            stream1 = stream1
        };
    }

    public StreamTuple<B, C> MapTuple<B, C>(Func<A, (B, C)> mapTuple)
    {
        return new StreamTuple<B, C>
        {
            source = Map(mapTuple)
        };
    }

    public StreamTuple<C, D> Map2Tuple<B, C, D>(Stream<B> other, Func<A, B, (C, D)> map)
    {
        return new StreamTuple<C, D>
        {
            source = Map2(other, map)
        };
    }

    public StreamTuple<B, C> AndThenTuple<B, C>(Func<A, Stream<(B, C)>> andThenTuple)
    {
        return new StreamTuple<B, C>
        {
            source = new StreamAndThen<A, (B, C)>
            {
                source = this,
                andThen = andThenTuple
            }
        };
    }

    public StreamTuple<B, C> FilterMapTuple<B, C>(Func<A, Maybe<(B, C)>> filterMapTuple)
    {
        return new StreamTuple<B, C>
        {
            source = new StreamFilterMap<A, (B, C)>
            {
                source = this,
                filterMap = filterMapTuple
            }
        };
    }


    // Static members

    /// <summary>
    /// Returns a Stream that never `Get`s any value.
    /// </summary>
    /// <remarks>
    /// Useful paired with `AndThen`. For example, `Filter` could be defined as:
    /// <code>
    /// public Stream<A> Filter(Func<A, bool> filter) {
    ///   return AndThen(value =>
    ///     filter(value)
    ///       ? Stream<A>.Singleton(value)
    ///       : Stream<A>.None());
    /// }
    /// </code>
    /// </remarks>
    public static Stream<A> None()
    {
        return new StreamNone<A> { };
    }

    /// <summary>
    /// Returns a Stream that `Get`s the `value` everytime it's turned on.
    /// </summary>
    /// <remarks>
    /// Useful paired with `AndThen`. For example, `Filter` could be defined as:
    /// <code>
    /// public Stream<A> Filter(Func<A, bool> filter) {
    ///   return AndThen(value =>
    ///     filter(value)
    ///       ? Stream<A>.Singleton(value)
    ///       : Stream<A>.None());
    /// }
    /// </code>
    /// </remarks>
    public static Stream<A> Singleton(A value)
    {
        return new StreamSingleton<A>
        {
            value = value
        };
    }
}



public static class Stream
{
    // Aliases.

    public static StreamTuple<A, B> Combine<A, B>(Stream<A> streamA, Stream<B> streamB)
    {
        return streamA.Combine(streamB);
    }

    public static Stream<A> Merge<A>(Stream<A> stream0, Stream<A> stream1)
    {
        return stream0.Merge(stream1);
    }
}


public class StreamSource<A> : Stream<A>
{
    protected override void TurnOn() { }
    protected override void TurnOff() { }

    private Maybe<A> _lastValue = new Nothing<A>();
    public override Maybe<A> LastValue => _lastValue;

    public void Push(A value)
    {
        _lastValue = new Just<A>(value);

        PushToListeners(value);
    }

    public void Update(Func<A, A> update)
    {
        _lastValue.Get(_.compose(Push, update));
    }

    public void Destroy()
    {
        ClearListeners();
        // I *hope* GC does the rest of the job.
    }
}


public class StreamMap<A, B> : Stream<B>
{
    public Stream<A> source;
    public Func<A, B> map;

    public override Maybe<B> LastValue =>
        source.LastValue.Map(map);

    protected override void TurnOn()
    {
        source.AddListener(this.Push);
    }

    protected override void TurnOff()
    {
        source.RemoveListener(this.Push);
    }

    private void Push(A value)
    {
        PushToListeners(map(value));
    }
}

public class StreamMap2<A, B, C> : Stream<C>
{
    public Stream<A> sourceA;
    public Stream<B> sourceB;
    public Func<A, B, C> map;

    public override Maybe<C> LastValue =>
        Maybe.Map2(sourceA.LastValue, sourceB.LastValue, map);

    protected override void TurnOn()
    {
        sourceA.AddListener(this.PushA);
        sourceB.AddListener(this.PushB);
    }

    protected override void TurnOff()
    {
        sourceA.RemoveListener(this.PushA);
        sourceB.RemoveListener(this.PushB);
    }

    private void PushA(A valueA)
    {
        sourceB.LastValue.Get(valueB =>
        {
            Push(valueA, valueB);
        });
    }

    private void PushB(B valueB)
    {
        sourceA.LastValue.Get(valueA =>
        {
            Push(valueA, valueB);
        });
    }

    private void Push(A valueA, B valueB)
    {
        PushToListeners(map(valueA, valueB));
    }
}



public class StreamFilter<A> : Stream<A>
{
    public Stream<A> source;
    public Func<A, bool> filter;

    public override Maybe<A> LastValue =>
        source.LastValue.Filter(filter);

    protected override void TurnOn()
    {
        source.AddListener(this.Push);
    }

    protected override void TurnOff()
    {
        source.RemoveListener(this.Push);
    }

    private void Push(A value)
    {
        if (filter(value))
            PushToListeners(value);
    }
}

public class StreamFold<A, B> : Stream<B>
{
    public Stream<A> source;
    public Func<B, A, B> fold;
    public B accumulator;

    public override Maybe<B> LastValue =>
        new Just<B>(accumulator);

    protected override void TurnOn()
    {
        source.AddListener(this.Push);
    }

    protected override void TurnOff()
    {
        source.RemoveListener(this.Push);
    }

    private void Push(A value)
    {
        accumulator = fold(accumulator, value);
        PushToListeners(accumulator);
    }
}

public class StreamAndThen<A, B> : Stream<B>
{
    public Stream<A> source;
    public Func<A, Stream<B>> andThen;

    public override Maybe<B> LastValue
    {
        get
        {
            if (_stream == null)
            {
                // Push sets _stream to a value.
                source.LastValue.Get(this.Push);

                if (_stream == null)
                    return new Nothing<B>();
            }

            return _stream.LastValue;
        }
    }

    protected override void TurnOn()
    {
        source.AddListener(this.Push);
    }

    protected override void TurnOff()
    {
        source.RemoveListener(this.Push);
    }

    private Stream<B> _stream = null;

    private void Push(A value)
    {
        if (_stream != null)
            _stream.RemoveListener(PushToListeners);

        _stream = andThen(value);
        _stream.AddListener(PushToListeners);
    }
}

public class StreamFilterMap<A, B> : Stream<B>
{
    public Stream<A> source;
    public Func<A, Maybe<B>> filterMap;

    public override Maybe<B> LastValue =>
        source.LastValue.AndThen(filterMap);

    protected override void TurnOn()
    {
        source.AddListener(this.Push);
    }

    protected override void TurnOff()
    {
        source.RemoveListener(this.Push);
    }

    private void Push(A value)
    {
        switch (filterMap(value))
        {
            case Just<B> b:
                PushToListeners(b.Value);
                break;
        }
    }
}

public class StreamLazy<A> : Stream<A>
{
    public Stream<A> source;

    private Maybe<A> _lastValue = new Nothing<A>();

    public override Maybe<A> LastValue =>
        _lastValue;

    protected override void TurnOn()
    {
        source.AddListener(this.Push);
    }

    protected override void TurnOff()
    {
        source.RemoveListener(this.Push);
    }

    private void Push(A value)
    {
        switch (_lastValue)
        {
            case Just<A> a:
                if (!_.eq(a.Value, value))
                {
                    PushToListeners(value);
                    _lastValue = new Just<A>(value);

                }

                break;

            case Nothing<A> a:
                PushToListeners(value);
                _lastValue = new Just<A>(value);

                break;
        }
    }
}

public class StreamTuple<A, B> : Stream<(A, B)>
{
    public Stream<(A, B)> source;

    public override Maybe<(A, B)> LastValue =>
        source.LastValue;

    protected override void TurnOn()
    {
        source.AddListener(PushToListeners);
    }

    protected override void TurnOff()
    {
        source.RemoveListener(PushToListeners);
    }

    public Stream<C> Map<C>(Func<A, B, C> map)
    {
        return new StreamMap<(A, B), C>
        {
            source = this,
            map = tuple => map(tuple.Item1, tuple.Item2)
        };
    }

    public StreamTuple<A, B> Filter(Func<A, B, bool> filter)
    {
        return new StreamTuple<A, B>
        {
            source = new StreamFilter<(A, B)>
            {
                source = this,
                filter = tuple => filter(tuple.Item1, tuple.Item2)
            }
        };
    }

    public Stream<C> Fold<C>(C accumulator, Func<C, A, B, C> fold)
    {
        return new StreamFold<(A, B), C>
        {
            source = this,
            fold = (accum, tuple) => fold(accum, tuple.Item1, tuple.Item2),
            accumulator = accumulator
        };
    }

    public Stream<C> AndThen<C>(Func<A, B, Stream<C>> andThen)
    {
        return new StreamAndThen<(A, B), C>
        {
            source = this,
            andThen = tuple => andThen(tuple.Item1, tuple.Item2)
        };
    }

    public Stream<C> FilterMap<C>(Func<A, B, Maybe<C>> filterMap)
    {
        return new StreamFilterMap<(A, B), C>
        {
            source = this,
            filterMap = tuple => filterMap(tuple.Item1, tuple.Item2)
        };
    }

    public void Get(Action<A, B> effect)
    {
        base.Get(tuple => effect(tuple.Item1, tuple.Item2));
    }
}

public class StreamMerge<A> : Stream<A>
{
    public Stream<A> stream0;
    public Stream<A> stream1;

    protected override void TurnOn()
    {
        stream0.AddListener(this.Push0);
        stream1.AddListener(this.Push1);
    }

    protected override void TurnOff()
    {
        stream0.RemoveListener(this.Push0);
        stream1.RemoveListener(this.Push1);
    }

    enum LastPush { None, Stream0, Stream1 };

    LastPush _lastPush = LastPush.None;

    private void Push0(A value)
    {
        _lastPush = LastPush.Stream0;
        PushToListeners(value);
    }

    private void Push1(A value)
    {
        _lastPush = LastPush.Stream1;
        PushToListeners(value);
    }

    public override Maybe<A> LastValue =>
        _lastPush == LastPush.None
            ? stream0.LastValue.OrThen(stream1.LastValue)
            : _lastPush == LastPush.Stream0
                ? stream0.LastValue
                : stream1.LastValue;
}

// Stream<A>.None. A stream that never yields any value.
// Useful paired with Stream<A>#AndThen

public class StreamNone<A> : Stream<A>
{
    public override Maybe<A> LastValue =>
        new Nothing<A>();

    protected override void TurnOn()
    {
        //
    }

    protected override void TurnOff()
    {
        //
    }
}

// A stream that yields a single value.

public class StreamSingleton<A> : Stream<A>
{
    public A value;

    public override Maybe<A> LastValue =>
        new Just<A>(value);

    protected override void TurnOn()
    {
        PushToListeners(value);
    }

    protected override void TurnOff()
    {
        //
    }
}
