using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    public abstract class ActionBase : ScriptableObject, ICommand
    {
        public abstract bool CanHandle(ref CommandContext commandContext);
        public abstract void Handle(CommandContext commandContext);
    }
}
