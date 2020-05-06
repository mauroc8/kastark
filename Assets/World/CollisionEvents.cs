using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEvents : MonoBehaviour
{
    private EventStream<Collision> _collisionEnterEvent = new EventStream<Collision>();
    private EventStream<Collision> _colliionExitEvent = new EventStream<Collision>();

    public Stream<Collision> CollisionEnter => _collisionEnterEvent;
    public Stream<Collision> CollisionExit => _colliionExitEvent;

    void OnCollisionEnter(Collision collision)
    {
        _collisionEnterEvent.Push(collision);
    }

    void OnCollisionExit(Collision collision)
    {
        _colliionExitEvent.Push(collision);
    }
}
