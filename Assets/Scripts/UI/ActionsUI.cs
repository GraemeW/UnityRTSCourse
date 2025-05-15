using GameDevTV.RTS.Commands;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Units;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevTV.RTS.UI
{
    public class ActionsUI : MonoBehaviour
    {
        // Tunables
        [SerializeField] ActionButtonUI[] actionButtons;

        // State
        HashSet<AbstractCommandable> commandableUnits = new HashSet<AbstractCommandable>();
        HashSet<ActionBase> availableCommands = new HashSet<ActionBase>();

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
            if (unitSelectedEvent.unit is not AbstractCommandable comandableUnit) { return; }
            commandableUnits.Add(comandableUnit);

            RefreshButtons();
        }

        private void HandleUnitDeselected(UnitDeselectedEvent unitSelectedEvent)
        {
            if (unitSelectedEvent.unit is not AbstractCommandable comandableUnit) { return; }
            commandableUnits.Remove(comandableUnit);

            RefreshButtons();
        }
        #endregion

        #region HelperMethods
        private void RefreshButtons()
        {
            ClearActionButtons();
            ReconcileCommands();
            DrawActionButtons();
        }

        private void ClearActionButtons()
        {
            foreach (ActionButtonUI actionButton in actionButtons)
            {
                actionButton.icon.sprite = null;
                actionButton.gameObject.SetActive(false);
            }
        }

        private void ReconcileCommands()
        {
            availableCommands.Clear();
            foreach (AbstractCommandable commandableUnit in commandableUnits)
            {
                foreach (ActionBase actionBase in commandableUnit.availableCommands)
                {
                    availableCommands.Add(actionBase);
                }
            }
        }

        private void DrawActionButtons()
        {
            foreach (ActionBase actionBase in availableCommands)
            {
                if (actionBase.slot >= actionButtons.Length) { continue; }

                actionButtons[actionBase.slot].icon.sprite = actionBase.icon;
                actionButtons[actionBase.slot].gameObject.SetActive(true);
            }
        }
        #endregion
    }
}
