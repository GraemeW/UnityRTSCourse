using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace GameDevTV.RTS.Units
{
    public class BaseBuilding : AbstractCommandable
    {
        // Fixed
        private const int MAX_QUEUE_SIZE = 5;
        public static string buildingsLayerMaskRef = "Buildings";

        // Tunables
        [field: SerializeField] public Transform spawnLocation { get; private set; }
        [field: SerializeField] public float spawnWalkDistance { get; private set; }

        // Expression Properties
        public int queueSize => buildingQueue.Count;
        public AbstractUnitSO[] buildingQueueSnapshot => buildingQueue.ToArray();

        // Cached References
        private NavMeshObstacle navMeshObstacle;
        private BuildingSO buildingSO;
        private Dictionary<MeshRenderer, Material> rendererLookup = new Dictionary<MeshRenderer, Material>();

        // State
        private List<AbstractUnitSO> buildingQueue = new (MAX_QUEUE_SIZE);
        private float currentQueueStartTime;
        private AbstractUnitSO buildingUnit;
        private Coroutine buildCoroutine = null;
        private BuildingProgress progress = new BuildingProgress(BuildingProgress.BuildingState.Destroyed, 0.0f, 0.0f);
        private IBuildingBuilder unitBuildingThis;

        // Events
        public delegate void QueueUpdatedEvent(AbstractUnitSO[] unitsInQueue);
        public event QueueUpdatedEvent onQueueUpdated;

        #region UnityMethods
        private void Awake()
        {
            navMeshObstacle = GetComponent<NavMeshObstacle>();

            foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
            {
                rendererLookup[meshRenderer] = meshRenderer.material;
            }

            buildingSO = unitSO as BuildingSO;
            if (buildingSO == null) { UnityEngine.Debug.Log($"BaseBuilding must use a BuildingSO for its AbstractUnitSO field.  Replace current: {unitSO}"); }
        }

        protected override void Start()
        {
            base.Start();
            if (navMeshObstacle != null) { navMeshObstacle.enabled = true; }
        }

        private void OnEnable()
        {
            unitBuildingThis = null;
            progress = new BuildingProgress(BuildingProgress.BuildingState.Completed, 0.0f, 1.0f);
            Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
        }

        private void OnDestroy()
        {
            Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
        }
        #endregion

        #region PublicMethods
        public BuildingSO GetBuildingSO() => buildingSO;
        public BuildingProgress GetBuildingProgress() => progress;
        public float GetUnitBuildProgress() => Mathf.Clamp01((Time.time - currentQueueStartTime) / buildingUnit.buildTime);
        public MeshRenderer GetRenderer() => rendererLookup.FirstOrDefault().Key;

        public void BuildUnit(AbstractUnitSO unitSO)
        {
            if (buildingQueue.Count == MAX_QUEUE_SIZE) { return; }

            buildingQueue.Add(unitSO);
            if (buildCoroutine == null)
            {
                buildCoroutine = StartCoroutine(DoBuildUnits());
            }
            else
            {
                onQueueUpdated?.Invoke(buildingQueue.ToArray());
            }
        }

        public void CancelBuildUnit(int index)
        {
            if (index < 0 || index >= buildingQueue.Count) { return; }

            if (index == 0)
            {
                StopCoroutine(buildCoroutine);
                buildingQueue.RemoveAt(0);
                buildCoroutine = StartCoroutine(DoBuildUnits());
            }
            else
            {
                buildingQueue.RemoveAt(index);
                onQueueUpdated?.Invoke(buildingQueue.ToArray());
            }
        }

        public void StartBuilding(IBuildingBuilder buildingBuilder, bool initializeProgress)
        {
            unitBuildingThis = buildingBuilder;
            ShowGhostVisuals(true);
            this.enabled = false;

            float currentProgress = initializeProgress ? 0.0f : progress.progress;
            progress = new BuildingProgress(
                BuildingProgress.BuildingState.Building, 
                Time.time - buildingSO.buildTime * currentProgress,
                currentProgress
            );

            Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
            Bus<UnitDeathEvent>.OnEvent += HandleUnitDeath;
        }

        public void ShowGhostVisuals(bool enable)
        {
            if (buildingSO == null) { return; }

            Material ghostMaterial = buildingSO.placementMaterial;
            foreach (var (renderer, initialMaterial) in rendererLookup)
            {
                if (enable) { renderer.material = ghostMaterial; }
                else { renderer.material = initialMaterial; }
            }
        }
        #endregion

        #region HelperMethods
        private IEnumerator DoBuildUnits()
        {
            while (buildingQueue.Count > 0)
            {
                // Peek
                currentQueueStartTime = Time.time;
                buildingUnit = buildingQueue[0];
                onQueueUpdated?.Invoke(buildingQueue.ToArray());

                // Build
                yield return new WaitForSeconds(buildingUnit.buildTime);

                // Spawn
                buildingQueue.RemoveAt(0);
                SpawnUnit();
            }
            onQueueUpdated?.Invoke(buildingQueue.ToArray());
            buildCoroutine = null;
        }

        private void SpawnUnit()
        {
            GameObject spawnedUnit = Instantiate(buildingUnit.prefab);

            if (spawnedUnit.TryGetComponent(out AbstractUnit abstractUnit))
            {
                abstractUnit.WarpTo(spawnLocation.position);
                SpawnOffsetWalk(abstractUnit);
            }
            else
            {
                spawnedUnit.transform.position = spawnLocation.position;
            }
        }

        private void SpawnOffsetWalk(AbstractUnit abstractUnit)
        {
            Vector3 baseToSpawnDelta = (spawnLocation.position - transform.position);
            baseToSpawnDelta.Normalize();
            if (!Mathf.Approximately(spawnWalkDistance, 0f))
            {
                Vector3 walkPosition = spawnLocation.position + baseToSpawnDelta * spawnWalkDistance;
                abstractUnit.MoveTo(walkPosition);
            }
        }

        private void HandleUnitDeath(UnitDeathEvent unitDeathEvent)
        {
            if (!unitDeathEvent.unit.TryGetComponent(out IBuildingBuilder buildingBuilder)) { return; } 
            if (buildingBuilder != unitBuildingThis) { return; }

            progress = new BuildingProgress(
                BuildingProgress.BuildingState.Paused, 
                progress.startTime, 
                Mathf.Clamp01((Time.time - progress.startTime) / buildingSO.buildTime)
            );

            Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
        }
        #endregion
    }
}
