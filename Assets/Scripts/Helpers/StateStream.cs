
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
        set { Push(value); }
    }

    public StateStream(A initialValue)
    {
        this._value = initialValue;
    }

    protected override void HasListener() { }
    protected override void DoesntHaveListener() { }

    public void Push(A value)
    {
        this._value = value;

        PushToListeners(value);
    }
}
