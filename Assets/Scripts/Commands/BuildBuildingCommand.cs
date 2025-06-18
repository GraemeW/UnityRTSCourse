using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "BuildBuildingAction", menuName = "Units/Commands/BuildBuilding", order = 105)]
    public class BuildBuildingCommand : ActionBase
    {
        [field: SerializeField] public BuildingSO buildingSO { get; private set; }
        [SerializeField] private LayerMask floorLayers;

        public override bool CanHandle(ref CommandContext commandContext)
        {
            bool isBuilder = commandContext.commandable is IBuildingBuilder;
            bool isFloor = Physics.Raycast(commandContext.cameraRay, out RaycastHit hit, float.MaxValue, floorLayers);
            commandContext.hit = hit;

            return isBuilder && isFloor;
        }

        public override void Handle(CommandContext commandContext)
        {
            IBuildingBuilder builder = (IBuildingBuilder)commandContext.commandable;
            builder.Build(buildingSO, commandContext.hit.point);
        }
    }
}
