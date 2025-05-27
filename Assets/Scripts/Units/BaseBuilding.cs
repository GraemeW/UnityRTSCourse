using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevTV.RTS.Units
{
    public class BaseBuilding : AbstractCommandable
    {
        // Fixed
        private const int MAX_QUEUE_SIZE = 5;

        // Tunables
        [field: SerializeField] public Transform spawnLocation { get; private set; }
        [field: SerializeField] public float spawnWalkDistance { get; private set; }

        // Expression Properties
        public int queueSize => buildingQueue.Count;

        // State
        private Queue<UnitSO> buildingQueue = new (MAX_QUEUE_SIZE);
        private float currentQueueStartTime;
        private UnitSO buildingUnit;

        // Events
        public delegate void QueueUpdatedEvent(UnitSO[] unitsInQueue);
        public event QueueUpdatedEvent onQueueUpdated;

        public void BuildUnit(UnitSO unitSO)
        {
            if (buildingQueue.Count == MAX_QUEUE_SIZE) { return; }

            buildingQueue.Enqueue(unitSO);
            if (buildingQueue.Count == 1)
            {
                StartCoroutine(DoBuildUnits());
            }
            else
            {
                onQueueUpdated?.Invoke(buildingQueue.ToArray());
            }
        }

        public float GetBuildProgress()
        {
            return Mathf.Clamp01((Time.time - currentQueueStartTime) / buildingUnit.buildTime);
        }

        private IEnumerator DoBuildUnits()
        {
            while (buildingQueue.Count > 0)
            {
                // Peek
                currentQueueStartTime = Time.time;
                buildingUnit = buildingQueue.Peek();
                onQueueUpdated?.Invoke(buildingQueue.ToArray());

                // Build
                yield return new WaitForSeconds(buildingUnit.buildTime);

                // Spawn
                buildingQueue.Dequeue();
                SpawnUnit();
            }
            onQueueUpdated?.Invoke(buildingQueue.ToArray());
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
    }
}
