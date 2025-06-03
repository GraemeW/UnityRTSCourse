using GameDevTV.RTS.Environment;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "GatherSupplies", story: "[Agent] gathers [Amount] supplies from [Supply]", category: "Action/Units", id: "7a861e045885ddce5bb53f554687f8f3")]
    public partial class GatherSuppliesAction : Action
    {
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<int> Amount;
    [SerializeReference] public BlackboardVariable<GatherableSupply> Supply;
        private float enterTime;

        protected override Status OnStart()
        {
            if (!Agent.Value.TryGetComponent(out Worker worker)) { return Status.Failure; }

            if (!Supply.Value.isBusy)
            {
                enterTime = Time.time;
                Supply.Value.BeginGather();
                return Status.Running;
            }

            return Status.Failure;
        }

        protected override Status OnUpdate()
        {
            if (Time.time > Supply.Value.supplySO.baseGatherTime + enterTime)
            {
                if (Supply.Value == null) { return Status.Failure; } // Safety on destruction due to supplies empty

                Amount.Value = Supply.Value.EndGather();
                return Status.Success;
            }

            return Status.Running;
        }
    }
}
