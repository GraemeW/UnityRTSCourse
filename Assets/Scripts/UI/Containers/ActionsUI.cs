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
        [SerializeField] private ActionButtonUI[] actionButtons;
        
        // State
        private readonly HashSet<AbstractCommandable> commandableUnits = new();
        
        #region Interfaces

        public void EnableFor(HashSet<AbstractCommandable> setCommandables)
        {
            commandableUnits.Clear();
            foreach (AbstractCommandable commandable in setCommandables) { commandableUnits.Add(commandable); }
            RefreshButtons();
        }

        public void Disable()
        {
            commandableUnits.Clear();
            RefreshButtons();
        }
        #endregion

        #region HelperMethods
        private void RefreshButtons()
        {
            ClearActionButtons();
            if (commandableUnits.Count == 0) { return; }
            
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

        private HashSet<BaseCommand> ReconcileCommands(HashSet<AbstractCommandable> passCommandableUnits)
        {
            var availableCommands = new HashSet<BaseCommand>();
            foreach (AbstractCommandable commandableUnit in passCommandableUnits)
            {
                if (commandableUnit == null)  { continue; }
                if (!commandableUnit.isActiveAndEnabled) { continue; }

                foreach (BaseCommand action in commandableUnit.currentCommands.Where(action => action != null))
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
                if (action == null) { continue; }
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
