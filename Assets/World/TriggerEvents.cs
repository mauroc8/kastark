using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvents : MonoBehaviour
{
    private StreamSource<Collider> _triggerEnterEvent = new StreamSource<Collider>();
    private StreamSource<Collider> _triggerExitEvent = new StreamSource<Collider>();

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
