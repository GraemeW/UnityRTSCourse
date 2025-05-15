using GameDevTV.RTS.Commands;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.Events
{
    public struct ActionSelectedEvent : IEvent
    {
        public ActionBase action { get; private set; }

        public ActionSelectedEvent(ActionBase action)
        {
            this.action = action;
        }
    }
}
