using System;
using System.Collections.Generic;

/* 

See Stream.API to browse the public API.

This file is only about implementation details.

*/

public partial class Stream<A>
{

    protected abstract void Awake();
    protected abstract void Sleep();

    private List<Action<A>> listeners =
        new List<Action<A>>();

    // These 3 are public but only because of implementation details.

    // They could as well be protected if I could.

    public abstract Optional<A> lastValue { get; }

    public virtual void AddListener(Action<A> listener)
    {
        listeners.Add(listener);

        if (listeners.Count == 1)
        {
            // "Someone's listening me, I must begin
            // listening my source."

            Awake();
        }
    }

    public virtual void RemoveListener(Action<A> listener)
    {
        listeners.Remove(listener);

        if (listeners.Count == 0)
        {
            // "I can stop listening my source now
            // that no one's watching ..."

            Sleep();

            // This ensures unused streams are garbage collected.
        }
    }

    protected void PushToListeners(A value)
    {
        // This is O(n^2) *ON PURPOSE*.
        // Do not cache the result of .Count !

        // listeners usually shrinks and grows while iterating

        for (int i = 0; i < listeners.Count; i++)
        {
            listeners[i](value);
        }
    }
}


namespace StreamOperations
{
    class StreamMap<A, B> : Stream<B>
    {
        public Stream<A> source;
        public Func<A, B> map;

        protected override void Awake()
        {
            source.AddListener(this.Push);
        }

        protected override void Sleep()
        {
            source.RemoveListener(this.Push);
        }

        private void Push(A value)
        {
            PushToListeners(map(value));
        }

        public override Optional<B> lastValue =>
            source.lastValue.Map(map);
    }

    class StreamMap2<A, B, C> : Stream<C>
    {
        public Stream<A> sourceA;
        public Stream<B> sourceB;
        public Func<A, B, C> map;

        protected override void Awake()
        {
            sourceA.AddListener(this.PushA);
            sourceB.AddListener(this.PushB);
        }

        protected override void Sleep()
        {
            sourceA.RemoveListener(this.PushA);
            sourceB.RemoveListener(this.PushB);
        }

        private void PushA(A valueA)
        {
            sourceB.lastValue
                .Get(valueB =>
                {
                    PushToListeners(map(valueA, valueB));
                });
        }

        private void PushB(B valueB)
        {
            sourceA.lastValue
                .Get(valueA =>
                {
                    PushToListeners(map(valueA, valueB));
                });
        }

        public override Optional<C> lastValue =>
            Optional.Map2(sourceA.lastValue, sourceB.lastValue, map);
    }

    class StreamFilter<A> : Stream<A>
    {
        public Stream<A> source;
        public Func<A, bool> filter;

        protected override void Awake()
        {
            source.AddListener(this.Push);
        }

        protected override void Sleep()
        {
            source.RemoveListener(this.Push);
        }

        private void Push(A value)
        {
            if (filter(value))
                PushToListeners(value);
        }

        public override Optional<A> lastValue =>
            source.lastValue.Filter(filter);
    }

    public class StreamAndThen<A, B> : Stream<B>
    {
        public Stream<A> source;
        public Func<A, Stream<B>> andThen;

        protected override void Awake()
        {
            source.AddListener(this.Push);
        }

        protected override void Sleep()
        {
            source.RemoveListener(this.Push);
        }

        private Stream<B> _stream = null;

        private void Push(A value)
        {
            if (_stream != null)
                _stream.RemoveListener(PushToListeners);

            _stream = andThen(value);

            if (_stream != null)
                _stream.AddListener(PushToListeners);

            else
                UnityEngine.Debug.LogError($"Failed to get stream from AndThen call.");
        }

        public override Optional<B> lastValue =>
            source.lastValue
                .AndThen(value => andThen(value).lastValue);
    }

    public class StreamFilterMap<A, B> : Stream<B>
    {
        public Stream<A> source;
        public Func<A, Optional<B>> filterMap;

        protected override void Awake()
        {
            source.AddListener(this.Push);
        }

        protected override void Sleep()
        {
            source.RemoveListener(this.Push);
        }

        private void Push(A value)
        {
            switch (filterMap(value))
            {
                case Some<B> b:
                    PushToListeners(b.Value);
                    break;
            }
        }

        public override Optional<B> lastValue =>
            source.lastValue.AndThen(filterMap);
    }

    class StreamFold<A, B> : Stream<B>
    {
        public Stream<A> source;
        public Func<B, A, B> fold;
        public B accumulator;

        protected override void Awake()
        {
            source.AddListener(this.Push);
        }

        protected override void Sleep()
        {
            source.RemoveListener(this.Push);
        }

