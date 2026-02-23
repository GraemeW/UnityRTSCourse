using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Player;
using GameDevTV.RTS.Units;
using GameDevTV.RTS.UI.Containers;

namespace GameDevTV.RTS.UI
{
    public class RuntimeUI : MonoBehaviour
    {
        // Tunables
        [Header("Hookups")]
        [SerializeField] private ActionsUI actionsUI;
        [SerializeField] private UnitIconUI unitIconUI;
        [SerializeField] private SingleUnitSelectedUI singleUnitSelectedUI;
        [SerializeField] private BuildingSelectedUI buildingSelectedUI;

        // State
        private readonly HashSet<AbstractCommandable> selectedUnits = new(PlayerInput.maxSelectionCount);

        #region UnityMethods
        private void OnEnable()
        {
            Bus<UnitSelectedEvent>.SubscribeToEvent(HandleUnitSelected);
            Bus<UnitDeselectedEvent>.SubscribeToEvent(HandleUnitDeselected);
            Bus<CommandListUpdatedEvent>.SubscribeToEvent(HandleCommandListUpdatedEvent);
            Bus<UnitDeathEvent>.SubscribeToEvent(HandleUnitDeath);
            Bus<SupplyEvent>.SubscribeToEvent(HandleSuppliesUpdate);
            Bus<BuildingSpawnEvent>.SubscribeToEvent(HandleBuildingSpawnEvent);
        }

        private void OnDisable()
        {
            Bus<UnitSelectedEvent>.UnsubscribeFromEvent(HandleUnitSelected);
            Bus<UnitDeselectedEvent>.UnsubscribeFromEvent(HandleUnitDeselected);
            Bus<CommandListUpdatedEvent>.UnsubscribeFromEvent(HandleCommandListUpdatedEvent);
            Bus<UnitDeathEvent>.UnsubscribeFromEvent(HandleUnitDeath);
            Bus<SupplyEvent>.UnsubscribeFromEvent(HandleSuppliesUpdate);
            Bus<BuildingSpawnEvent>.UnsubscribeFromEvent(HandleBuildingSpawnEvent);
        }

        private void Start()
        {
            ClearUI();
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

        private void HandleCommandListUpdatedEvent(CommandListUpdatedEvent commandListUpdatedEvent)
        {
            if (!commandListUpdatedEvent.commandables.Any(commandableUnit => selectedUnits.Contains(commandableUnit))) { return; }
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
        private void HandleBuildingSpawnEvent(BuildingSpawnEvent buildingSpawnEvent)
        {
            if (buildingSpawnEvent.baseBuilding is not ISelectable selectableBuilding) { return; }
            if (!selectedUnits.Contains(selectableBuilding)) { return; }
            RefreshUI();
        }
        
        #endregion

        #region HelperMethods
        private void RefreshUI()
        {
            ClearUI();
            if (selectedUnits.Count == 0) { return; }
            
            actionsUI.EnableFor(selectedUnits);
            if (selectedUnits.Count == 1)
            {
                AbstractCommandable commandableUnit = selectedUnits.First();
                unitIconUI.EnableFor(commandableUnit);
                switch (commandableUnit)
                {
                    case BaseBuilding baseBuilding:
                        buildingSelectedUI.EnableFor(baseBuilding);
                        break;
                    case AbstractUnit abstractUnit:
                        singleUnitSelectedUI.EnableFor(abstractUnit);
                        break;
                }
            }
        }

        private void ClearUI()
        {
            actionsUI.Disable();
            unitIconUI.Disable();
            singleUnitSelectedUI.Disable();
            buildingSelectedUI.Disable();
        }
        #endregion
    }
}
