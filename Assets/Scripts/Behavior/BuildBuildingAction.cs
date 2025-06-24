using GameDevTV.RTS.Units;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

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

            return MakeBuildingInstance();
        }

        protected override Status OnUpdate()
        {
            return UpdateRendererPosition();
        }

        protected override void OnEnd()
        {
            if (CurrentStatus == Status.Success)
            {
                BuildingUnderConstruction.Value.ShowGhostVisuals(false);
                BuildingUnderConstruction.Value.enabled = true; 
            }
            else
            {
                if (BuildingUnderConstruction != null) { GameObject.Destroy(BuildingUnderConstruction); }
            }
        }

        private bool HasValidInputs() => (Agent.Value != null && BuildingSO.Value != null && BuildingSO.Value.prefab != null);

        private Status MakeBuildingInstance()
        {
            GameObject building = GameObject.Instantiate(BuildingSO.Value.prefab);
            if (!building.TryGetComponent(out BaseBuilding newBuilding)) { return Status.Failure; }
            BuildingUnderConstruction.Value = newBuilding;
            BuildingUnderConstruction.Value.transform.position = TargetLocation.Value;

            buildingRenderer = BuildingUnderConstruction.Value.GetRenderer();
            if (buildingRenderer == null) { return Status.Failure; }
            InitializeRendererPosition();

            BuildingUnderConstruction.Value.ShowGhostVisuals(true);

            return Status.Running;
        }

        private Status UpdateRendererPosition()
        {
            float normalizedTime = Mathf.Clamp01((Time.time - startBuildTime) / buildTime);
            buildingRenderer.transform.SetLocalPositionAndRotation(Vector3.Lerp(startPosition, endPosition, normalizedTime), Quaternion.identity);
            return (normalizedTime >= 1.0) ? Status.Success : Status.Running;
        }

        private void InitializeRendererPosition()
        {
            startPosition = -Vector3.up * buildingRenderer.bounds.size.y;
            endPosition = Vector3.zero;
            buildingRenderer.transform.SetLocalPositionAndRotation(startPosition, Quaternion.identity);
        }
    }
}
