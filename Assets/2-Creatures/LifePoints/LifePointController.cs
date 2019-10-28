using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class LifePointHitEvent : UnityEvent<GameObject> { }

public class LifePointController : MonoBehaviour
{
    [Header("Event Handlers")]
    [SerializeField] LifePointHitEvent _lifePointHitEvent;

    bool _isHit = false;

    public bool IsAlive => !_isHit;

    public void GetsHit()
    {
        if (_isHit) return;
        _isHit = true;

        _lifePointHitEvent.Invoke(this.gameObject);
    }

    // This should be in a different behaviour.

    [Header("Color")]
    public MultiAlphaController alphaController;
}
