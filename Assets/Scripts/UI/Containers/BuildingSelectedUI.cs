using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.UI.Containers
{
    public class BuildingSelectedUI : MonoBehaviour, IUIElement<BaseBuilding>
    {
        // Tunables
        [Header("Hookups")]
        [SerializeField] private BuildingUnderConstructionUI buildingUnderConstructionUI;
        [SerializeField] private BuildingBuildingUI buildingBuildingUI;
        [SerializeField] private SingleUnitSelectedUI singleUnitSelectedUI;
        
        // State
        private BaseBuilding baseBuilding;
        
        public void EnableFor(BaseBuilding setBaseBuilding)
        {
            ClearState();
            
            if (setBaseBuilding == null) { return; }
            baseBuilding = setBaseBuilding;
            baseBuilding.onQueueUpdated += HandleQueueUpdated;
            
            RefreshUI();
        }

        public void Disable()
        {
            ClearState();
            ClearUI();
        }

        private void ClearState()
        {
            if (baseBuilding == null) { return; }
            baseBuilding.onQueueUpdated -= HandleQueueUpdated;
            baseBuilding = null;
        }

        private void ClearUI()
        {
            buildingBuildingUI.Disable();
            buildingUnderConstructionUI.Disable();
            singleUnitSelectedUI.Disable();
        }

        private void RefreshUI()
        {
            if (baseBuilding == null) { return; }
            
            if (baseBuilding.GetBuildingProgress().state is BuildingProgress.BuildingState.Completed)
            {
                if (baseBuilding.queueSize > 0) { buildingBuildingUI.EnableFor(baseBuilding); }
                else { singleUnitSelectedUI.EnableFor(baseBuilding); }
            }
            else if (baseBuilding.GetBuildingProgress().state is BuildingProgress.BuildingState.Building or BuildingProgress.BuildingState.Paused)
            {
                buildingUnderConstructionUI.EnableFor(baseBuilding);
            }
        }

        private void HandleQueueUpdated(AbstractUnitSO[] _)
        {
            ClearUI();
            RefreshUI();
        }
    }
}
