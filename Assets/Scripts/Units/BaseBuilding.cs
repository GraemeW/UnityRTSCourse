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

        // State
        private Queue<UnitSO> buildingQueue = new (MAX_QUEUE_SIZE);

        public void BuildUnit(UnitSO unitSO)
        {
            if (buildingQueue.Count == MAX_QUEUE_SIZE) { return; }

            buildingQueue.Enqueue(unitSO);
            if (buildingQueue.Count == 1)
            {
                StartCoroutine(DoBuildUnits());
            }
        }

        private IEnumerator DoBuildUnits()
        {
            while (buildingQueue.Count > 0)
            {
                UnitSO unitSO = buildingQueue.Peek();
                yield return new WaitForSeconds(unitSO.buildTime);
                buildingQueue.Dequeue();
                SpawnUnit(unitSO);
            }
        }

        private void SpawnUnit(UnitSO unitSO)
        {
            GameObject spawnedUnit = Instantiate(unitSO.prefab);

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
