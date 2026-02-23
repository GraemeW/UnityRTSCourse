using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.Events
{
    public struct BuildingSpawnEvent : IEvent
    {
        public EventType eventType => EventType.UnitSpawn;

        public BaseBuilding baseBuilding { get; private set; }

        public BuildingSpawnEvent(BaseBuilding baseBuilding)
        {
            this.baseBuilding = baseBuilding;
        }
    }
}
