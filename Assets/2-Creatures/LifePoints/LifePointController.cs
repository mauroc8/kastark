using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePointController : MonoBehaviour
{
    [NonSerialized]
    public float percentage;
    [NonSerialized]
    public LifePointManager lifePointManager;

    [Header("Creature")]
    [SerializeField] Creature _creature;


    [Header("Color")]
    [SerializeField] MultiAlphaController _alphaController;

    bool _isHit = false;

    public void GetsHit()
    {
        if (_isHit) return;
        
        _isHit = true;
        
        _creature.controller.ReceiveAttack(1);

        lifePointManager.LifePointGotHit(this);
    }

    public void SetTarget(Vector3 target)
    {
        transform.localPosition = target;
    }

    public void FadeIn()
    {
        _alphaController.FadeIn(0.3f, 2);
    }

    public void FadeOut()
    {
        _alphaController.FadeOut(0.3f, 2);
    }
}
