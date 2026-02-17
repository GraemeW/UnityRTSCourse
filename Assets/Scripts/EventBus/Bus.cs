using System;
using System.Collections.Generic;
using GameDevTV.RTS.Events;

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

    public static class Bus
    {
        public static void PrintAllEvents()
        {
            foreach (EventType eventType in Enum.GetValues(typeof(EventType)))
            {
                PrintEvents(eventType);
            }
        }

        public static void DeleteAllEvents()
        {
            foreach (EventType eventType in Enum.GetValues(typeof(EventType)))
            {
                DeleteEvents(eventType);
            }
        }   

        public static void PrintEvents(EventType eventType)
        {
            switch (eventType)
            {
                case EventType.ActionSelected:
                    Bus<ActionSelectedEvent>.PrintAllEvents();
                    break;
                case EventType.Supply:
                    Bus<SupplyEvent>.PrintAllEvents();
                    break;
                case EventType.UnitSelected:
                    Bus<UnitSelectedEvent>.PrintAllEvents();
                    break;
                case EventType.UnitDeselected:
                    Bus<UnitDeselectedEvent>.PrintAllEvents();
                    break;
                case EventType.UnitSpawn:
                    Bus<UnitSpawnEvent>.PrintAllEvents();
                    break;
                case EventType.UnitDeath:
                    Bus<UnitDeathEvent>.PrintAllEvents();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
            }
        }

        public static void DeleteEvents(EventType eventType)
        {
            switch (eventType)
            {
                case EventType.ActionSelected:
                    Bus<ActionSelectedEvent>.ClearAllSubscriptions();
                    break;
                case EventType.Supply:
                    Bus<SupplyEvent>.ClearAllSubscriptions();
                    break;
                case EventType.UnitSelected:
                    Bus<UnitSelectedEvent>.ClearAllSubscriptions();
                    break;
                case EventType.UnitDeselected:
                    Bus<UnitDeselectedEvent>.ClearAllSubscriptions();
                    break;
                case EventType.UnitSpawn:
                    Bus<UnitSpawnEvent>.ClearAllSubscriptions();
                    break;
                case EventType.UnitDeath:
                    Bus<UnitDeathEvent>.ClearAllSubscriptions();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
            }
        }


    }
}
