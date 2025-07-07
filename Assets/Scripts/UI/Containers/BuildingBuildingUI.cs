using GameDevTV.RTS.UI.Components;
using GameDevTV.RTS.Units;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GameDevTV.RTS.UI.Containers
{
    public class BuildingBuildingUI : MonoBehaviour, IUIElement<BaseBuilding>
    {
        // Tunables
        [Header("Hookups")]
        [SerializeField] private ProgressBar progressBar;
        [SerializeField] private BuildQueueButtonUI[] unitButtons;
        [Header("Properties")]
        [SerializeField] private float timeStep = 0.1f;

        // State
        private BaseBuilding baseBuilding;
        private Coroutine buildCoroutine;

        public void EnableFor(BaseBuilding baseBuilding)
        {
            if (baseBuilding == null) { return; }

            gameObject.SetActive(true);
            this.baseBuilding = baseBuilding;
            this.baseBuilding.onQueueUpdated += HandleQueueUpdated;

            HandleQueueUpdated(null);
            RefreshUnitButtons(baseBuilding.buildingQueueSnapshot);
        }

        public void Disable()
        {
            if (baseBuilding != null) { baseBuilding.onQueueUpdated -= HandleQueueUpdated; }
            if (buildCoroutine != null) { StopCoroutine(buildCoroutine); }

            buildCoroutine = null;
            baseBuilding = null;
            gameObject.SetActive(false);
        }

        private void HandleQueueUpdated(AbstractUnitSO[] unitsInQueue)
        {
            if (buildCoroutine == null) { buildCoroutine = StartCoroutine(UpdateUnitProgress()); }
            RefreshUnitButtons(unitsInQueue);
        }

        private IEnumerator UpdateUnitProgress()
        {
            while (baseBuilding != null && baseBuilding.queueSize > 0)
            {
                progressBar.SetProgress(baseBuilding.GetUnitBuildProgress());
                yield return new WaitForSeconds(timeStep);
            }
            buildCoroutine = null;
            progressBar.SetProgress(0);
        }

        private void RefreshUnitButtons(AbstractUnitSO[] unitsInQueue)
        {
            ClearUnitButtons();
            SetUnitButtons(unitsInQueue);
        }

        private void SetUnitButtons(AbstractUnitSO[] unitsInQueue)
        {
            if (unitsInQueue == null) { return; }
            if (baseBuilding == null) { return; }

            for(int i = 0; i < unitsInQueue.Length; i++)
            {
                unitButtons[i].EnableFor(unitsInQueue[i], HandleCancelBuildUnit(i));
            }
        }

        private void ClearUnitButtons()
        {
            foreach (BuildQueueButtonUI queuedUnitButton in unitButtons)
            {
                queuedUnitButton.Disable();
            }
        }

        private UnityAction HandleCancelBuildUnit(int unitIndex)
        {
            return () => baseBuilding.CancelBuildUnit(unitIndex);
        }
    }
}
