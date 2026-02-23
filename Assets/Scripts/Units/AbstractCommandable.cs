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
        [field: SerializeField] public int maxHealth { get; protected set; }
        [field: SerializeField] private float initialHealthFraction = 0.35f;
        [field: SerializeField] public AbstractUnitSO unitSO { get; private set; }

        [Header("Hookups")]
        [SerializeField] private DecalProjector decalProjector;
        [SerializeField] private List<BaseCommand> availableCommands = new();

        // State
        public List<BaseCommand> currentCommands { get; private set; } =  new();
        protected float currentHealth = 0f;
        
        // Events
        public delegate void HealthUpdatedEvent(AbstractCommandable commandable, int lastHealth, int newHealth);
        public event HealthUpdatedEvent onHealthUpdated;

        #region UnityMethods
        protected virtual void Start()
        {
            currentCommands = availableCommands;
        }
        #endregion

        #region Selection
        public void Deselect()
        {
            SetCommandOverrides(null);
            if (decalProjector != null) { decalProjector.gameObject.SetActive(false); }
            Bus<UnitDeselectedEvent>.Raise(new UnitDeselectedEvent(this));
        }

        public void Select()
        {
            if (decalProjector != null) { decalProjector.gameObject.SetActive(true); }
            SetCommandOverrides(null, false);
            Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(this));
        }
        #endregion

        #region Commands
        protected abstract void ReconcileContingentCommands();

        public void SetCommandOverrides(IList<BaseCommand> commandOverrides, bool announceCommandList = true)
        {
            if (commandOverrides == null || commandOverrides.Count == 0) { currentCommands = new List<BaseCommand>(availableCommands); }
            else { currentCommands = new List<BaseCommand>(commandOverrides); }
            ReconcileContingentCommands();
            
            if (announceCommandList) { Bus<CommandListUpdatedEvent>.Raise(new CommandListUpdatedEvent(this, currentCommands)); }
        }

        protected void AppendToCommands(IList<BaseCommand> commandOverrides, bool announceCommandList = true)
        {
            if (commandOverrides == null || commandOverrides.Count == 0) { return; }
            
            foreach (BaseCommand commandOverride in commandOverrides) { currentCommands.Add(commandOverride); }
            if (announceCommandList) { Bus<CommandListUpdatedEvent>.Raise(new CommandListUpdatedEvent(this, currentCommands)); }
        }
        #endregion
        
        #region PublicMethods

        public int GetCurrentHealth() => Mathf.RoundToInt(currentHealth);
        

        public void Heal(float amount)
        {
            int lastHealth = GetCurrentHealth();
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
            onHealthUpdated?.Invoke(this, lastHealth, GetCurrentHealth());
        }

        public void SetHealthFraction(float fractionOfHealth, bool floorToInitial = false)
        {
            int lastHealth = GetCurrentHealth();
            float minimumHealth = floorToInitial ? initialHealthFraction * maxHealth : 0;
            currentHealth = Mathf.Clamp(maxHealth * fractionOfHealth, minimumHealth, maxHealth);
            onHealthUpdated?.Invoke(this, lastHealth, GetCurrentHealth());
        }

        public void IncrementHealthDelta(float normalizedDelta, bool deltaToInitial = false)
        {
            float minimum = deltaToInitial ? initialHealthFraction * maxHealth : 0;
            Heal(normalizedDelta * (maxHealth - minimum));
        }
        #endregion
    }
}
