using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public UnitSO[] buildingQueueSnapshot => buildingQueue.ToArray();

        // State
        private List<UnitSO> buildingQueue = new (MAX_QUEUE_SIZE);
        private float currentQueueStartTime;
        private UnitSO buildingUnit;
        private Coroutine buildCoroutine = null;

        // Events
        public delegate void QueueUpdatedEvent(UnitSO[] unitsInQueue);
        public event QueueUpdatedEvent onQueueUpdated;

        #region GettersSetters
        public float GetBuildProgress() => Mathf.Clamp01((Time.time - currentQueueStartTime) / buildingUnit.buildTime);
        #endregion

        #region PublicMethods
        public void BuildUnit(UnitSO unitSO)
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
        #endregion
    }
}
