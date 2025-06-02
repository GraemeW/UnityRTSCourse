using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetMoveTarget", story: "Set [Target] from [InputObject]", category: "Action/Navigation", id: "2c0802b65aee54e0671f63eff8a0ac41")]
public partial class SetMoveTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Target;
    [SerializeReference] public BlackboardVariable<GameObject> InputObject;

    protected override Status OnStart()
    {
        if (InputObject.Value == null) { return Status.Failure; }

        Target.Value = InputObject.Value.transform;
        return Status.Success;
    }
}
