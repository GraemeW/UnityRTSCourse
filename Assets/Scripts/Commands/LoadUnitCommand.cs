using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "LoadUnit", menuName = "Units/Commands/LoadUnit", order = 106)]
    public class LoadUnitCommand : BaseCommand
    {
        [SerializeField] private LayerMask selectableLayers;
        
        public override bool CanHandle(ref CommandContext commandContext, bool skipCondition = false)
        {
            bool canMove = commandContext.commandable is IMoveable;
            bool canFollow = Physics.Raycast(commandContext.cameraRay, out RaycastHit hit, float.MaxValue, selectableLayers) && hit.collider.TryGetComponent(out ISelectable _);
            bool canLoad = commandContext.commandable is ITransporter && hit.collider.TryGetComponent(out ITransportable _);
            commandContext.hit = hit;

            return canMove && canFollow && canLoad;
        }

        public override void Handle(CommandContext commandContext)
        {
            var transporter = (ITransporter)commandContext.commandable;
            var transportable = commandContext.hit.collider.GetComponent<ITransportable>();
            transporter.Load(transportable);
        }

        public override bool IsLocked(CommandContext commandContext) => false;
    }
}
