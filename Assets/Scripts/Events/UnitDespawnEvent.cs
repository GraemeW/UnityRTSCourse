using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.Events
{
    public struct UnitDespawnEvent : IEvent
    {
        public AbstractUnit unit { get; private set; }

        public UnitDespawnEvent(AbstractUnit unit)
        {
            this.unit = unit;
        }
    }
}
