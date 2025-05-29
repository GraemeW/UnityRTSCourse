using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

namespace GameDevTV.RTS.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "MoveToTargetLocation", story: "[Agent] moves to [TargetLocation]", category: "Action/Navigation", id: "8cdc95513a454c6972af6d37f43154cf")]
    public partial class MoveToTargetLocationAction : Action
    {
        // Behavior Properties
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;

        // Cached References
        private NavMeshAgent navMeshAgent;

        protected override Status OnStart()
        {
            if (!Agent.Value.TryGetComponent(out navMeshAgent)) { return Status.Failure; }
            if (Vector3.Distance(navMeshAgent.transform.position, TargetLocation.Value) <= navMeshAgent.stoppingDistance) { return Status.Success; }

            navMeshAgent.SetDestination(TargetLocation.Value);

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) { return Status.Success; }

            return Status.Running;
        }
    }
}
