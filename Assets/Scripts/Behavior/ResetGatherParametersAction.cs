using GameDevTV.RTS.Environment;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ResetGatherParameters", story: "Reset [Supply] , [SupplyType] and [CommandPost]", category: "Action/Units", id: "93b036c6f6d1e731202718ed8c05f1ee")]
public partial class ResetGatherParametersAction : Action
{
    [SerializeReference] public BlackboardVariable<GatherableSupply> Supply;
    [SerializeReference] public BlackboardVariable<SupplySO> SupplyType;
    [SerializeReference] public BlackboardVariable<GameObject> CommandPost;
    protected override Status OnStart()
    {
        if (Supply.Value != null) { Supply.Value.ResetGather(); }
        Supply.Value = null;
        SupplyType.Value = null;
        CommandPost.Value = null;

        return Status.Success;
    }
}

