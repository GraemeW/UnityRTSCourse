using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

namespace GameDevTV.RTS.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "SamplePosition", story: "Set [TargetLocation] to the nearest point on the NavMesh to [Target]", category: "Action/Navigation", id: "b4ee10852ffb6895bc9906466cc51b36")]
    public partial class SamplePositionAction : Action
    {
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<float> Radius = new(5);

        protected override Status OnStart()
        {
            if (Target.Value == null || !Target.Value.TryGetComponent(out NavMeshAgent navMeshAgent)) { return Status.Failure; }

            NavMeshQueryFilter queryFilter = new();
            queryFilter.agentTypeID = navMeshAgent.agentTypeID;
            queryFilter.areaMask = navMeshAgent.areaMask;

            if (NavMesh.SamplePosition(Target.Value.transform.position, out NavMeshHit hit, Radius.Value, queryFilter))
            {
                TargetLocation.Value = hit.position;
                return Status.Success;
            }

            return Status.Failure;
        }
    }
}
