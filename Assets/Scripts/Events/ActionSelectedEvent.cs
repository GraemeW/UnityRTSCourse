using GameDevTV.RTS.Commands;
using GameDevTV.RTS.EventBus;

namespace GameDevTV.RTS.Events
{
    public struct ActionSelectedEvent : IEvent
    {
        public EventType EventType => EventType.ActionSelected;

        public ActionBase action { get; private set; }

        public ActionSelectedEvent(ActionBase action)
        {
            this.action = action;
        }
    }
}
