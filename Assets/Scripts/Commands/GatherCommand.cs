using UnityEngine;
using GameDevTV.RTS.Environment;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "GatherAction", menuName = "Units/Commands/Gather", order = 105)]
    public class GatherCommand : BaseCommand
    {
        [SerializeField] private LayerMask selectableLayers;

        public override bool CanHandle(ref CommandContext commandContext, bool skipCondition = false)
        {
            bool isWorker = commandContext.commandable is Worker;
            bool validSupply = Physics.Raycast(commandContext.cameraRay, out RaycastHit hit, float.MaxValue, selectableLayers)
                &&  ((hit.collider.TryGetComponent(out GatherableSupply _) || hit.collider.TryGetComponent(out CommandPost _)));
            commandContext.hit = hit;

            return isWorker && validSupply;
        }

        public override void Handle(CommandContext commandContext)
        {
            Worker worker = (Worker)commandContext.commandable;
            if (commandContext.hit.collider.TryGetComponent(out GatherableSupply gatherableSupply))
            {
                worker.Gather(gatherableSupply);
            }
            else if (commandContext.hit.collider.TryGetComponent(out CommandPost commandPost))
            {
                worker.ReturnSupplies(commandPost);
            }
        }
        
        public override bool IsLocked(CommandContext commandContext) => false;
    }
}
