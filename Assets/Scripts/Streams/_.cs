using System;
using System.Collections;

public static partial class _
{
    public static bool isTypeOf<A, B>(A value) where B : A
        =>
            typeof(B).IsAssignableFrom(value.GetType());

    public static bool eq<A>(A a, A b)
    {
        return
            a.GetType().IsValueType
                ? StructuralComparisons.StructuralEqualityComparer.Equals(a, b)
                : (object)a == (object)b
            ;
    }

    public static T NullCheck<T>(T val)
    {
        UnityEngine.Debug.Assert(val != null);

        return val;
    }


    public static T identity<T>(T val)
        => val;

    public static float sum(float a, float b)
        => a + b;

    public static Action apply<A>(Action<A> f, A a)
        => () => f(a);

    public static Action<B> apply<A, B>(Action<A, B> f, A a)
        => b => f(a, b);

    public static Action<B, C> apply<A, B, C>(Action<A, B, C> f, A a)
        => (b, c) => f(a, b, c);

    public static Action<C> apply<A, B, C>(Action<A, B, C> f, A a, B b)
        => c => f(a, b, c);


    public static Func<B> apply<A, B>(Func<A, B> f, A a)
        => () => f(a);

    public static Func<B, C> apply<A, B, C>(Func<A, B, C> f, A a)
        => b => f(a, b);

    public static Func<B, C, D> apply<A, B, C, D>(Func<A, B, C, D> f, A a)
        => (b, c) => f(a, b, c);

    public static Func<C, D> apply<A, B, C, D>(Func<A, B, C, D> f, A a, B b)
        => c => f(a, b, c);


    public static Action<A> compose<A, B>(Action<B> f, Func<A, B> g)
        => a => f(g(a));

    public static Func<A, C> compose<A, B, C>(Func<B, C> f, Func<A, B> g)
        => a => f(g(a));


    public static Action<A> pipe<A, B>(Func<A, B> f, Action<B> g)
        => _.compose(g, f);

    public static Func<A, C> pipe<A, B, C>(Func<A, B> f, Func<B, C> g)
        => _.compose(g, f);


    public static A log<A>(A value)
    {
        UnityEngine.Debug.Log($"{value}");
        return value;
    }
}
