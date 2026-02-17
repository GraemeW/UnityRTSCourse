using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.Events
{
    public struct UnitSelectedEvent : IEvent
    {
        public EventType eventType => EventType.UnitSelected;

        public ISelectable unit { get; private set; }

        public UnitSelectedEvent(ISelectable unit)
        {
            this.unit = unit;
        }
    }
}
