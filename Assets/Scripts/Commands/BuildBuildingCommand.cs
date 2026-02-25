using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameDevTV.RTS.Player;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "BuildBuilding", menuName = "Units/Commands/BuildBuilding", order = 105)]
    public class BuildBuildingCommand : BaseCommand
    {
        [field: SerializeField] public BuildingSO buildingSO { get; private set; }
        [field: SerializeField] public List<BuildingRestrictionSO> buildingRestrictions { get; private set; } = new();
        [SerializeField] private LayerMask floorLayers;
        [SerializeField] private LayerMask selectableLayers;

        public override bool CanHandle(ref CommandContext commandContext, bool skipCondition = false)
        {
            if (commandContext.commandable is not IBuildingBuilder buildingBuilder) { return false; }

            bool isSelectable = Physics.Raycast(commandContext.cameraRay, out RaycastHit unitHit, float.MaxValue, selectableLayers);
            commandContext.hit = unitHit;

            if (isSelectable && IsResumable(unitHit))
            {
                bool isBuilderAvailable = !buildingBuilder.IsBuilding;
                return isBuilderAvailable;
            }
            
            bool isFloor = Physics.Raycast(commandContext.cameraRay, out RaycastHit floorHit, float.MaxValue, floorLayers);
            commandContext.hit = floorHit; 
            
            // These final checks down here (instead of early return) to allow commandContext.hit to be populated correctly
            if (!Supplies.HasEnoughSuppliesToBuild(buildingSO)) { return false; }
            if (buildingBuilder.IsBuilding) { return false; }
            
            if (skipCondition || !isFloor) { return false; }
            return buildingRestrictions.All(restriction => restriction.CanPlace(floorHit.point));
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
                if (!Supplies.HasEnoughSuppliesToBuild(buildingSO)) { return; }
                
                if (buildingRestrictions.Any(restriction => !restriction.CanPlace(commandContext.hit.point))) { return; }
                builder.Build(buildingSO, commandContext.hit.point);
            }
        }

        public override bool IsLocked(CommandContext commandContext) => !Supplies.HasEnoughSuppliesToBuild(buildingSO);

        private bool IsResumable(RaycastHit unitHit)
        {
            if (!unitHit.collider.TryGetComponent(out BaseBuilding baseBuilding) || baseBuilding.GetBuildingSO() != buildingSO) { return false; }
            
            BuildingProgress buildingProgress = baseBuilding.GetBuildingProgress();
            return buildingProgress.state is BuildingProgress.BuildingState.Paused or BuildingProgress.BuildingState.Destroyed;
        }
    }
}
