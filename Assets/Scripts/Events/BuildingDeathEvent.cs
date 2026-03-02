using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.Events
{
    public struct BuildingDeathEvent : IEvent
    {
        public EventType eventType => EventType.UnitDeath;
        
        public BaseBuilding building { get; private set; }

        public BuildingDeathEvent(BaseBuilding building)
        {
            this.building = building;
        }
    }
}