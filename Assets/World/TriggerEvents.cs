using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvents : MonoBehaviour
{
    private EventStream<Collider> _triggerEnterEvent = new EventStream<Collider>();
    private EventStream<Collider> _triggerExitEvent = new EventStream<Collider>();

    public Stream<Collider> TriggerEnter => _triggerEnterEvent;
    public Stream<Collider> TriggerExit => _triggerExitEvent;

    void OnTriggerEnter(Collider collider)
    {
        _triggerEnterEvent.Push(collider);
    }

    void OnTriggerExit(Collider collider)
    {
        _triggerExitEvent.Push(collider);
    }
}
