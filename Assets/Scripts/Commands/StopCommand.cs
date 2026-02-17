using UnityEngine;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "StopAction", menuName = "Units/Commands/Stop")]
    public class StopCommand : BaseCommand
    {
        public override bool CanHandle(ref CommandContext commandContext, bool skipCondition = false)
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
