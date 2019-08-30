using System.Collections.Generic;
using UnityEngine;

namespace Events {

	public class EventController : MonoBehaviour {

		//Private Members
		private static bool _applicationIsQuitting = false;
		private static EventController _instance;
		private Dictionary<System.Type, EventDelegate> _listeners = new Dictionary<System.Type, EventDelegate>();

		private Dictionary<System.Delegate, EventDelegate> _delegateLookup =
			new Dictionary<System.Delegate, EventDelegate>();

		private delegate void EventDelegate(GameEvent eventData);

		//Public Members
		public delegate void EventDelegate<T>(T eventData) where T : GameEvent;

		public static EventController Instance {
			set { _instance = value; }

			get {

				if (_instance == null) {
					_instance = GameObject.FindObjectOfType<EventController>() as EventController;
					if (_instance == null) {
						_instance = new GameObject().AddComponent<EventController>();
						_instance.name = "EventManager";
						DontDestroyOnLoad(_instance);
					}
				}

				return _instance;
			}
		}


		public static void AddListener<T>(EventDelegate<T> listener) where T : GameEvent {
			if (Instance._delegateLookup.ContainsKey(listener)) return;

			//Create a generic delegate from non-generic param.
			EventDelegate genericListener = (eventData) => listener((T) eventData);
			Instance._delegateLookup[listener] = genericListener;
			EventDelegate tempListener;

			if (Instance._listeners.TryGetValue(typeof(T), out tempListener)) {
				tempListener += genericListener;
				Instance._listeners[typeof(T)] = tempListener;
			}
			else {
				Instance._listeners[typeof(T)] = genericListener;
			}
		}

		public static void RemoveListener<T>(EventDelegate<T> listener) where T : GameEvent {
			if (_applicationIsQuitting) return;

			EventDelegate genericListener;
			if (!Instance._delegateLookup.TryGetValue(listener, out genericListener)) return;

			EventDelegate tempListener;

			if (Instance._listeners.TryGetValue(typeof(T), out tempListener)) {
				if (genericListener != null) {
					tempListener -= genericListener;
				}

				if (tempListener == null) {
					Instance._listeners.Remove(typeof(T));
				}
				else {
					Instance._listeners[typeof(T)] = tempListener;
				}
			}

			Instance._delegateLookup.Remove(listener);
		}

		public static void Clear() {
			Instance._listeners.Clear();
		}

		public static void TriggerEvent(GameEvent evt) {
			EventDelegate listener;
			if (Instance._listeners.TryGetValue(evt.GetType(), out listener)) {
				listener.Invoke(evt);
			}
			else {
				Debug.Log(evt + " has no listeners");
			}
		}


		public void OnDestroy() {
			_applicationIsQuitting = true;
		}
	}
}


