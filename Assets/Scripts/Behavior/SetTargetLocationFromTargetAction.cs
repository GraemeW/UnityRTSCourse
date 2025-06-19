using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace GameDevTV.RTS.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "SetTargetLocationFromTarget", story: "Attempt to set [TargetLocation] from [Target]", category: "Action/Units", id: "dec68c11a876f2d4c9251b380fedee72")]
    public partial class SetTargetLocationFromTargetAction : Action
    {
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        protected override Status OnStart()
        {
            if (Target == null) { return Status.Failure; }

            TargetLocation.Value = Target.Value.transform.position;
            return Status.Success;
        }
    }
}
