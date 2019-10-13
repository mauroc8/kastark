using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePointBehaviour : MonoBehaviour
{
    [System.NonSerialized] public float percentage;

    [Header("Creature")]
    [SerializeField] Creature _creature = null;

    public void GetsHit()
    {
        _creature.controller.ReceiveAttack(1);
        Destroy(gameObject);
    }

    [Header("Movement")]
    [SerializeField] float _maxMovementSpeed = 30;

    Vector3 _target;

    public void SetTarget(Vector3 target)
    {
        _target = target;
    }

    void Update()
    {
        var pos = transform.localPosition;
        transform.localPosition = Vector3.MoveTowards(pos, _target, _maxMovementSpeed * Time.deltaTime);
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
