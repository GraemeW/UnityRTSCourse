using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "IsCommandPostSet", story: "Agent [CommandPost] is Set", category: "Conditions", id: "158081f5d21946e90b0135f6f5a100c6")]
public partial class IsCommandPostSetCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> CommandPost;

    public override bool IsTrue()
    {
        return CommandPost.Value != null;
    }
}
