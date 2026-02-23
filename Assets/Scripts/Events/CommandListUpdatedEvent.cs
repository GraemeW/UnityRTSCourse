using System.Collections.Generic;
using System.Linq;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Units;
using GameDevTV.RTS.Commands;

namespace GameDevTV.RTS.Events
{
        public struct CommandListUpdatedEvent : IEvent
        {
            public EventType eventType => EventType.CommandSelected;

            public HashSet<AbstractCommandable> commandables { get; private set; }
            public List<BaseCommand> commandList { get; private set; }

            public CommandListUpdatedEvent(HashSet<AbstractCommandable> commandables, IList<BaseCommand> commandList)
            {
                this.commandables = commandables != null ? commandables.ToHashSet() : new HashSet<AbstractCommandable>();
                this.commandList = commandList != null ? commandList.ToList() : new List<BaseCommand>();
            }

            public CommandListUpdatedEvent(AbstractCommandable commandable, IList<BaseCommand> commandList)
            {
                commandables = new HashSet<AbstractCommandable> { commandable  };
                this.commandList = commandList != null ? commandList.ToList() : new List<BaseCommand>();
            }
        }
}
