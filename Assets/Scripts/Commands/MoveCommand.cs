using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "Move", menuName = "AI/Commands/Move")]
    public class MoveCommand : ActionBase
    {
        // Tunables
        [SerializeField] private LayerMask floorLayers;
        [SerializeField] private bool isComplexMoveBehaviour = true;
        [SerializeField] private float complexMoveRadiusExpansion = 3.5f;

        // State
        private int unitsOnLayer = 0;
        private int maxUnitsOnLayer = 1;
        private float circleRadius = 0;
        private float radialOffset = 0;

        public override bool CanHandle(ref CommandContext commandContext)
        {
            bool canMove = commandContext.commandable is IMoveable;
            bool isFloor = Physics.Raycast(commandContext.cameraRay, out RaycastHit hit, float.MaxValue, floorLayers);
            commandContext.hit = hit;

            return canMove && isFloor;
        }

        public override void Handle(CommandContext commandContext)
        {
            IMoveable moveable = (IMoveable)commandContext.commandable;

            AbstractUnit abstractUnit = (AbstractUnit)commandContext.commandable;
            if (!isComplexMoveBehaviour || abstractUnit == null) { moveable.MoveTo(commandContext.hit.point); }

            ComplexRadialMove(commandContext, moveable, abstractUnit);
        }

        private void ComplexRadialMove(CommandContext commandContext, IMoveable moveable, AbstractUnit abstractUnit)
        {
            if (commandContext.unitIndex == 0)
            {
                unitsOnLayer = 0;
                maxUnitsOnLayer = 1;
                circleRadius = 0;
                radialOffset = 0;
            }

            Vector3 targetPosition = new Vector3(
                commandContext.hit.point.x + circleRadius * Mathf.Cos(radialOffset * unitsOnLayer),
                commandContext.hit.point.y,
                commandContext.hit.point.z + circleRadius * Mathf.Sin(radialOffset * unitsOnLayer)
                );

            moveable.MoveTo(targetPosition);
            unitsOnLayer++;

            if (unitsOnLayer >= maxUnitsOnLayer)
            {
                unitsOnLayer = 0;
                circleRadius += abstractUnit.agentRadius * complexMoveRadiusExpansion;
                maxUnitsOnLayer = Mathf.FloorToInt(2 * Mathf.PI * circleRadius / (abstractUnit.agentRadius * 2));
                radialOffset = 2 * Mathf.PI / maxUnitsOnLayer;
            }
        }
    }
}
