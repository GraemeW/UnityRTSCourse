using System.Collections;
using UnityEngine;

namespace GameDevTV.RTS.Units
{
    public class BaseBuilding : AbstractCommandable
    {
        [field: SerializeField] public Transform spawnLocation { get; private set; }
        [field: SerializeField] public float spawnWalkDistance { get; private set; }

        public void BuildUnit(UnitSO unitSO)
        {
            StartCoroutine(DoBuildUnit(unitSO));
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

        private IEnumerator DoBuildUnit(UnitSO unitSO)
        {
            yield return new WaitForSeconds(unitSO.buildTime);
            SpawnUnit(unitSO);
        }
    }
}
