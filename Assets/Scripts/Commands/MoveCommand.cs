using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "MoveAction", menuName = "AI/Actions/Move")]
    public class MoveCommand : ActionBase
    {
        // Tunables
        [SerializeField] private LayerMask floorLayers;
        [SerializeField] private bool isComplexMoveBehaviour = true;
        [SerializeField] private float complexMoveRadiusExpansion = 3.5f;

        // State
        

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

            // Simple Move
            bool isAbstractUnit = commandContext.commandable is AbstractUnit abstractUnit;
            if (!isComplexMoveBehaviour || !isAbstractUnit) { moveable.MoveTo(commandContext.hit.point); }

            // Temp Dumb Behaviour -- TODO:  Port over complex, need to pass struct w/ data
            moveable.MoveTo(commandContext.hit.point);
        }
    }
}
