using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using GameDevTV.RTS.Units;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ReconcileMoveTarget", story: "Reset [Agent] [TargetLocation] for no [Target]", category: "Action/Navigation", id: "52432eb6f0eef23cdfe6e4d42a007081")]
public partial class ReconcileMoveTargetAction : Action
{
    // Behavior Properties
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    // State
    GameObject currentTarget;
    Vector3 currentTargetLocation;

    // Cached References
    private BehaviorGraphAgent behaviorAgent;

    protected override Status OnStart()
    {
        if (!Agent.Value.TryGetComponent(out behaviorAgent)) { return Status.Failure; }
        return ReconcileTargets();
    }

    protected override Status OnUpdate()
    {
        return ReconcileTargets();
    }

    private Status ReconcileTargets()
    {
        if (currentTarget != Target.Value)
        {
            currentTarget = Target.Value;
        }
        else if (currentTargetLocation != TargetLocation.Value)
        {
            currentTargetLocation = TargetLocation.Value;
            currentTarget = null;
            behaviorAgent.SetVariableValue(AbstractUnit.targetRef, currentTarget);
        }
        return Status.Success;
    }
}
