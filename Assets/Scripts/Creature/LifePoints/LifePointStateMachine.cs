using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePointStateMachine : StateMachine
{
    [SerializeField] ColorController _lifePointColorController;
    
    LifePointIdleState _lifePointIdleState;

    void Start()
    {
        var index = transform.GetSiblingIndex();
        var lifePointCount = transform.parent.childCount;

        _lifePointIdleState = new LifePointIdleState{
            transform = transform,
            index = index,
            lifePointsCount = lifePointCount
        };
    }

}
