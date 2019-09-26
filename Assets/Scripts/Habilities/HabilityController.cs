using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HabilityController : MonoBehaviour
{
    protected bool _cast = false;
    protected float _difficulty;

    public float Difficulty => _difficulty;

    void Start()
    {
        _difficulty = GameState.selectedHability.Difficulty;
    }
}
