using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    public interface ICommand
    {
        bool CanHandle(ref CommandContext commandContext, bool skipCondition = false);
        void Handle(CommandContext commandContext);
    }
}
