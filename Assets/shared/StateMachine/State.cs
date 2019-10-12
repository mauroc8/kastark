using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public abstract void InitState();
    public abstract void UpdateState(float dt);
    public abstract void ExitState();
}
