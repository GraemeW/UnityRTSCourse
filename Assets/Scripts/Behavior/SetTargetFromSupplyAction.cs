using GameDevTV.RTS.Environment;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace GameDevTV.RTS.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "SetTargetFromSupply", story: "Set [Target] from [Supply]", category: "Action/Units", id: "9bbb9fdea0d25c48605f20ff83f7fb45")]
    public partial class SetTargetFromSupplyAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<GatherableSupply> Supply;

        protected override Status OnStart()
        {
            if (Supply.Value != null)
            {
                Target.Value = Supply.Value.gameObject;
            }
            return Status.Success;
        }
    }
}
