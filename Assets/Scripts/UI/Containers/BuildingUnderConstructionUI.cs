using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using GameDevTV.RTS.Units;
using GameDevTV.RTS.UI.Components;

namespace GameDevTV.RTS.UI.Containers
{
    public class BuildingUnderConstructionUI : MonoBehaviour, IUIElement<BaseBuilding>
    {
        // Tunables
        [Header("Hookups")]
        [SerializeField] private TextMeshProUGUI buildingNameText;
        [SerializeField] private ProgressBar progressBar;
        [Header("Properties")]
        [SerializeField] private float timeStep = 0.1f;
        
        public void EnableFor(BaseBuilding baseBuilding)
        {
            ClearState();
            if (baseBuilding == null || baseBuilding.unitSO == null) { return; }
            
            gameObject.SetActive(true);
            buildingNameText.text = Regex.Replace(baseBuilding.unitSO.name, "([A-Z])", " $1", RegexOptions.Compiled);
            StartCoroutine(AnimateBuildingProgress(baseBuilding));
        }

        public void Disable()
        {
            ClearState();
            gameObject.SetActive(false);
        }

        private void ClearState()
        {
            buildingNameText.text = string.Empty;
            StopAllCoroutines();
        }

        private IEnumerator AnimateBuildingProgress(BaseBuilding baseBuilding)
        {
            if (baseBuilding == null || baseBuilding.unitSO == null) { yield break; }
            
            while (enabled && baseBuilding.GetBuildingProgress().progress < 1)
            {
                if (baseBuilding.GetBuildingProgress().state != BuildingProgress.BuildingState.Building)
                {
                    yield return new WaitForSeconds(timeStep);
                    continue;
                }
                
                float startTime = baseBuilding.GetBuildingProgress().startTime;
                float endTime = startTime + baseBuilding.unitSO.buildTime;
                float currentProgress = Mathf.Clamp01((Time.time - startTime) / (endTime - startTime));
                
                progressBar.SetProgress(currentProgress);
                yield return new WaitForSeconds(timeStep);
            }
        }
    }
}
