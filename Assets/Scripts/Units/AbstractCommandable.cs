using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Commands;

namespace GameDevTV.RTS.Units
{
    public abstract class AbstractCommandable : MonoBehaviour, ISelectable
    {
        [field: SerializeField] public int currentHealth { get; private set; }
        [field: SerializeField] public int maxHealth { get; private set; }
        [field: SerializeField] public AbstractUnitSO unitSO { get; private set; }

        [Header("Hookups")]
        [SerializeField] private DecalProjector decalProjector;
        [SerializeField] private List<BaseCommand> availableCommands = new();

        // State
        public List<BaseCommand> currentCommands { get; private set; }

        #region UnityMethods
        protected virtual void Start()
        {
            currentHealth = unitSO.health;
            maxHealth = unitSO.health;
            currentCommands = availableCommands;
        }
        #endregion

        #region Selection
        public void Deselect()
        {
            SetCommandOverrides(null, false);

            if (decalProjector != null)
            {
                decalProjector.gameObject.SetActive(false);
            }

            Bus<UnitDeselectedEvent>.Raise(new UnitDeselectedEvent(this));
        }

        public void Select()
        {
            if (decalProjector != null)
            {
                decalProjector.gameObject.SetActive(true);
            }

            Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(this));
        }
        #endregion

        #region Commands

        private void ResetCommandOverrides()
        {
            currentCommands = new List<BaseCommand>(availableCommands);
        }
        
        public void SetCommandOverrides(IList<BaseCommand> commandOverrides, bool callUnitSelectedEvent = true)
        {
            if (commandOverrides == null || commandOverrides.Count == 0) { ResetCommandOverrides(); }
            else { currentCommands = new List<BaseCommand>(commandOverrides); }
            if (callUnitSelectedEvent) { Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(this)); }
        }

        protected void AppendToCommands(IList<BaseCommand> commandOverrides, bool callUnitSelectedEvent = true)
        {
            if (commandOverrides == null || commandOverrides.Count == 0) { ResetCommandOverrides(); }
            else
            {
                foreach (BaseCommand commandOverride in commandOverrides)
                {
                    currentCommands.Add(commandOverride);
                }
            }
            if (callUnitSelectedEvent) { Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(this)); }
        }
        #endregion
    }
}
