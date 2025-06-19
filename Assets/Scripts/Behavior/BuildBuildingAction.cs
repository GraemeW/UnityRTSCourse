using GameDevTV.RTS.Units;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace GameDevTV.RTS.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "BuildBuilding", story: "[Agent] builds [BuildingSO] at [TargetLocation]", category: "Action/Units", id: "504bbb9f3e439b5ced98912b920ec6d2")]
    public partial class BuildBuildingAction : Action
    {
        // Tunables
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<BuildingSO> BuildingSO;
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;

        // State
        private float startBuildTime;
        private float buildTime;
        private BaseBuilding buildingInProgress;
        private Vector3 startPosition;

        protected override Status OnStart()
        {
            if (!HasValidInputs()) { return Status.Failure; }
            startBuildTime = Time.time;
            buildTime = (BuildingSO.Value.buildTime > 0.0f) ? BuildingSO.Value.buildTime : 0.1f;
            return MakeBuildingInstance();
        }

        protected override Status OnUpdate()
        {
            float normalizedTime = Mathf.Clamp01((Time.time - startBuildTime) / buildTime);
            buildingInProgress.transform.position = Vector3.Lerp(startPosition, TargetLocation.Value, normalizedTime);
            return (normalizedTime >= 1.0) ? Status.Success : Status.Running;
        }

        protected override void OnEnd()
        {
            if (CurrentStatus == Status.Success)
            { 
                buildingInProgress.enabled = true; 
            }
            else
            {
                if (buildingInProgress != null) { GameObject.Destroy(buildingInProgress); }
            }
        }

        private bool HasValidInputs() => (Agent.Value != null && BuildingSO.Value != null && BuildingSO.Value.prefab != null);

        private Status MakeBuildingInstance()
        {
            GameObject building = GameObject.Instantiate(BuildingSO.Value.prefab);
            if (!building.TryGetComponent(out buildingInProgress)) { return Status.Failure; }

            Renderer buildingRenderer = buildingInProgress.GetRenderer();
            startPosition = TargetLocation.Value - Vector3.up * buildingRenderer.bounds.size.y;

            buildingInProgress.transform.position = startPosition;

            return Status.Running;
        }
    }
}
