namespace GameDevTV.RTS.EventBus
{
    public interface IEvent
    {
        public EventType EventType { get; }
    }
}
