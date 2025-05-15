using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "MoveAction", menuName = "AI/Actions/Move")]
    public class MoveCommand : ActionBase
    {
        [SerializeField] private LayerMask floorLayers;
        [SerializeField] private bool isComplexMoveBehaviour = true;
        [SerializeField] private float complexMoveRadiusExpansion = 3.5f;

        public override bool CanHandle(AbstractCommandable commandable, Ray cameraRay, out RaycastHit hit)
        {
            bool canMove = commandable is IMoveable;
            bool isFloor = Physics.Raycast(cameraRay, out hit, float.MaxValue, floorLayers);

            return canMove && isFloor;
        }

        public override void Handle(AbstractCommandable commandable, RaycastHit hit)
        {
            IMoveable moveable = (IMoveable)commandable;

            // Simple Move
            bool isAbstractUnit = commandable is AbstractUnit abstractUnit;
            if (!isComplexMoveBehaviour || !isAbstractUnit) { moveable.MoveTo(hit.point); }

            // Temp Dumb Behaviour -- TODO:  Port over complex, need to pass struct w/ data
            moveable.MoveTo(hit.point);
        }
    }
}
