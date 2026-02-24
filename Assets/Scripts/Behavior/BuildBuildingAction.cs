using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;
using Action = Unity.Behavior.Action;
using Object = UnityEngine.Object;
using GameDevTV.RTS.Utilities;
using GameDevTV.RTS.Units;

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
            IncrementHealth();
            return UpdateRendererPosition();
        }

        protected override void OnEnd()
        {
            if (Agent.Value.TryGetComponent(out IBuildingBuilder builder)) { builder.Abort(); }
            
            if (CurrentStatus == Status.Success)
            {
                BuildingUnderConstruction.Value.ShowGhostVisuals(false);
                BuildingUnderConstruction.Value.enabled = true;
                return;
            }
            
            BuildingUnderConstruction.Value.PauseBuildingProgress();
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

        private void IncrementHealth()
        {
            if (BuildingUnderConstruction.Value == null) { return; }
            
            float normalizedDeltaTime = Time.deltaTime / buildTime;
            BuildingUnderConstruction.Value.IncrementHealthDelta(normalizedDeltaTime, true);
        }

        private Status UpdateRendererPosition()
        {
            if (buildingRenderer == null) { return Status.Failure; }
            
            float normalizedTime = Mathf.Clamp01((Time.time - startBuildTime) / buildTime);
            buildingRenderer.transform.SetLocalPositionAndRotation(Vector3.Lerp(startPosition, endPosition, normalizedTime), Quaternion.identity);
            return (normalizedTime >= 1.0) ? Status.Success : Status.Running;
        }

        private void InitializeRendererPosition(bool isBuildingResumed)
        {
            startPosition = -Vector3.up * buildingRenderer.bounds.size.y;
            if (isBuildingResumed) { startPosition = buildingRenderer.transform.localPosition; }
            endPosition = Vector3.zero;
            buildingRenderer.transform.SetLocalPositionAndRotation(startPosition, Quaternion.identity);
        }
    }
}
