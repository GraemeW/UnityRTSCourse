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
        private const int _maxQueueSize = 5;
        public const string buildingsLayerMaskRef = "Buildings";

        // Tunables
        [field: SerializeField] public Transform spawnLocation { get; private set; }
        [field: SerializeField] public float spawnWalkDistance { get; private set; }

        // Expression Properties
        public int queueSize => buildingQueue.Count;
        public AbstractUnitSO[] buildingQueueSnapshot => buildingQueue.ToArray();

        // Cached References
        private NavMeshObstacle navMeshObstacle;
        private BuildingSO buildingSO;
        private readonly Dictionary<MeshRenderer, Material> rendererLookup = new();

        // State
        private readonly List<AbstractUnitSO> buildingQueue = new (_maxQueueSize);
        private float currentQueueStartTime;
        private AbstractUnitSO buildingUnit;
        private Coroutine buildCoroutine;
        private BuildingProgress progress = new(BuildingProgress.BuildingState.Destroyed, 0.0f, 0.0f);
        private IBuildingBuilder unitBuildingThis;

        // Events
        public delegate void QueueUpdatedEvent(AbstractUnitSO[] unitsInQueue);
        public event QueueUpdatedEvent onQueueUpdated;

        #region UnityMethods
        private void Awake()
        {
            InitializeBuildingProperties();
            InitializeBuildingReferences();
        }

        private void InitializeBuildingProperties()
        {
            buildingSO = unitSO as BuildingSO;
            if (buildingSO == null) { Debug.Log($"BaseBuilding must use a BuildingSO for its AbstractUnitSO field.  Replace current: {unitSO}"); }
            
            maxHealth = unitSO.health;
        }

        private void InitializeBuildingReferences()
        {
            navMeshObstacle = GetComponent<NavMeshObstacle>();

            foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
            {
                rendererLookup[meshRenderer] = meshRenderer.material;
            }
        }

        protected override void Start()
        {
            base.Start();
            if (navMeshObstacle != null) { navMeshObstacle.enabled = true; }
            
            unitBuildingThis = null;
            if (Mathf.Approximately(currentHealth, 0f)) { currentHealth = unitSO.health; }
            progress = new BuildingProgress(BuildingProgress.BuildingState.Completed, 0.0f, 1.0f);
            Bus<UnitDeathEvent>.SubscribeToEvent(HandleUnitDeath);
            Bus<BuildingSpawnEvent>.Raise(new BuildingSpawnEvent(this));
        }

        protected override void ReconcileContingentCommands()
        {
            // No special commands
        }

        protected void OnDestroy()
        {
            Bus<UnitDeathEvent>.UnsubscribeFromEvent(HandleUnitDeath);
            Bus<BuildingDeathEvent>.Raise(new BuildingDeathEvent(this));
        }
        #endregion

        #region PublicMethods
        public BuildingSO GetBuildingSO() => buildingSO;
        public BuildingProgress GetBuildingProgress() => progress;
        public float GetUnitBuildProgress() => buildingUnit != null ? Mathf.Clamp01((Time.time - currentQueueStartTime) / buildingUnit.buildTime) : 0.0f;
        public MeshRenderer GetRenderer() => rendererLookup.FirstOrDefault().Key;

        public void PauseBuildingProgress()
        {
            float currentProgress = progress.progress;
            progress = new BuildingProgress( BuildingProgress.BuildingState.Paused, 0f, currentProgress);
        }
        
        public void BuildUnit(AbstractUnitSO unitToBuild)
        {
            if (buildingQueue.Count == _maxQueueSize) { return; }

            buildingQueue.Add(unitToBuild);
            unitToBuild.ChargeSupplies();
            
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

            AbstractUnitSO canceledUnit = buildingQueue[index];
            canceledUnit.RefundSupplies();
            
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
            enabled = false;

            float currentProgress = initializeProgress ? 0.0f : progress.progress;
            progress = new BuildingProgress(
                BuildingProgress.BuildingState.Building, 
                Time.time - buildingSO.buildTime * currentProgress,
                currentProgress
            );

            if (progress.progress == 0) { SetHealthFraction(0f, true); }

            Bus<UnitDeathEvent>.UnsubscribeFromEvent(HandleUnitDeath);
            Bus<UnitDeathEvent>.SubscribeToEvent(HandleUnitDeath);
        }

        public void ShowGhostVisuals(bool enable)
        {
            if (buildingSO == null) { return; }

            Material ghostMaterial = buildingSO.placementMaterial;
            foreach ((MeshRenderer setRenderer, Material initialMaterial) in rendererLookup)
            {
                setRenderer.material = enable ? ghostMaterial : initialMaterial;
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
            if (Mathf.Approximately(spawnWalkDistance, 0f)) { return; }
            
            Vector3 walkPosition = spawnLocation.position + baseToSpawnDelta * spawnWalkDistance;
            abstractUnit.MoveTo(walkPosition);
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

            Bus<UnitDeathEvent>.UnsubscribeFromEvent(HandleUnitDeath);
        }
        #endregion
    }
}
