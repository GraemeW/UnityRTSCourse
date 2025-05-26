using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.UI.Containers;
using GameDevTV.RTS.Units;
using System.Collections.Generic;
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
        }

        private void OnDisable()
        {
            Bus<UnitSelectedEvent>.OnEvent -= HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent -= HandleUnitDeselected;

        }
        #endregion

        #region EventHandlers
        private void HandleUnitSelected(UnitSelectedEvent unitSelectedEvent)
        {
            if (unitSelectedEvent.unit is not AbstractCommandable commandableUnit) { return; }
            selectedUnits.Add(commandableUnit);

            if (actionsUI != null) { actionsUI.EnableFor(selectedUnits); }
        }

        private void HandleUnitDeselected(UnitDeselectedEvent unitDeselectedEvent)
        {
            if (unitDeselectedEvent.unit is not AbstractCommandable commandableUnit) { return; }
            selectedUnits.Remove(commandableUnit);

            if (actionsUI != null && selectedUnits.Count > 0) { actionsUI.EnableFor(selectedUnits); }
            else { actionsUI.Disable(); }
        }
        #endregion
    }
}
