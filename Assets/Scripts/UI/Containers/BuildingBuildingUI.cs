using GameDevTV.RTS.UI.Components;
using GameDevTV.RTS.Units;
using System.Collections;
using UnityEngine;

namespace GameDevTV.RTS.UI.Containers
{
    public class BuildingBuildingUI : MonoBehaviour, IUIElement<BaseBuilding>
    {
        // Tunables
        [Header("Hookups")]
        [SerializeField] private ProgressBar progressBar;
        [Header("Properties")]
        [SerializeField] private float timeStep = 0.1f;

        // State
        private BaseBuilding baseBuilding;
        private Coroutine buildCoroutine;

        public void EnableFor(BaseBuilding baseBuilding)
        {
            gameObject.SetActive(true);
            this.baseBuilding = baseBuilding;
            baseBuilding.onQueueUpdated += HandleQueueUpdated;

            HandleQueueUpdated(null);
        }

        public void Disable()
        {
            if (baseBuilding != null) { baseBuilding.onQueueUpdated -= HandleQueueUpdated; }
            buildCoroutine = null;
            baseBuilding = null;
            gameObject.SetActive(false);
        }

        private void HandleQueueUpdated(UnitSO[] unitsInQueue)
        {
            if (buildCoroutine == null) { buildCoroutine = StartCoroutine(UpdateUnitProgress()); }
        }

        private IEnumerator UpdateUnitProgress()
        {
            while (baseBuilding != null && baseBuilding.queueSize > 0)
            {
                progressBar.SetProgress(baseBuilding.GetBuildProgress());
                yield return new WaitForSeconds(timeStep);
            }
            buildCoroutine = null;
            progressBar.SetProgress(0);
        }
    }
}
