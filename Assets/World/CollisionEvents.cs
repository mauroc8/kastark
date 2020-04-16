using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEvents : MonoBehaviour
{
    private StreamSource<Collision> _collisionEnterEvent = new StreamSource<Collision>();
    private StreamSource<Collision> _colliionExitEvent = new StreamSource<Collision>();

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
