using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalEvents
{
    public class GlobalEvent { }

    public static class EventController
    {
        static Dictionary<System.Type, Action<GlobalEvent>> _listeners = new Dictionary<System.Type, Action<GlobalEvent>>();

        static Dictionary<System.Delegate, Action<GlobalEvent>> _delegateLookup =
            new Dictionary<System.Delegate, Action<GlobalEvent>>();

        public static void AddListener<T>(Action<T> listener) where T : GlobalEvent
        {
            if (_delegateLookup.ContainsKey(listener)) return;

            //Create a generic delegate from non-generic param.
            Action<GlobalEvent> genericListener = (eventData) => listener((T)eventData);
            _delegateLookup[listener] = genericListener;
            Action<GlobalEvent> tempListener;

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

        public static void RemoveListener<T>(Action<T> listener) where T : GlobalEvent
        {
            Action<GlobalEvent> genericListener;
            if (!_delegateLookup.TryGetValue(listener, out genericListener)) return;

            Action<GlobalEvent> tempListener;

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

        public static void TriggerEvent(GlobalEvent evt)
        {
            Action<GlobalEvent> listener;
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


