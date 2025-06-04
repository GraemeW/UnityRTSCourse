using GameDevTV.RTS.Environment;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using GameDevTV.RTS.Units;
using UnityEngine.AI;
using GameDevTV.RTS.Utilities;

namespace GameDevTV.RTS.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "GatherSupplies", story: "[Agent] gathers [Amount] supplies from [Supply]", category: "Action/Units", id: "7a861e045885ddce5bb53f554687f8f3")]
    public partial class GatherSuppliesAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<int> Amount;
        [SerializeReference] public BlackboardVariable<GatherableSupply> Supply;
        
        // State
        private float enterTime;
        bool thisSupplyMined;

        // Cached References
        private Animator animator;

        protected override Status OnStart()
        {
            if (!Agent.Value.TryGetComponent(out Worker worker)) { return Status.Failure; }
            if (Supply.Value == null) { return Status.Failure; }

            Agent.Value.TryGetComponent(out animator);

            // Check if already has resources on unit -- return them first
            thisSupplyMined = false;
            if (Amount.Value > 0) { return Status.Success; } 

            // Otherwise kick off mining
            if (!Supply.Value.isBusy)
            {
                enterTime = Time.time;
                AnimationConstants.AnimateGathering(animator, true);
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

                thisSupplyMined = true;
                return Status.Success;
            }

            return Status.Running;
        }

        protected override void OnEnd()
        {
            AnimationConstants.AnimateGathering(animator, false);

            if (Supply.Value == null) { return; }
            if (CurrentStatus == Status.Success && thisSupplyMined)
            {
                Amount.Value = Supply.Value.EndGather();
                return;
            }

            // Early exit, e.g. due to new command issued
            Supply.Value.AbortGather();
        }
    }
}
