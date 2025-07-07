using System;
using Unity.Behavior;
using UnityEngine;

namespace GameDevTV.RTS.Behavior
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "IsGhostSet", story: "Is [Ghost] Set", category: "Conditions", id: "6e769af53c854370cb1e5be2a6f8fc97")]
    public partial class IsGhostSetCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<GameObject> Ghost;

        public override bool IsTrue()
        {
            return Ghost.Value != null;
        }
    }
}
