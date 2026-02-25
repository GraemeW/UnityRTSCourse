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

        #endregion
        
        #region ColliderMethods
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IDamageable damageable)) { return; }
            damageables.Add(damageable);
            onUnitEnter?.Invoke(damageable);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out IDamageable damageable)) { return; }
            damageables.Remove(damageable);
            onUnitExit?.Invoke(damageable);
        }
        #endregion
        
        #region PublicMethods
        public void SetupFrom(AttackConfigSO attackConfig)
        {
            if (attackConfig == null)  { return; }
            sensorCollider.radius = attackConfig.attackRange;
        }
        #endregion
    }
}
