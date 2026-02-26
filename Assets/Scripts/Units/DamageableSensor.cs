using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameDevTV.RTS.Units
{
    [RequireComponent(typeof(SphereCollider))]
    public class DamageableSensor : MonoBehaviour
    {
        // State
        private readonly HashSet<IDamageable> damageables = new();
        public IList<IDamageable> GetDamageables() => damageables.ToList();
        
        // Cached References
        private SphereCollider sensorCollider;
        
        // Events
        public delegate void UnitDetectionEvent(IDamageable damageable);
        public event UnitDetectionEvent onUnitEnter;
        public event UnitDetectionEvent onUnitExit;
        
        #region UnityMethods

        public void Awake()
        { 
            sensorCollider = GetComponent<SphereCollider>();
        }

        private void OnDestroy()
        {
            foreach (IDamageable damageable in damageables)
            {
                SubscribeToDamageableDeathEvent(damageable, false);
            }
        }
        #endregion
        
        #region ColliderMethods
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IDamageable damageable)) { return; }
            HandleTriggerEvent(damageable, true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out IDamageable damageable)) { return; }
            HandleTriggerEvent(damageable, false);
        }
        #endregion
        
        #region PublicMethods
        public void SetupFrom(AttackConfigSO attackConfig)
        {
            if (attackConfig == null)  { return; }
            sensorCollider.radius = attackConfig.attackRange;
        }
        #endregion
        
        #region PrivateMethods

        private void SubscribeToDamageableDeathEvent(IDamageable damageable, bool enable)
        {
            if (damageable == null) { return; }
            damageable.onDeath -= HandleDamageableDeath;
            if (enable) { damageable.onDeath += HandleDamageableDeath; }
        }

        private void HandleTriggerEvent(IDamageable damageable, bool isEntry)
        {
            if (damageable == null) { return; }
            
            SubscribeToDamageableDeathEvent(damageable, isEntry);
            if (isEntry)
            {
                damageables.Add(damageable);
                onUnitEnter?.Invoke(damageable);
            }
            else
            {
                damageables.Remove(damageable);
                onUnitExit?.Invoke(damageable);
            }
        }

        private void HandleDamageableDeath(IDamageable damageable)
        {
            HandleTriggerEvent(damageable, false);
        }
        #endregion
    }
}
