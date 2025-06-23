using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace GameDevTV.RTS.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "PickClosestPointOnCollider", story: "Set [TargetLocation] to closest point from [Agent] to [Target]", category: "Action", id: "004500396f2d0f70f50835d042434bb2")]
    public partial class PickClosestPointOnColliderAction : Action
    {
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<float> offsetDistance;

        protected override Status OnStart()
        {
            if (Agent.Value == null || Target.Value == null) { return Status.Failure; }

            if (Target.Value.TryGetComponent(out Collider targetCollider))
            {
                Vector3 newPosition = targetCollider.ClosestPoint(Agent.Value.transform.position);
                if (!Mathf.Approximately(offsetDistance, 0.0f))
                {
                    Vector3 unitShift = (newPosition - Target.Value.transform.position);
                    unitShift.y = 0.0f;
                    unitShift.Normalize();

                    newPosition += unitShift * offsetDistance;
                }
                TargetLocation.Value = newPosition;
            }
            else
            {
                TargetLocation.Value = Target.Value.transform.position;
            }
            return Status.Success;
        }
    }
}
