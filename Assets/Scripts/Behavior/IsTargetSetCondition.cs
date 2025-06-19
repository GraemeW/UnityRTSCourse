using System;
using Unity.Behavior;
using UnityEngine;

namespace GameDevTV.RTS.Behavior
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "IsTargetSet", story: "Agent [Target] is Set", category: "Variable Conditions", id: "c6d46be7a00ba0ad0a32e406b7a6e075")]
    public partial class IsTargetSetCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        public override bool IsTrue()
        {
            return Target.Value != null;
        }
    }
}
