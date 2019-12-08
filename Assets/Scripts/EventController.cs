using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class GameEvent { }

    public static class EventController
    {
        delegate void EventDelegate(GameEvent eventData);

        static Dictionary<System.Type, EventDelegate> _listeners = new Dictionary<System.Type, EventDelegate>();

        static Dictionary<System.Delegate, EventDelegate> _delegateLookup =
            new Dictionary<System.Delegate, EventDelegate>();

        public delegate void EventDelegate<T>(T eventData) where T : GameEvent;

        public static void AddListener<T>(EventDelegate<T> listener) where T : GameEvent
        {
            if (_delegateLookup.ContainsKey(listener)) return;

            //Create a generic delegate from non-generic param.
            EventDelegate genericListener = (eventData) => listener((T)eventData);
            _delegateLookup[listener] = genericListener;
            EventDelegate tempListener;

            if (_listeners.TryGetValue(typeof(T), out tempListener))
            {
                tempListener += genericListener;
                _listeners[typeof(T)] = tempListener;
            }
            else
            {
                _listeners[typeof(T)] = genericListener;
            }
        }

        public static void RemoveListener<T>(EventDelegate<T> listener) where T : GameEvent
        {
            EventDelegate genericListener;
            if (!_delegateLookup.TryGetValue(listener, out genericListener)) return;

            EventDelegate tempListener;

            if (_listeners.TryGetValue(typeof(T), out tempListener))
            {
                if (genericListener != null)
                {
                    tempListener -= genericListener;
                }

                if (tempListener == null)
                {
                    _listeners.Remove(typeof(T));
                }
                else
                {
                    _listeners[typeof(T)] = tempListener;
                }
            }

            _delegateLookup.Remove(listener);
        }

        public static void TriggerEvent(GameEvent evt)
        {
            EventDelegate listener;
            if (_listeners.TryGetValue(evt.GetType(), out listener))
            {
                listener.Invoke(evt);
            }
            else
            {
                Debug.Log(evt + " has no listeners");
            }
        }
    }
}


