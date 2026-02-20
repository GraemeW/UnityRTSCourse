using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using GameDevTV.RTS.UI.Components;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.UI.Containers
{
    public class BuildingBuildingUI : MonoBehaviour, IUIElement<BaseBuilding>
    {
        // Tunables
        [Header("Hookups")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private ProgressBar progressBar;
        [SerializeField] private BuildQueueButtonUI[] unitButtons;
        [Header("Properties")]
        [SerializeField] private float timeStep = 0.1f;

        // State
        private BaseBuilding baseBuilding;
        private Coroutine buildCoroutine;

        public void EnableFor(BaseBuilding setBaseBuilding)
        {
            if (setBaseBuilding == null) { return; }

            gameObject.SetActive(true);
            baseBuilding = setBaseBuilding;
            baseBuilding.onQueueUpdated += HandleQueueUpdated;

            HandleQueueUpdated(null);
            if (setBaseBuilding.unitSO != null) { nameText.text = Regex.Replace(setBaseBuilding.unitSO.name, "([A-Z])", " $1", RegexOptions.Compiled); }
            RefreshUnitButtons(setBaseBuilding.buildingQueueSnapshot);
        }

        public void Disable()
        {
            if (baseBuilding != null) { baseBuilding.onQueueUpdated -= HandleQueueUpdated; }
            if (buildCoroutine != null) { StopCoroutine(buildCoroutine); }

            nameText.text = string.Empty;
            buildCoroutine = null;
            baseBuilding = null;
            gameObject.SetActive(false);
        }

        private void HandleQueueUpdated(AbstractUnitSO[] unitsInQueue)
        {
            buildCoroutine ??= StartCoroutine(UpdateUnitProgress());
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
