using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.Events
{
    public struct UnitSpawnEvent : IEvent
    {
        public EventType eventType => EventType.UnitSpawn;

        public AbstractUnit unit { get; private set; }

        public UnitSpawnEvent(AbstractUnit unit)
        {
            this.unit = unit;
        }
    }
}
