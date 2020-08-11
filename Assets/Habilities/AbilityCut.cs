using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCut : MonoBehaviour
{
    enum State
    {
        Initial,
        Broken
    };

    StateStream<State> state =
        new StateStream<State>(State.Initial);

    void Awake()
    {
        var image =
            Query
                .From(this)
                .Get<Image>();

        var initialAlpha =
            image.color.a;

        state
            .Get(value =>
            {
                if (value == State.Initial)
                    image.color =
                        new Color(image.color.r, image.color.g, image.color.b, initialAlpha);
                else
                    image.color =
                        new Color(image.color.r, image.color.g, image.color.b, 0.1f);
            });
    }

    public void Filled()
    {
        if (state.Value != State.Initial)
            state.Value =
                State.Initial;
    }

    public void Empty()
    {
        if (state.Value != State.Broken)
            state.Value =
                State.Broken;
    }
}
