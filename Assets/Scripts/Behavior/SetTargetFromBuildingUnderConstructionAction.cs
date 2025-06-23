using GameDevTV.RTS.Units;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace GameDevTV.RTS.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "SetTargetFromBuildingUnderConstruction", story: "Set [Target] from [BuildingUnderConstruction]", category: "Action/Units", id: "b0be2b397a2bdc4afc6d5e6c8b304821")]
    public partial class SetTargetFromBuildingUnderConstructionAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<BaseBuilding> BuildingUnderConstruction;
        protected override Status OnStart()
        {
            if (BuildingUnderConstruction.Value == null) { return Status.Failure; }

            Target.Value = BuildingUnderConstruction.Value.gameObject;
            return Status.Success;
        }
    }
}
