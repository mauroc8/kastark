using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OthokAnimatorEvents : MonoBehaviour
{
    public EventStream<Void> runEnd =
        new EventStream<Void>();

    void RunEnd()
    {
        runEnd.Push(new Void());
    }
}
