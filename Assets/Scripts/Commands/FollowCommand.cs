using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "Follow", menuName = "Units/Commands/Follow")]
    public class FollowCommand : ActionBase
    {
        [SerializeField] private LayerMask selectableLayers;

        public override bool CanHandle(ref CommandContext commandContext, bool skipCondition = false)
        {
            bool canMove = commandContext.commandable is IMoveable;
            bool canFollow = Physics.Raycast(commandContext.cameraRay, out RaycastHit hit, float.MaxValue, selectableLayers)
                && hit.collider.TryGetComponent(out ISelectable selectable);
            commandContext.hit = hit;

            return canMove && canFollow;
        }

        public override void Handle(CommandContext commandContext)
        {
            IMoveable moveable = (IMoveable)commandContext.commandable;
            moveable.SetMoveTarget(commandContext.hit.collider.gameObject);
        }
    }
}