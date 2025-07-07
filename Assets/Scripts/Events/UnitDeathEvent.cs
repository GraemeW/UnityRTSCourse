using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.Events
{
    public struct UnitDeathEvent : IEvent
    {
        public AbstractUnit unit { get; private set; }

        public UnitDeathEvent(AbstractUnit unit)
        {
            this.unit = unit;
        }
    }
}
