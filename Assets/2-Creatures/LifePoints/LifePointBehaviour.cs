using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePointBehaviour : MonoBehaviour
{
    [System.NonSerialized] public float percentage;

    [Header("Creature")]
    [SerializeField] Creature _creature = null;

    bool _isHit = false;

    public void GetsHit()
    {
        if (_isHit) return;
        
        _isHit = true;
        
        _creature.controller.ReceiveAttack(1);
        Destroy(gameObject);
    }

    public void SetTarget(Vector3 target)
    {
        transform.localPosition = target;
    }

    [Header("Color")]
    [SerializeField] FadeInController _fadeInController = null;
    [SerializeField] FadeOutController _fadeOutController = null;

    public void FadeIn()
    {
        _fadeInController.FadeIn();
    }

    public void FadeOut()
    {
        _fadeOutController.FadeOut();
    }
}
