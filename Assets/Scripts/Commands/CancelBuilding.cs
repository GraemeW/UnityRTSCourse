using UnityEngine;
using GameDevTV.RTS.Commands;

namespace GameDevTV.RTS.Units
{
    [CreateAssetMenu(fileName = "CancelBuilding", menuName = "Units/Commands/CancelBuilding")]
    public class CancelBuilding : BaseCommand
    {
        public override bool CanHandle(ref CommandContext commandContext, bool skipCondition = false)
        {
            return commandContext.commandable is IBuildingBuilder { IsBuilding: true };
        }

        public override void Handle(CommandContext commandContext)
        {
            IBuildingBuilder buildingBuilder = (IBuildingBuilder)commandContext.commandable;
            buildingBuilder.CancelBuilding();
        }

        public override bool IsLocked(CommandContext commandContext) => false;
    }
}
