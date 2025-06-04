using GameDevTV.RTS.Environment;
using GameDevTV.RTS.EventBus;

namespace GameDevTV.RTS.Events
{
    public struct SupplyEvent : IEvent
    {
        public SupplySO supplyType { get; private set; }
        public int amount { get; private set; }

        public SupplyEvent(SupplySO supplyType, int amount)
        {
            this.supplyType = supplyType;
            this.amount = amount;
        }
    }
}