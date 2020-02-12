using System;

public class LazyProc<T>
{
    Action<T> proc;

    public LazyProc(Action<T> proc)
    {
        this.proc = proc;
    }

    bool hasBeenCalled = false;
    T lastUsedArg = default(T);

    public void Call(T arg)
    {
        if (hasBeenCalled &&
            _.eq(arg, lastUsedArg))
        {
            return;
        }
        else
        {
            hasBeenCalled = true;
            lastUsedArg = arg;

            proc(arg);
        }
    }
}

public class LazyProc<A, B>
{
    Action<A, B> proc;

    public LazyProc(Action<A, B> proc)
    {
        this.proc = proc;
    }

    bool hasBeenCalled = false;
    A lastUsedArg0 = default(A);
    B lastUsedArg1 = default(B);

    public void Call(A arg0, B arg1)
    {
        if (hasBeenCalled &&
            _.eq(arg0, lastUsedArg0) &&
            _.eq(arg1, lastUsedArg1)
            )
        {
            return;
        }
        else
        {
            hasBeenCalled = true;
            lastUsedArg0 = arg0;
            lastUsedArg1 = arg1;

            proc(arg0, arg1);
        }
    }
}


public class LazyFunc<A, T>
{
    Func<A, T> func;

    public LazyFunc(Func<A, T> func)
    {
        this.func = func;
    }

    bool hasBeenCalled = false;
    A lastUsedArg = default(A);
    T lastReturnValue = default(T);

    public T Call(A arg)
    {
        if (hasBeenCalled &&
            _.eq(arg, lastUsedArg))
        {
            return lastReturnValue;
        }
        else
        {
            hasBeenCalled = true;
            lastUsedArg = arg;

            return lastReturnValue = func(arg);
        }
    }
}

public class LazyFunc<A, B, T>
{
    Func<A, B, T> func;

    public LazyFunc(Func<A, B, T> func)
    {
        this.func = func;
    }

    bool hasBeenCalled = false;
    A lastUsedArg0 = default(A);
    B lastUsedArg1 = default(B);
    T lastReturnValue = default(T);

    public T Call(A arg0, B arg1)
    {
        if (hasBeenCalled &&
            _.eq(arg0, lastUsedArg0) &&
            _.eq(arg1, lastUsedArg1)
            )
        {
            return lastReturnValue;
        }
        else
        {
            hasBeenCalled = true;
            lastUsedArg0 = arg0;
            lastUsedArg1 = arg1;

            return lastReturnValue = func(arg0, arg1);
        }
    }
}

