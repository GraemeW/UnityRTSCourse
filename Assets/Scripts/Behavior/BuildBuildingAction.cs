using GameDevTV.RTS.Units;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Object = UnityEngine.Object;

namespace GameDevTV.RTS.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "BuildBuilding", story: "[Agent] uses [BuildingSO] for [BuildingUnderConstruction] at [TargetLocation]", category: "Action/Units", id: "504bbb9f3e439b5ced98912b920ec6d2")]
    public partial class BuildBuildingAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<BuildingSO> BuildingSO;
        [SerializeReference] public BlackboardVariable<BaseBuilding> BuildingUnderConstruction;
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
        
        // State
        private float startBuildTime;
        private float buildTime;
        private Vector3 startPosition;
        private Vector3 endPosition;

        // Cached References
        Renderer buildingRenderer;

        protected override Status OnStart()
        {
            if (!HasValidInputs()) { return Status.Failure; }

            startBuildTime = Time.time;
            buildTime = (BuildingSO.Value.buildTime > 0.0f) ? BuildingSO.Value.buildTime : 0.1f;

            return EstablishBuildingInstance();
        }

        protected override Status OnUpdate()
        {
            return UpdateRendererPosition();
        }

        protected override void OnEnd()
        {
            if (CurrentStatus != Status.Success) { return; }
            
            BuildingUnderConstruction.Value.ShowGhostVisuals(false);
            BuildingUnderConstruction.Value.enabled = true;

            if (Agent.Value.TryGetComponent(out IBuildingBuilder builder))
            {
                builder.ResetCommandList();
            }
        }

        private bool HasValidInputs() => (Agent.Value != null && BuildingSO.Value != null && BuildingSO.Value.prefab != null);

        private Status EstablishBuildingInstance()
        {
            if (!Agent.Value.TryGetComponent(out IBuildingBuilder builder)) { return Status.Failure; }

            bool isBuildingResumed = true;
            if (BuildingUnderConstruction.Value == null)
            {
                isBuildingResumed = false;

                GameObject building = Object.Instantiate(BuildingSO.Value.prefab);
                if (!building.TryGetComponent(out BaseBuilding newBuilding)) { return Status.Failure; }

                BuildingUnderConstruction.Value = newBuilding;
                BuildingUnderConstruction.Value.transform.position = TargetLocation.Value;
            }

            buildingRenderer = BuildingUnderConstruction.Value.GetRenderer();
            if (buildingRenderer == null) { return Status.Failure; }
            InitializeRendererPosition(isBuildingResumed);

            BuildingUnderConstruction.Value.StartBuilding(builder, !isBuildingResumed);
            startBuildTime = BuildingUnderConstruction.Value.GetBuildingProgress().startTime;

            return Status.Running;
        }

        private Status UpdateRendererPosition()
        {
            float normalizedTime = Mathf.Clamp01((Time.time - startBuildTime) / buildTime);
            buildingRenderer.transform.SetLocalPositionAndRotation(Vector3.Lerp(startPosition, endPosition, normalizedTime), Quaternion.identity);
            return (normalizedTime >= 1.0) ? Status.Success : Status.Running;
        }

        private void InitializeRendererPosition(bool isBuildingResumed)
        {
            startPosition = -Vector3.up * buildingRenderer.bounds.size.y;
            endPosition = Vector3.zero;
            if (!isBuildingResumed) { buildingRenderer.transform.SetLocalPositionAndRotation(startPosition, Quaternion.identity); }
        }
    }
}
