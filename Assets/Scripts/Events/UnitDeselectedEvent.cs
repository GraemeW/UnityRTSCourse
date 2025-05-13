using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.Events
{
    public struct UnitDeselectedEvent : IEvent
    {
        public ISelectable unit { get; private set; }

        public UnitDeselectedEvent(ISelectable unit)
        {
            this.unit = unit;
        }
    }
}
