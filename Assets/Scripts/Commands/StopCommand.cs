using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "StopAction", menuName = "AI/Commands/Stop")]
    public class StopCommand : ActionBase
    {
        public override bool CanHandle(ref CommandContext commandContext)
        {
            return (commandContext.commandable is AbstractUnit);
        }

        public override void Handle(CommandContext commandContext)
        {
            AbstractUnit abstractUnit = (AbstractUnit)commandContext.commandable;
            abstractUnit.Stop();
        }
    }
}
