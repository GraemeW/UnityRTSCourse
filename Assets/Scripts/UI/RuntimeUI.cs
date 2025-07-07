using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Player;
using GameDevTV.RTS.UI.Containers;
using GameDevTV.RTS.Units;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameDevTV.RTS.UI
{
    public class RuntimeUI : MonoBehaviour
    {
        // Tunables
        [Header("Hookups")]
        [SerializeField] private ActionsUI actionsUI;
        [SerializeField] private BuildingBuildingUI buildingBuildingUI;

        // State
        HashSet<AbstractCommandable> selectedUnits = new HashSet<AbstractCommandable>(PlayerInput.MAX_SELECTION_COUNT);

        #region UnityMethods
        private void OnEnable()
        {
            Bus<UnitSelectedEvent>.OnEvent += HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent += HandleUnitDeselected;
            Bus<UnitDeathEvent>.OnEvent += HandleUnitDeath;
        }

        private void OnDisable()
        {
            Bus<UnitSelectedEvent>.OnEvent -= HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent -= HandleUnitDeselected;
            Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
        }

        private void Start()
        {
            actionsUI.Disable();
            buildingBuildingUI.Disable();
        }
        #endregion

        #region EventHandlers
        private void HandleUnitSelected(UnitSelectedEvent unitSelectedEvent)
        {
            if (unitSelectedEvent.unit is not AbstractCommandable commandableUnit) { return; }
            selectedUnits.Add(commandableUnit);

            actionsUI.EnableFor(selectedUnits);
            if (selectedUnits.Count == 1 && unitSelectedEvent.unit is BaseBuilding baseBuilding) { buildingBuildingUI.EnableFor(baseBuilding); }
        }

        private void HandleUnitDeselected(UnitDeselectedEvent unitDeselectedEvent)
        {
            if (unitDeselectedEvent.unit is not AbstractCommandable commandableUnit) { return; }
            selectedUnits.Remove(commandableUnit);
            RefreshUI();
        }

        private void HandleUnitDeath(UnitDeathEvent unitDeathEvent)
        {
            if (unitDeathEvent.unit is not AbstractCommandable commandableUnit) { return; }
            selectedUnits.Remove(commandableUnit);
            RefreshUI();
        }
        #endregion

        #region HelperMethods
        private void RefreshUI()
        {
            if (selectedUnits.Count > 0)
            {
                actionsUI.EnableFor(selectedUnits);
                buildingBuildingUI.Disable();

                if (selectedUnits.Count == 1 && selectedUnits.First() is BaseBuilding baseBuilding) { buildingBuildingUI.EnableFor(baseBuilding); }
            }
            else
            {
                actionsUI.Disable();
                buildingBuildingUI.Disable();
            }
        }
        #endregion
    }
}
