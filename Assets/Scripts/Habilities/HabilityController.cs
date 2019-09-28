using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HabilityController : MonoBehaviour
{
    protected bool _cast = false;
    
    private float _difficulty;
    public float Difficulty => _difficulty;

    void Awake()
    {
        _difficulty = Global.selectedHability.Difficulty;
    }
}
