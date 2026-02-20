using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Player;
using GameDevTV.RTS.UI.Containers;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.UI
{
    public class RuntimeUI : MonoBehaviour
    {
        // Tunables
        [Header("Hookups")]
        [SerializeField] private ActionsUI actionsUI;
        [SerializeField] private UnitIconUI unitIconUI;
        [SerializeField] private SingleUnitSelectedUI singleUnitSelectedUI;
        [SerializeField] private BuildingBuildingUI buildingBuildingUI;

        // State
        private readonly HashSet<AbstractCommandable> selectedUnits = new(PlayerInput.maxSelectionCount);

        #region UnityMethods
        private void OnEnable()
        {
            Bus<UnitSelectedEvent>.SubscribeToEvent(HandleUnitSelected);
            Bus<UnitDeselectedEvent>.SubscribeToEvent(HandleUnitDeselected);
            Bus<UnitDeathEvent>.SubscribeToEvent(HandleUnitDeath);
            Bus<SupplyEvent>.SubscribeToEvent(HandleSuppliesUpdate);
        }

        private void OnDisable()
        {
            Bus<UnitSelectedEvent>.UnsubscribeFromEvent(HandleUnitSelected);
            Bus<UnitDeselectedEvent>.UnsubscribeFromEvent(HandleUnitDeselected);
            Bus<UnitDeathEvent>.UnsubscribeFromEvent(HandleUnitDeath);
            Bus<SupplyEvent>.UnsubscribeFromEvent(HandleSuppliesUpdate);
        }

        private void Start()
        {
            actionsUI.Disable();
            unitIconUI.Disable();
            singleUnitSelectedUI.Disable();
            buildingBuildingUI.Disable();
        }
        #endregion

        #region EventHandlers
        private void HandleUnitSelected(UnitSelectedEvent unitSelectedEvent)
        {
            if (unitSelectedEvent.unit is not AbstractCommandable commandableUnit) { return; }
            selectedUnits.Add(commandableUnit);

            RefreshUI();
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

        private void HandleSuppliesUpdate(SupplyEvent supplyEvent)
        {
            RefreshUI();
        }
        #endregion

        #region HelperMethods
        private void RefreshUI()
        {
            ClearUI();
            if (selectedUnits.Count > 0)
            {
                actionsUI.EnableFor(selectedUnits);
                if (selectedUnits.Count == 1)
                {
                    AbstractCommandable commandableUnit = selectedUnits.First();
                    unitIconUI.EnableFor(commandableUnit);
                    switch (commandableUnit)
                    {
                        case BaseBuilding baseBuilding:
                            buildingBuildingUI.EnableFor(baseBuilding);
                            break;
                        case AbstractUnit abstractUnit:
                            singleUnitSelectedUI.EnableFor(abstractUnit);
                            break;
                    }
                }
            }
        }

        private void ClearUI()
        {
            actionsUI.Disable();
            unitIconUI.Disable();
            singleUnitSelectedUI.Disable();
            buildingBuildingUI.Disable();
        }
        #endregion
    }
}
