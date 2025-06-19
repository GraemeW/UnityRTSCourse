using GameDevTV.RTS.Environment;
using System;
using Unity.Behavior;
using UnityEngine;

namespace GameDevTV.RTS.Behavior
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "IsSupplyTypeSet", story: "Agent [SupplyType] is Set", category: "Variable Conditions", id: "9ea7c88c781eddd144b235233a4f1b4c")]
    public partial class IsSupplyTypeSetCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<SupplySO> SupplyType;

        public override bool IsTrue()
        {
            return SupplyType.Value != null;
        }
    }
}
