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
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    // Behavior Properties
    // Cached References
    private NavMeshAgent navMeshAgent;

    protected override Status OnStart()
    {
        if (Agent.Value == null || !Agent.Value.TryGetComponent(out navMeshAgent)) { return Status.Failure; }
        if (Target.Value == null) { return Status.Failure; }

        Vector3 targetLocation = GetTargetPosition();
        navMeshAgent.ResetPath();
        navMeshAgent.SetDestination(targetLocation);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Agent.Value == null || Target.Value == null) { return Status.Failure; }

        // Insane number of checks to verify arrival -- vetted, this is awful but necessary
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
            {
                return Status.Success;
            }
        }

        Vector3 targetLocation = GetTargetPosition();
        navMeshAgent.SetDestination(targetLocation);
        return Status.Running;
    }

    private Vector3 GetTargetPosition()
    {
        Vector3 targetLocation = Target.Value.transform.position;

        if (Target.Value.TryGetComponent(out Collider targetCollider))
        {
            targetLocation = targetCollider.ClosestPoint(Agent.Value.transform.position);
        }

        return targetLocation;
    }
}
