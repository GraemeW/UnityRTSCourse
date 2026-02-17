using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using GameDevTV.RTS.Commands;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.UI.Components;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.UI.Containers
{
    public class ActionsUI : MonoBehaviour, IUIElement<HashSet<AbstractCommandable>>
    {
        // Tunables
        [SerializeField] ActionButtonUI[] actionButtons;

        #region Interfaces
        public void EnableFor(HashSet<AbstractCommandable> setBaseBuilding) => RefreshButtons(setBaseBuilding);
        public void Disable() => RefreshButtons(null);
        #endregion

        #region HelperMethods
        private void RefreshButtons(HashSet<AbstractCommandable> commandableUnits)
        {
            ClearActionButtons();
            if (commandableUnits == null || commandableUnits.Count == 0) { return; }

            HashSet<BaseCommand> availableCommands = ReconcileCommands(commandableUnits);
            DrawActionButtons(availableCommands);
        }

        private void ClearActionButtons()
        {
            foreach (ActionButtonUI actionButton in actionButtons)
            {
                actionButton.Disable();
            }
        }

        private HashSet<BaseCommand> ReconcileCommands(HashSet<AbstractCommandable> commandableUnits)
        {
            var availableCommands = new HashSet<BaseCommand>();
            foreach (AbstractCommandable commandableUnit in commandableUnits)
            {
                if (commandableUnit == null)  { continue; }
                if (!commandableUnit.isActiveAndEnabled) { continue; }

                foreach (BaseCommand action in commandableUnit.currentCommands)
                {
                    availableCommands.Add(action);
                }
            }
            return availableCommands;
        }

        private void DrawActionButtons(HashSet<BaseCommand> availableCommands)
        {
            foreach (BaseCommand action in availableCommands.Where(action => action.slot < actionButtons.Length))
            {
                actionButtons[action.slot].EnableFor(action, HandleClick(action));
            }
        }

        private static UnityAction HandleClick(BaseCommand action)
        {
            return () => Bus<CommandSelectedEvent>.Raise(new CommandSelectedEvent(action));
        }
        #endregion
    }
}
