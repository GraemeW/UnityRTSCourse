using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Commands;

namespace GameDevTV.RTS.Units
{
    public abstract class AbstractCommandable : MonoBehaviour, ISelectable, IDamageable
    {
        [field: SerializeField] public float maxHealth { get; protected set; }
        [field: SerializeField] public float initialHealthFraction { get; private set; } = 0.35f;
        [field: SerializeField] public AbstractUnitSO unitSO { get; private set; }

        [Header("Hookups")]
        public GameObject unitGameObject => gameObject;
        public Transform unitTransform => transform;
        [SerializeField] private DecalProjector decalProjector;
        [SerializeField] private List<BaseCommand> availableCommands = new();
        
        // State
        public List<BaseCommand> currentCommands { get; private set; } =  new();
        public float currentHealth  { get; protected set; } = 0f;
        
        // Events
        public event IDamageable.HealthUpdatedEvent onHealthUpdated;
        public event Action<IDamageable> onDeath;

        #region UnityMethods
        protected virtual void Start()
        {
            currentCommands = availableCommands;
        }

        protected virtual void OnDestroy()
        {
            onDeath?.Invoke(this);
        }
        #endregion

        #region ISelectableMethods  
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
        
        #region IDamageableMethods
        public int GetCurrentHealth() => Mathf.RoundToInt(currentHealth);

        public void AdjustHealth(float amount)
        {
            int lastHealth = GetCurrentHealth();
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
            onHealthUpdated?.Invoke(this, lastHealth, GetCurrentHealth());
            
            if (Mathf.Approximately(currentHealth, 0f)) { Die(); }
        }

        public void SetHealthFraction(float fractionOfHealth, bool floorToInitial = false)
        {
            int lastHealth = GetCurrentHealth();
            float minimumHealth = floorToInitial ? initialHealthFraction * maxHealth : 0;
            currentHealth = Mathf.Clamp(maxHealth * fractionOfHealth, minimumHealth, maxHealth);
            onHealthUpdated?.Invoke(this, lastHealth, GetCurrentHealth());
            
            if (Mathf.Approximately(currentHealth, 0f)) { Die(); }
        }

        public void AdjustHealthDelta(float normalizedDelta, bool deltaToInitial = false)
        {
            float minimum = deltaToInitial ? initialHealthFraction * maxHealth : 0;
            AdjustHealth(normalizedDelta * (maxHealth - minimum));
        }

        public void Die()
        {
            Destroy(gameObject);
        }
        #endregion
    }
}
