using System.Collections.Generic;

namespace GameDevTV.RTS.EventBus
{
    public static class Bus<T> where T : IEvent
    {
        private static readonly List<Event> _activeHandlers = new();
        private static event Event onEvent;

        public delegate void Event(T args);
        public static void Raise(T evt) => onEvent?.Invoke(evt);

        public static void SubscribeToEvent(Event handler)
        {
            onEvent += handler;
            _activeHandlers.Add(handler);
        }
        public static void UnsubscribeFromEvent(Event handler)
        {
            onEvent -= handler;
            _activeHandlers.Remove(handler);
        }

        public static void ClearAllSubscriptions()
        {
            foreach (Event handler in _activeHandlers)
            {
                onEvent -= handler;
            }
            _activeHandlers.Clear();
            onEvent = null;
        }

        public static void PrintAllEvents()
        {
            foreach (Event handler in _activeHandlers)
            {
                UnityEngine.Debug.Log(handler.Method.Name);
            }
        }
    }
}
