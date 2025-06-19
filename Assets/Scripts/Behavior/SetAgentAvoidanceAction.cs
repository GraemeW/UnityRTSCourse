using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "SetAgentAvoidance", story: "Set [Agent] avoidance to [AvoidanceEnable]", category: "Action/Navigation", id: "1c8911f2fa3485965438f55f29b27ff0")]
    public partial class SetAgentAvoidanceAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<bool> AvoidanceEnable;
        protected override Status OnStart()
        {
            if (!Agent.Value.TryGetComponent(out AbstractUnit abstractUnit)) { return Status.Failure; }

            abstractUnit.ToggleAvoidance(AvoidanceEnable.Value);
            return Status.Success;
        }
    }
}
