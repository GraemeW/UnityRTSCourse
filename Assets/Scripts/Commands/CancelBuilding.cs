using UnityEngine;
using GameDevTV.RTS.Commands;

namespace GameDevTV.RTS.Units
{
    [CreateAssetMenu(fileName = "CancelBuilding", menuName = "Units/Commands/CancelBuilding")]
    public class CancelBuilding : BaseCommand
    {
        public override bool CanHandle(ref CommandContext commandContext, bool skipCondition = false)
        {
            IBuildingBuilder buildingBuilder = commandContext.commandable as IBuildingBuilder;
            bool isWorker = buildingBuilder != null;
            if (!isWorker) { return false; }

            bool isBuilding = buildingBuilder.IsBuilding;
            return isBuilding;
        }

        public override void Handle(CommandContext commandContext)
        {
            IBuildingBuilder buildingBuilder = (IBuildingBuilder)commandContext.commandable;
            buildingBuilder.CancelBuilding();
        }

        public override bool IsLocked(CommandContext commandContext) => false;
    }
}
