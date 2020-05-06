using StreamOperations;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A Stream is a flow of data through time.
/// A Stream can represent an event or state that changes over time.
///
/// See StreamSource to create your own Stream.
/// 
/// To subscribe to the stream use `Get` or `AddListener`/`RemoveListener`.
/// 
/// There are also Stream transformations like `Map`, `Filter` or `AndThen`
/// (these are the equivalent of Linq's `Select`, `Where` and `SelectMany`).
/// 
/// See also the static class `Stream` that has some methods like `Stream.Merge`
/// or `Stream.Combine`.
/// </summary>
public abstract partial class Stream<A>
{
    /// <summary>
    /// `Get` the value of the stream.
    /// The callback is called every time the stream receives a new value.
    /// 
    /// This is equivalent to `AddListener`. In most cases you won't need to
    /// explicitly remove the listener.
    /// </summary>
    public void Get(Action<A> callback)
    {
        AddListener(callback);
    }

    /// <summary>
    /// Subscribe to the stream, but ignore its value. Similar to `Get`, but
    /// taking an `Action` instead of an `Action&lt;A&gt;`.
    /// </summary>
    public void Do(Action listener)
    {
        AddListener(_ => listener());
    }

    /// <summary>
    /// Create a new stream transforming every value using the `map` function.
    /// </summary>
    public Stream<B> Map<B>(Func<A, B> map)
    {
        return new StreamMap<A, B>
        {
            source = this,
            map = map
        };
    }

    internal Stream<C> Map2<B, C>(Stream<B> other, Func<A, B, C> map)
    {
        return new StreamMap2<A, B, C>
        {
            sourceA = this,
            sourceB = other,
            map = map
        };
    }


    /// <summary>
    /// Convert the stream to always yield the same `value`.
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

    /// <summary>
    /// Convert the stream to only yield values that pass a certain `filter`. 
    /// </summary>
    public Stream<A> Filter(Func<A, bool> filter)
    {
        return new StreamFilter<A>
        {
            source = this,
            filter = filter
        };
    }

    /// <summary>
    /// Convert the stream using a `fold` function and an initial value.
    /// The `fold` function receives the old accumulator and returns the new one.
    /// 
    /// For example, to lerp a float with its previous value:
    /// <code>
    /// floatStream
    ///     .Accumulate(0.0f, (lastValue, currValue) =>
    ///         Mathf.Lerp(lastValue, currValue, 12f * Time.deltaTime)
    ///     );
    /// </code>
    /// </summary>
    public Stream<B> Accumulate<B>(B accumulator, Func<B, A, B> fold)
    {
        return new StreamFold<A, B>
        {
            source = this,
            fold = fold,
            accumulator = accumulator
        };
    }

    /// <summary>
    /// Get a `Stream&lt;A, A&gt;` that yields `(lastValue, value)`.
    /// </summary>
    public Stream<A, A> WithLastValue(A initialValue)
    {
        return Stream<A, A>.From(
            this.Accumulate((initialValue, initialValue),
                (memory, value) => (memory.Item2, value)
            )
        );
    }

    /// <summary>
    /// Returns the stream created inside the given `andThen` function.
    ///
    /// The following example creates a stream that yields the behaviour's
    /// lifetime on every `update`
    /// <code>
    /// start
    ///     .Map(_ => Time.time)
    ///     .AndThen(update.Always)
    ///     .Map(t => Time.time - t);
    /// </code>
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
    /// Returns the `apply` stream where each value is applied using `this` stream's value.
    /// <code>
    public Stream<B> Apply<B>(Stream<Func<A, B>> apply)
    {
        return AndThen(value => apply.Map(f => f(value)));
    }

    /// <summary>
    /// Construct a stream that yields when `Some` value is received.
    ///
    /// Let's say that we have an `Event` that can either be a `PlayerEvent` or a
    /// `WorldEvent`. 
    ///
    /// We can convert a `Stream&lt;Event&gt;` into a `Stream&lt;PlayerEvent&gt;` (ignoring all
    /// `WorldEvents`) using:
    ///
    /// <code>
    /// var playerEvent = event.FilterMap(Optional.FromCast&lt;Event, PlayerEvent&gt;);
    /// </code>
    ///
    /// </summary>
    public Stream<B> FilterMap<B>(Func<A, Optional<B>> filterMap)
    {
        return new StreamFilterMap<A, B>
        {
            source = this,
            filterMap = filterMap
        };
    }

    /// <summary>
    /// Returns a stream that only yields a value when it's different
    /// from the last time.
    /// </summary>
    public Stream<A> Lazy()
    {
        return new StreamLazy<A>
        {
            source = this
        };
    }

    internal Stream<A, B> Combine<B>(Stream<B> streamB)
    {
        return this.Map2(streamB, ValueTuple.Create);
    }

    internal Stream<A> Merge(Stream<A> stream1)
    {
        return new StreamMerge<A>
        {
            stream0 = this,
            stream1 = stream1
        };
    }

    public Stream<B, C> Map<B, C>(Func<A, (B, C)> mapTuple)
    {
        return Stream<B, C>.From(
            new StreamMap<A, (B, C)>
            {
                source = this,
                map = mapTuple
            }
        );
    }

