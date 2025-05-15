using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "FollowAction", menuName = "AI/Actions/Follow")]
    public class FollowCommand : ActionBase
    {
        [SerializeField] private LayerMask selectableLayers;

        public override bool CanHandle(AbstractCommandable commandable, Ray cameraRay, out RaycastHit hit)
        {
            bool canMove = commandable is IMoveable;
            bool canFollow = Physics.Raycast(cameraRay, out hit, float.MaxValue, selectableLayers)
                && hit.collider.TryGetComponent(out ISelectable selectable);

            return canMove && canFollow;
        }

        public override void Handle(AbstractCommandable commandable, RaycastHit hit)
        {
            IMoveable moveable = (IMoveable)commandable;
            moveable.SetMoveTarget(hit.transform);
        }
    }
}