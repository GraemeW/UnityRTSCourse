using GameDevTV.RTS.Commands;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Units;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
                actionButton.Disable();
            }
        }

        private void ReconcileCommands()
        {
            availableCommands.Clear();
            foreach (AbstractCommandable commandableUnit in commandableUnits)
            {
                foreach (ActionBase action in commandableUnit.availableCommands)
                {
                    availableCommands.Add(action);
                }
            }
        }

        private void DrawActionButtons()
        {
            foreach (ActionBase action in availableCommands)
            {
                if (action.slot >= actionButtons.Length) { continue; }

                actionButtons[action.slot].EnableFor(action, HandleClick(action));
            }
        }

        private UnityAction HandleClick(ActionBase action)
        {
            return () => Bus<ActionSelectedEvent>.Raise(new ActionSelectedEvent(action));
        }
        #endregion
    }
}
