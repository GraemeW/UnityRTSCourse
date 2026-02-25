using UnityEngine;

namespace GameDevTV.RTS.Units
{
    public interface IDamageable
    {
        // Properties
        public float maxHealth {  get; }
        public float initialHealthFraction { get; }
        public float currentHealth { get; }
        public GameObject unitGameObject { get; }
        public Transform unitTransform { get; }
        
        // Interface Methods
        public int GetCurrentHealth();
        public void AdjustHealth(float amount);
        public void SetHealthFraction(float fractionOfHealth, bool floorToInitial = false);
        public void AdjustHealthDelta(float normalizedDelta, bool deltaToInitial = false);
        public void Die();
        
        // Events
        public delegate void HealthUpdatedEvent(AbstractCommandable commandable, int lastHealth, int newHealth);
        public event HealthUpdatedEvent onHealthUpdated;
    }
}
