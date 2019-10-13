using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface State
{
    void InitState();
    void UpdateState(float dt);
    void ExitState();
}
