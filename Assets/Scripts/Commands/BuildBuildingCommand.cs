using System.Collections.Generic;
using System.Linq;
using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "BuildBuildingAction", menuName = "Units/Commands/BuildBuilding", order = 105)]
    public class BuildBuildingCommand : ActionBase
    {
        [field: SerializeField] public BuildingSO buildingSO { get; private set; }
        [field: SerializeField] public List<BuildingRestrictionSO> buildingRestrictions { get; private set; } = new();
        [SerializeField] private LayerMask floorLayers;
        [SerializeField] private LayerMask selectableLayers;

        public override bool CanHandle(ref CommandContext commandContext, bool skipCondition = false)
        {
            bool isBuilder = commandContext.commandable is IBuildingBuilder;
            if (!isBuilder) { return false; }

            bool isSelectable = Physics.Raycast(commandContext.cameraRay, out RaycastHit unitHit, float.MaxValue, selectableLayers);
            if (isSelectable && unitHit.collider.TryGetComponent(out BaseBuilding baseBuilding))
            {
                if (baseBuilding.GetBuildingSO() == buildingSO)
                {
                    BuildingProgress buildingProgress = baseBuilding.GetBuildingProgress();
                    
                    if (buildingProgress.state is BuildingProgress.BuildingState.Paused or BuildingProgress.BuildingState.Destroyed)
                    {
                        commandContext.hit = unitHit;
                        return true;
                    }
                }
            }

            bool isFloor = Physics.Raycast(commandContext.cameraRay, out RaycastHit floorHit, float.MaxValue, floorLayers);
            if (skipCondition || !isFloor) { return false; }
            if (buildingRestrictions.Any(restriction => !restriction.CanPlace(floorHit.point))) { return false; }
            
            commandContext.hit = floorHit; 
            return true;
        }

        public override void Handle(CommandContext commandContext)
        {
            IBuildingBuilder builder = (IBuildingBuilder)commandContext.commandable;
            if (commandContext.hit.collider.TryGetComponent(out BaseBuilding baseBuilding))
            {
                builder.ResumeBuilding(baseBuilding);
            }
            else
            {
                if (buildingRestrictions.Any(restriction => !restriction.CanPlace(commandContext.hit.point))) { return; }
                builder.Build(buildingSO, commandContext.hit.point);
            }
        }
    }
}
