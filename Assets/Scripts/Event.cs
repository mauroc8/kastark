using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Kastark/Event")]
public class Event : ScriptableObject
{
    public delegate void EventListener();

    List<EventListener> _listeners = new List<EventListener>();

    public void AddListener(EventListener listener) =>
        _listeners.Add(listener);

    public void RemoveListener(EventListener listener) =>
        _listeners.Remove(listener);

    public void Invoke()
    {
        _listeners.ForEach(listener => listener());
    }
}

