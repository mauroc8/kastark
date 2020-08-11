using System;

/// <summary>
/// 
/// <code>
/// </code>
/// </summary>
public class StateStream<A> : Stream<A>
{
    A _value;

    public A Value
    {
        get { return _value; }
        set
        {
            this._value = value;

            PushToListeners(value);
        }
    }

    public StateStream(A initialValue)
    {
        this._value = initialValue;
    }

    protected override void Awake() { }
    protected override void Sleep() { }

    public override Optional<A> lastValue =>
        Optional.Some(_value);
}
