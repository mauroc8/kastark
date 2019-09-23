using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HabilityController : MonoBehaviour
{
    [System.NonSerialized] public float difficulty = 1;
    protected bool _cast = false;

    void Start()
    {
        difficulty = GameState.selectedHability.Difficulty;
    }
}
