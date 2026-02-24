using GameDevTV.RTS.Units;
using System;
using Unity.Behavior;
using UnityEngine;

// Note:
// Moving namespaces currently breaks serialization -- don't do it for new blackboard scripts

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "BuildingIsInProgress", story: "[BaseBuilding] is being built", category: "Conditions", id: "ff39a03a4ea88444c8e568133aade2c4")]
public partial class BuildingIsInProgressCondition : Condition
{
    [SerializeReference] public BlackboardVariable<BaseBuilding> BaseBuilding;

    public override bool IsTrue()
    {
        if (BaseBuilding.Value == null) { return false; }
        return BaseBuilding.Value.GetBuildingProgress().state == BuildingProgress.BuildingState.Building;
    }
}
