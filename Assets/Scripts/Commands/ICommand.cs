using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    public interface ICommand
    {
        bool CanHandle(ref CommandContext commandContext);
        void Handle(CommandContext commandContext);
    }
}