    public Stream<C, D> Map2<B, C, D>(Stream<B> other, Func<A, B, (C, D)> mapTuple)
    {
        return Stream<C, D>.From(
            new StreamMap2<A, B, (C, D)>
            {
                sourceA = this,
                sourceB = other,
                map = mapTuple
            }
        );
    }

    public Stream<B, C> AndThen<B, C>(Func<A, Stream<(B, C)>> andThenTuple)
    {
        return Stream<B, C>.From(
            new StreamAndThen<A, (B, C)>
            {
                source = this,
                andThen = andThenTuple
            }
        );
    }

    public Stream<B, C> FilterMap<B, C>(Func<A, Optional<(B, C)>> filterMapTuple)
    {
        return Stream<B, C>.From(
            new StreamFilterMap<A, (B, C)>
            {
                source = this,
                filterMap = filterMapTuple
            }
        );
    }

    internal static Stream<A> None()
    {
        return new StreamNone<A> { };
    }

    internal static Stream<A> Of(A value)
    {
        return new StreamSingleton<A>
        {
            value = value
        };
    }
}

/// <summary>
/// Under the hood, a `Stream&lt;A, B&gt;` is just a `Stream&lt;(A, B)&gt;`.
///
/// The sole purpose of this class is to add friendlier method calls.
/// For example, `Map(Func&lt;A, B, C&gt; map)` instead of `Map(Func&lt;(A, B), C&gt; map)`.
/// </summary>
public partial class Stream<A, B> : Stream<(A, B)>
{

    public Stream<C> Map<C>(Func<A, B, C> map)
    {
        return source.Map(tuple => map(tuple.Item1, tuple.Item2));
    }

    public Stream<A, B> Filter(Func<A, B, bool> filter)
    {
        return Stream<A, B>.From(source.Filter(tuple => filter(tuple.Item1, tuple.Item2)));
    }

    public Stream<C> Accumulate<C>(C accumulator, Func<C, A, B, C> fold)
    {
        return source.Accumulate(accumulator, (accum, tuple) => fold(accum, tuple.Item1, tuple.Item2));
    }

    public Stream<C> AndThen<C>(Func<A, B, Stream<C>> andThen)
    {
        return source.AndThen(tuple => andThen(tuple.Item1, tuple.Item2));
    }

    public Stream<C> FilterMap<C>(Func<A, B, Optional<C>> filterMap)
    {
        return source.FilterMap(tuple => filterMap(tuple.Item1, tuple.Item2));
    }

    public void Get(Action<A, B> effect)
    {
        source.Get(tuple => effect(tuple.Item1, tuple.Item2));
    }
}

public static class Stream
{
    /// <summary>
    /// Returns a new `Stream&lt;A, B&gt;` that yield when any of its sources does.
    /// It uses the other source's previous value.
    /// </summary>
    public static Stream<A, B> Combine<A, B>(Stream<A> streamA, Stream<B> streamB)
    {
        return streamA.Combine(streamB);
    }

    /// <summary>
    /// Returns a new `Stream&lt;A&gt;` that merges many streams into one.
    /// </summary>
    public static Stream<A> Merge<A>(params Stream<A>[] streams)
    {
        return MergeHelp(streams, 0, streams.Length);
    }

    /// <summary>
    /// Returns a new `Stream&lt;A&gt;` that merges many streams into one.
    /// </summary>
    public static Stream<A> Merge<A>(IEnumerable<Stream<A>> streams)
    {
        return MergeHelp(streams.ToArray(), 0, streams.Count());
    }

    /// <summary>
    /// Uses a map function to convert two streams into one.
    /// </summary>
    public static Stream<C> Map2<A, B, C>(Stream<A> streamA, Stream<B> streamB, Func<A, B, C> mapFunction)
    {
        return streamA.Map2(streamB, mapFunction);
    }

    /// <summary>
    /// Returns a `Stream&lt;A&gt;` that sends a single `value`, once, to its first listener.
    /// </summary>
    public static Stream<A> Of<A>(A value)
    {
        return Stream<A>.Of(value);
    }

    /// <summary>
    /// Returns a Sream&lt;A&gt; that will never send nothing to no one.
    /// </summary>
    public static Stream<A> None<A>()
    {
        return Stream<A>.None();
    }


    // Helpers to optimize Merge.

    private static Stream<A> MergeHelp<A>(Stream<A>[] streams, int i, int j)
    {
        // Returns the merged streams in a binary tree of `Merge2`s.
        // This makes pushing values to the returned stream O(logn) instead of O(n).

        if (j - i <= 0)
        {
            return Stream.None<A>();
        }
        if (j - i == 1)
        {
            return streams[i];
        }

        int k = (i + j) / 2;

        return
            Stream.Merge2(
                Stream.MergeHelp(streams, i, k),
                Stream.MergeHelp(streams, k, j)
            );
    }

    private static Stream<A> Merge2<A>(Stream<A> stream0, Stream<A> stream1)
    {
        return stream0.Merge(stream1);
    }
}
