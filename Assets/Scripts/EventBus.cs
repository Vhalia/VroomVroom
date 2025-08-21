using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public enum EEventType
    {
        RED_LIGHT_VIOLATION,
        GOAL_REACHED,
        GOAL_COMPLETED,
        EXPERIENCE_GAINED
    }

    public class EventData
    {
        private Dictionary<string, object> _data = new Dictionary<string, object>();

        public void Set<T>(string key, T value) => _data[key] = value;
        public T Get<T>(string key) => (T)_data[key];
    }

    public class EventBus : Singleton<EventBus>
    {
        private readonly Dictionary<EEventType, Action> _events = new();
        private readonly Dictionary<EEventType, Action<object>> _eventsWithParams = new();

        public static void Subscribe(EEventType eventType, Action listener)
        {
            if (Instance._events.ContainsKey(eventType))
                Instance._events[eventType] += listener;
            else
                Instance._events[eventType] = listener;
        }

        public static void Subscribe<T>(EEventType eventType, Action<T> listener)
        {
            Action<object> wrapper = (obj) => listener((T)obj);

            if (Instance._eventsWithParams.ContainsKey(eventType))
                Instance._eventsWithParams[eventType] += wrapper;
            else
                Instance._eventsWithParams[eventType] = wrapper;
        }

        public static void Unsubscribe(EEventType eventType, Action listener)
        {
            if (Instance._events.ContainsKey(eventType))
                Instance._events[eventType] -= listener;
        }

        public static void Unsubscribe<T>(EEventType eventType, Action<T> listener)
        {
            if (Instance._eventsWithParams.ContainsKey(eventType))
                Instance._eventsWithParams[eventType] -= (obj) => listener((T)obj);
        }

        public static void TriggerEvent(EEventType eventType)
        {
            if (!Instance._events.ContainsKey(eventType))
            {
                Debug.LogWarning($"Event {eventType} has no subscribers.");
                return;
            }

            Instance._events[eventType]?.Invoke();
        }

        public static void TriggerEvent<T>(EEventType eventType, T parameter)
        {
            if (!Instance._eventsWithParams.ContainsKey(eventType))
            {
                Debug.LogWarning($"Event {eventType} has no subscribers.");
                return;
            }

            Instance._eventsWithParams[eventType]?.Invoke(parameter);
        }
    }

}
