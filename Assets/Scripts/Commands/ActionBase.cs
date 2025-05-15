using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    public abstract class ActionBase : ScriptableObject, ICommand
    {
        [field: SerializeField] public Sprite icon { get; private set; }
        [field: Range(0,8)][field: SerializeField] public int slot { get; private set; }
        [field: SerializeField] public bool requiresClickToActivate { get; private set; } = true;
        public abstract bool CanHandle(ref CommandContext commandContext);
        public abstract void Handle(CommandContext commandContext);
    }
}
