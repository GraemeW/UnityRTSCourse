using GameDevTV.RTS.Environment;
using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "GatherAction", menuName = "AI/Commands/Gather", order = 105)]
    public class GatherCommand : ActionBase
    {
        [SerializeField] private LayerMask selectableLayers;

        public override bool CanHandle(ref CommandContext commandContext)
        {
            bool isWorker = commandContext.commandable is Worker;
            bool validSupply = Physics.Raycast(commandContext.cameraRay, out RaycastHit hit, float.MaxValue, selectableLayers)
                &&  (hit.collider.TryGetComponent(out GatherableSupply _));
            commandContext.hit = hit;

            return isWorker && validSupply;
        }

        public override void Handle(CommandContext commandContext)
        {
            Worker worker = (Worker)commandContext.commandable;
            GatherableSupply gatherableSupply = commandContext.hit.collider.GetComponent<GatherableSupply>();
            worker.Gather(gatherableSupply);
        }
    }
}
