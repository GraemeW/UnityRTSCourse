using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "BuildAction", menuName = "Buildings/Commands/Build", order = 120)]
    public class BuildCommand : ActionBase
    {
        [SerializeField] private UnitSO unitSO;

        public override bool CanHandle(ref CommandContext commandContext)
        {
            bool hasUnitConfigured = unitSO != null && unitSO.prefab != null;
            bool canSpawnUnit = false;
            if (commandContext.commandable is BaseBuilding baseBuilding)
            {
                canSpawnUnit = baseBuilding.spawnLocation != null;
            }

            return hasUnitConfigured && canSpawnUnit;
        }

        public override void Handle(CommandContext commandContext)
        {
            BaseBuilding baseBuilding = (BaseBuilding)commandContext.commandable;
            baseBuilding.BuildUnit(unitSO);
        }
    }
}
