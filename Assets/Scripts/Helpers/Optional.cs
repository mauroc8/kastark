using System;
using System.Collections;

public interface Optional<A>
{
    Optional<B> Map<B>(Func<A, B> map);

    Optional<A> Filter(Func<A, bool> filter);

    Optional<B> AndThen<B>(Func<A, Optional<B>> bind);

    A WithDefault(A def);

    Optional<A> OrThen(Optional<A> other);

    bool ToBool();

    void Get(Action<A> callback);

    void CaseOf(Action<A> just, Action nothing);

    B CaseOf<B>(Func<A, B> just, Func<B> nothing);
}


public static class Optional
{
    public static Optional<A> Some<A>(A val)
    {
        return new Some<A>(val);
    }

    public static Optional<A> None<A>()
    {
        return new None<A>();
    }

    public static Optional<A> FromNullable<A>(A val)
    {
        if (val == null)
            return new None<A>();
        else
            return new Some<A>(val);
    }

    public static Optional<B> FromCast<A, B>(A val) where B : A
    {
        if (Functions.IsTypeOf<A, B>(val))
            return new Some<B>((B)val);
        else
            return new None<B>();
    }

    public static Optional<C> Map2<A, B, C>(Optional<A> m, Optional<B> n, Func<A, B, C> f)
    {
        return
            m
                .AndThen(
                    a =>
                        n.Map(b => f(a, b))
                );
    }

    public static bool ToBool<A>(Optional<A> maybe)
    {
        return maybe.ToBool();
    }
}

public struct Some<A> : Optional<A>
{
    private A _value;

    public A Value => _value;

    public Some(A value)
        =>
            this._value = value;

    public A WithDefault(A _)
        =>
            _value;

    public Optional<A> OrThen(Optional<A> _)
        =>
            this;

    public Optional<B> Map<B>(Func<A, B> map)
        =>
            new Some<B>(map(_value));

    public Optional<A> Filter(Func<A, bool> filter)
    {
        if (filter(_value))
            return this;
        else
            return new None<A>();
    }

    public Optional<B> AndThen<B>(Func<A, Optional<B>> bind)
        =>
            bind(_value);

    public bool ToBool()
        =>
            true;

    public void Get(Action<A> callback)
        =>
            callback(_value);

    public void CaseOf(Action<A> just, Action nothing)
        =>
            just(_value);

    public B CaseOf<B>(Func<A, B> just, Func<B> nothing)
        =>
            just(_value);
}

public struct None<A> : Optional<A>
{
    public Optional<B> AndThen<B>(Func<A, Optional<B>> bind)
        => new None<B>();

    public Optional<B> Map<B>(Func<A, B> map)
        => new None<B>();

    public Optional<A> Filter(Func<A, bool> filter)
        => this;

    public A WithDefault(A defaultValue)
        => defaultValue;

    public Optional<A> OrThen(Optional<A> other)
        => other;

    public bool ToBool()
        => false;

    public void Get(Action<A> _)
    { }

    public void CaseOf(Action<A> just, Action nothing)
        =>
            nothing();

    public B CaseOf<B>(Func<A, B> just, Func<B> nothing)
        =>
            nothing();
}

