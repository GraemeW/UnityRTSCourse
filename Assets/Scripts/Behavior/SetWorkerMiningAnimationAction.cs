using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using GameDevTV.RTS.Units;
using GameDevTV.RTS.Utilities;

namespace GameDevTV.RTS.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "SetWorkerMiningAnimation", story: "Set [Agent] work animation to [toggle]", category: "Action/Units", id: "dcc07817bdd01308832b9503655e141f")]
    public partial class SetWorkerMiningAnimationAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<bool> Toggle;

        protected override Status OnStart()
        {
            if (!Agent.Value.TryGetComponent(out Worker worker)) { return Status.Failure; }
            if (!Agent.Value.TryGetComponent(out Animator animator)) { return Status.Failure; }

            AnimationConstants.AnimateGathering(animator, Toggle.Value);
            return Status.Success;
        }
    }
}
