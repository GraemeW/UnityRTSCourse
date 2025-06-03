using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveToTarget", story: "[Agent] moves to [Target]", category: "Action", id: "d49bfa35417b5afb5c87429dfab334ca")]
public partial class MoveToTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    // Behavior Properties
    // Cached References
    private NavMeshAgent navMeshAgent;

    protected override Status OnStart()
    {
        if (!Agent.Value.TryGetComponent(out navMeshAgent)) { return Status.Failure; }
        return SetTargetLocation(true);
    }

    protected override Status OnUpdate()
    {
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) { return Status.Success; }

        return SetTargetLocation();
    }

    private Status SetTargetLocation(bool earlyCheckCriteria = false)
    {
        if (Target.Value == null) { return Status.Failure; }

        Vector3 targetLocation = GetTargetPosition();
        if (earlyCheckCriteria)
        {
            if (Vector3.Distance(navMeshAgent.transform.position, targetLocation) <= navMeshAgent.stoppingDistance) { return Status.Success; }
        }
        navMeshAgent.SetDestination(targetLocation);

        return Status.Running;
    }

    private Vector3 GetTargetPosition()
    {
        Vector3 targetLocation = Target.Value.position;
        if (Target.Value.TryGetComponent(out Collider targetCollider))
        {
            targetLocation = targetCollider.ClosestPoint(Agent.Value.transform.position);
        }

        return targetLocation;
    }
}
