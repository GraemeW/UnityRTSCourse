using GameDevTV.RTS.Commands;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.UI.Components;
using GameDevTV.RTS.Units;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameDevTV.RTS.UI.Containers
{
    public class ActionsUI : MonoBehaviour, IUIElement<HashSet<AbstractCommandable>>
    {
        // Tunables
        [SerializeField] ActionButtonUI[] actionButtons;

        #region Interfaces
        public void EnableFor(HashSet<AbstractCommandable> commandableUnits) => RefreshButtons(commandableUnits);
        public void Disable() => RefreshButtons(null);
        #endregion

        #region HelperMethods
        private void RefreshButtons(HashSet<AbstractCommandable> commandableUnits)
        {
            ClearActionButtons();
            if (commandableUnits == null || commandableUnits.Count == 0) { return; }

            HashSet<ActionBase> availableCommands = ReconcileCommands(commandableUnits);
            DrawActionButtons(availableCommands);
        }

        private void ClearActionButtons()
        {
            foreach (ActionButtonUI actionButton in actionButtons)
            {
                actionButton.Disable();
            }
        }

        private HashSet<ActionBase> ReconcileCommands(HashSet<AbstractCommandable> commandableUnits)
        {
            HashSet<ActionBase> availableCommands = new HashSet<ActionBase>();
            foreach (AbstractCommandable commandableUnit in commandableUnits)
            {
                foreach (ActionBase action in commandableUnit.currentCommands)
                {
                    availableCommands.Add(action);
                }
            }
            return availableCommands;
        }

        private void DrawActionButtons(HashSet<ActionBase> availableCommands)
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
