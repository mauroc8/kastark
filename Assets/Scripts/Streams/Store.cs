using System;


public class Store<State, Msg>
{
    State value;
    Func<State, Msg, State> update;

    public Store(State initial, Func<State, Msg, State> update)
    {
        this.value = initial;
        this.update = update;
    }

    public void Push(Msg msg)
    {
        this.value = this.update(value, msg);
    }

    public void Effect(Action<State, Action<Msg>> effect)
    {
        effect(value, Push);
    }
}
