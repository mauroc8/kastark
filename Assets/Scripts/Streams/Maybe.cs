using System;
using System.Collections;

public interface Maybe<A>
{
    Maybe<B> Map<B>(Func<A, B> map);

    Maybe<A> Filter(Func<A, bool> filter);

    Maybe<B> AndThen<B>(Func<A, Maybe<B>> bind);

    A WithDefault(A def);

    Maybe<A> OrThen(Maybe<A> other);

    bool ToBool();

    void Get(Action<A> callback);
}


public static class Maybe
{
    public static Maybe<A> FromNullable<A>(A val)
    {
        if (val == null)
            return new Nothing<A>();
        else
            return new Just<A>(val);
    }

    public static Maybe<B> FromCast<A, B>(A val) where B : A
    {
        if (_.isTypeOf<A, B>(val))
            return new Just<B>((B)val);
        else
            return new Nothing<B>();
    }

    public static Maybe<C> Map2<A, B, C>(Maybe<A> m, Maybe<B> n, Func<A, B, C> f)
    {
        return
            m
                .AndThen(
                    a =>
                        n.Map(b => f(a, b))
                );
    }
}

public struct Just<A> : Maybe<A>
{
    private A _value;

    public A Value => _value;

    public Just(A value)
        =>
            this._value = value;

    public A WithDefault(A _)
        =>
            _value;

    public Maybe<A> OrThen(Maybe<A> _)
        =>
            this;

    public Maybe<B> Map<B>(Func<A, B> map)
        =>
            new Just<B>(map(_value));

    public Maybe<A> Filter(Func<A, bool> filter)
    {
        if (filter(_value))
            return this;
        else
            return new Nothing<A>();
    }

    public Maybe<B> AndThen<B>(Func<A, Maybe<B>> bind)
        =>
            bind(_value);

    public bool ToBool()
        =>
            true;

    public void Get(Action<A> callback)
        =>
            callback(_value);
}

public struct Nothing<A> : Maybe<A>
{
    public Maybe<B> AndThen<B>(Func<A, Maybe<B>> bind)
        => new Nothing<B>();

    public Maybe<B> Map<B>(Func<A, B> map)
        => new Nothing<B>();

    public Maybe<A> Filter(Func<A, bool> filter)
        => this;

    public A WithDefault(A defaultValue)
        => defaultValue;

    public Maybe<A> OrThen(Maybe<A> other)
        => other;

    public bool ToBool()
        => false;

    public void Get(Action<A> _)
    { }
}

