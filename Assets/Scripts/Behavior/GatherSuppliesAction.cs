using GameDevTV.RTS.Environment;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace GameDevTV.RTS.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "GatherSupplies", story: "[Agent] gathers [Amount] supplies from [GatherableSupplies]", category: "Action/Units", id: "7a861e045885ddce5bb53f554687f8f3")]
    public partial class GatherSuppliesAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<int> Amount;
        [SerializeReference] public BlackboardVariable<GatherableSupply> GatherableSupplies;

        private float enterTime;

        protected override Status OnStart()
        {
            if (!GatherableSupplies.Value.isBusy)
            {
                enterTime = Time.time;
                GatherableSupplies.Value.BeginGather();
            }
            
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Time.time > GatherableSupplies.Value.supply.baseGatherTime + enterTime)
            {
                Amount.Value = GatherableSupplies.Value.EndGather();
                return Status.Success;
            }

            return Status.Running;
        }
    }
}
