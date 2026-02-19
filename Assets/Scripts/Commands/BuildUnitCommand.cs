using UnityEngine;
using GameDevTV.RTS.Player;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "BuildAction", menuName = "Buildings/Commands/Build", order = 120)]
    public class BuildUnitCommand : BaseCommand
    {
        [field: SerializeField] public AbstractUnitSO unitSO { get; private set; }

        public override bool CanHandle(ref CommandContext commandContext, bool skipCondition = false)
        {
            bool hasUnitConfigured = unitSO != null && unitSO.prefab != null;
            if (!hasUnitConfigured) { return false; }

            if (!Supplies.HasEnoughSuppliesToBuild(unitSO)) { return false; }
            
            bool canSpawnUnit = false;
            if (commandContext.commandable is BaseBuilding baseBuilding)
            {
                canSpawnUnit = baseBuilding.spawnLocation != null;
            }

            return canSpawnUnit;
        }

        public override void Handle(CommandContext commandContext)
        {
            if (!Supplies.HasEnoughSuppliesToBuild(unitSO)) { return; }
            
            BaseBuilding baseBuilding = (BaseBuilding)commandContext.commandable;
            baseBuilding.BuildUnit(unitSO);
        }

        public override bool IsLocked(CommandContext commandContext) => !Supplies.HasEnoughSuppliesToBuild(unitSO);
    }
}
