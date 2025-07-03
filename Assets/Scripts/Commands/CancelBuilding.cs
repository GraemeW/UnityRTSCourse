using GameDevTV.RTS.Commands;
using GameDevTV.RTS.UI;
using UnityEngine;

namespace GameDevTV.RTS.Units
{
    [CreateAssetMenu(fileName = "CancelBuilding", menuName = "Units/Commands/CancelBuilding")]
    public class CancelBuilding : ActionBase
    {
        public override bool CanHandle(ref CommandContext commandContext)
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
    }
}
