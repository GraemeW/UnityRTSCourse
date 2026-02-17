namespace GameDevTV.RTS.EventBus
{
    public interface IEvent
    {
        public EventType eventType { get; }
    }
}
