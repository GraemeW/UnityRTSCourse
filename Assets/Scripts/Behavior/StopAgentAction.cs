using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

namespace GameDevTV.RTS.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "StopAgent", story: "[Agent] stops moving", category: "Action/Navigation", id: "8b0790b5962c63647125ecf5bdba882c")]
    public partial class StopAgentAction : Action
    {
        // Behavior Properties
        [SerializeReference] public BlackboardVariable<GameObject> Agent;

        // Cached References
        private NavMeshAgent navMeshAgent;

        protected override Status OnStart()
        {
            if (!Agent.Value.TryGetComponent(out navMeshAgent)) { return Status.Failure; }

            navMeshAgent.ResetPath();
            return Status.Success;
        }
    }
}
