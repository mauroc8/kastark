using System;
using System.Collections.Generic;

/* 

See Stream.API to browse the public API.

This file is only about implementation details.

*/

public abstract partial class Stream<A>
{
    private List<Action<A>> _listeners = new List<Action<A>> { };

    internal void AddListener(Action<A> listener)
    {
        _listeners.Add(listener);

        if (_listeners.Count == 1)
        {
            // "Someone's listening me, I must begin
            // listening my source."

            HasListener();
        }
    }

    internal void RemoveListener(Action<A> listener)
    {
        _listeners.Remove(listener);

        if (_listeners.Count == 0)
        {
            // "I can stop listening my source now
            // that no one's watching ..."

            DoesntHaveListener();

            // This ensures unused streams are garbage collected.
        }
    }

    protected void PushToListeners(A value)
    {
        // This is O(n^2) *ON PURPOSE*.
        // Do not cache .Count !

        for (int i = 0; i < _listeners.Count; i++)
        {
            _listeners[i](value);
        }
    }

    protected abstract void HasListener();
    protected abstract void DoesntHaveListener();
}


namespace StreamOperations
{
    class StreamMap<A, B> : Stream<B>
    {
        public Stream<A> source;
        public Func<A, B> map;

        protected override void HasListener()
        {
            source.AddListener(this.Push);
        }

        protected override void DoesntHaveListener()
        {
            source.RemoveListener(this.Push);
        }

        private void Push(A value)
        {
            PushToListeners(map(value));
        }
    }

    class StreamMap2<A, B, C> : Stream<C>
    {
        public Stream<A> sourceA;
        public Stream<B> sourceB;
        public Func<A, B, C> map;

        protected override void HasListener()
        {
            sourceA.AddListener(this.PushA);
            sourceB.AddListener(this.PushB);
        }

        protected override void DoesntHaveListener()
        {
            sourceA.RemoveListener(this.PushA);
            sourceB.RemoveListener(this.PushB);
        }

        private Optional<A> lastValueA = new None<A>();
        private Optional<B> lastValueB = new None<B>();

        private void PushA(A valueA)
        {
            lastValueA = new Some<A>(valueA);

            lastValueB.Get(valueB =>
            {
                Push(valueA, valueB);
            });
        }

        private void PushB(B valueB)
        {
            lastValueB = new Some<B>(valueB);

            lastValueA.Get(valueA =>
            {
                Push(valueA, valueB);
            });
        }

        private void Push(A valueA, B valueB)
        {
            PushToListeners(map(valueA, valueB));
        }
    }



    class StreamFilter<A> : Stream<A>
    {
        public Stream<A> source;
        public Func<A, bool> filter;

        protected override void HasListener()
        {
            source.AddListener(this.Push);
        }

        protected override void DoesntHaveListener()
        {
            source.RemoveListener(this.Push);
        }

        private void Push(A value)
        {
            if (filter(value))
                PushToListeners(value);
        }
    }

    class StreamFold<A, B> : Stream<B>
    {
        public Stream<A> source;
        public Func<B, A, B> fold;
        public B accumulator;

        protected override void HasListener()
        {
            source.AddListener(this.Push);
        }

        protected override void DoesntHaveListener()
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

        protected override void HasListener()
        {
            source.AddListener(this.Push);
        }

        protected override void DoesntHaveListener()
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
    }

    public class StreamFilterMap<A, B> : Stream<B>
    {
        public Stream<A> source;
        public Func<A, Optional<B>> filterMap;

        protected override void HasListener()
        {
            source.AddListener(this.Push);
        }

        protected override void DoesntHaveListener()
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
    }

    public class StreamLazy<A> : Stream<A>
    {
        public Stream<A> source;

        private Optional<A> _lastValue = new None<A>();

        protected override void HasListener()
        {
            source.AddListener(this.Push);
        }

        protected override void DoesntHaveListener()
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
    }


    public class StreamMerge<A> : Stream<A>
    {
        public Stream<A> stream0;
        public Stream<A> stream1;

        protected override void HasListener()
        {
            stream0.AddListener(this.Push0);
            stream1.AddListener(this.Push1);
        }

        protected override void DoesntHaveListener()
        {
            stream0.RemoveListener(this.Push0);
            stream1.RemoveListener(this.Push1);
        }

        private void Push0(A value)
        {
            PushToListeners(value);
        }

        private void Push1(A value)
        {
            PushToListeners(value);
        }
    }

    // Stream<A>.None. A stream that never yields any value.
    // Useful paired with Stream<A>#AndThen

    public class StreamNone<A> : Stream<A>
    {
        protected override void HasListener()
        {
            //
        }

        protected override void DoesntHaveListener()
        {
            //
        }
    }

    // A stream that yields a single value, to its first listener.

    public class StreamSingleton<A> : Stream<A>
    {
        public A value;

        private bool sent = false;

        protected override void HasListener()
        {
            if (!sent)
            {
                sent = true;
                PushToListeners(value);
            }
        }

        protected override void DoesntHaveListener() { }
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

    protected override void HasListener()
    {
        source.AddListener(PushToListeners);
    }

    protected override void DoesntHaveListener()
    {
        source.RemoveListener(PushToListeners);
    }
}
