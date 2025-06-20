using GameDevTV.RTS.Units;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace GameDevTV.RTS.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "PickRandomLocationWithinRendererBounds", story: "Set [TargetLocation] to random point in [BuildingUnderConstruction]", category: "Action", id: "46b0e8d26490b3a865c20ddd5268a9ca")]
    public partial class PickRandomLocationWithinRendererBoundsAction : Action
    {
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
        [SerializeReference] public BlackboardVariable<BaseBuilding> BuildingUnderConstruction;

        protected override Status OnStart()
        {
            if (BuildingUnderConstruction.Value == null) { return Status.Failure; }

            Renderer renderer = BuildingUnderConstruction.Value.GetRenderer();
            if (renderer == null) { return Status.Failure; }

            Bounds bounds = renderer.bounds;
            TargetLocation.Value = new Vector3(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                TargetLocation.Value.y,
                UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
            );

            return Status.Success;
        }
    }
}