        private void Push(A value)
        {
            accumulator = fold(accumulator, value);
            PushToListeners(accumulator);
        }

        public override Optional<B> lastValue =>
            Optional.Some(accumulator);
    }

    public class StreamLazy<A> : Stream<A>
    {
        public Stream<A> source;

        private Optional<A> _lastValue = new None<A>();

        protected override void Awake()
        {
            source.AddListener(this.Push);
        }

        protected override void Sleep()
        {
            source.RemoveListener(this.Push);
        }

        private void Push(A value)
        {
            switch (_lastValue)
            {
                case Some<A> a:
                    if (!Functions.Eq(a.Value, value))
                    {
                        PushToListeners(value);
                        _lastValue = new Some<A>(value);

                    }

                    break;

                case None<A> a:
                    PushToListeners(value);
                    _lastValue = new Some<A>(value);

                    break;
            }
        }

        public override Optional<A> lastValue =>
            source.lastValue;
    }


    public class StreamMerge<A> : Stream<A>
    {
        public Stream<A> stream0;
        public Stream<A> stream1;

        protected override void Awake()
        {
            stream0.AddListener(PushToListeners);
            stream1.AddListener(PushToListeners);
        }

        protected override void Sleep()
        {
            stream0.RemoveListener(PushToListeners);
            stream1.RemoveListener(PushToListeners);
        }

        public override Optional<A> lastValue =>
            stream0.lastValue;
    }

    // Stream<A>.None. A stream that never yields any value.
    // Useful paired with Stream<A>#AndThen

    public class StreamNone<A> : Stream<A>
    {
        protected override void Awake()
        {
            //
        }

        protected override void Sleep()
        {
            //
        }

        public override Optional<A> lastValue =>
            Optional.None<A>();
    }

    // A stream that yields a value a single time to each listener.

    public class StreamOf<A> : Stream<A>
    {
        public A value;

        protected override void Awake() { }

        protected override void Sleep() { }

        public override void AddListener(Action<A> listener)
        {
            base.AddListener(listener);

            listener(value);
        }

        public override Optional<A> lastValue =>
            Optional.Some(value);
    }


    public class StreamInitializeWith<A> : Stream<A>
    {
        public A initialValue;
        public Stream<A> source;

        protected override void Awake()
        {
            source.AddListener(PushToListeners);
        }

        protected override void Sleep()
        {
            source.RemoveListener(PushToListeners);
        }

        public override void AddListener(Action<A> listener)
        {
            base.AddListener(listener);

            listener(initialValue);
        }

        public override Optional<A> lastValue =>
            Optional.Some(
                source.lastValue
                    .WithDefault(initialValue)
            );
    }

    public class StreamInitialized<A> : Stream<A>
    {
        public Stream<A> source;

        protected override void Awake()
        {
            source.AddListener(PushToListeners);
        }

        protected override void Sleep()
        {
            source.RemoveListener(PushToListeners);
        }

        public override void AddListener(Action<A> listener)
        {
            base.AddListener(listener);

            lastValue
                .Get(listener);
        }

        public override Optional<A> lastValue =>
            source.lastValue;
    }

    public class StreamFromArray<A> : Stream<(A, int)>
    {
        public Stream<A>[] streamArray;

        protected override void Awake()
        {
            for (int i = 0; i < streamArray.Length; i++)
            {
                streamArray[i].AddListener(Push[i]);
            }
        }

        protected override void Sleep()
        {
            for (int i = 0; i < streamArray.Length; i++)
            {
                streamArray[i].RemoveListener(Push[i]);
            }
        }

        public override Optional<(A, int)> lastValue =>
            streamArray[0].lastValue
                .Map(value => (value, 0));

        private Action<A>[] push = null;

        private Action<A>[] Push =>
            push == null ? (push = InitialPush) : push;

        private Action<A>[] InitialPush
        {
            get
            {
                var initialPush =
                    new Action<A>[streamArray.Length];

                for (int i = 0; i < streamArray.Length; i++)
                {
                    // closures in c# are weird
                    var internal_i =
                        i;

                    initialPush[i] =
                        value => PushToListeners((value, internal_i));
                }

                return initialPush;
            }
        }
    }
}


public partial class Stream<A, B> : Stream<(A, B)>
{
    public Stream<(A, B)> source;

    public static Stream<A, B> From(Stream<(A, B)> source)
    {
        return new Stream<A, B>
        {
            source = source
        };
    }

    protected override void Awake()
    {
        source.AddListener(PushToListeners);
    }

    protected override void Sleep()
    {
        source.RemoveListener(PushToListeners);
    }

    public override Optional<(A, B)> lastValue =>
        source.lastValue;
}
