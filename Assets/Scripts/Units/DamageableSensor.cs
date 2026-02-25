using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameDevTV.RTS.Units
{
    [RequireComponent(typeof(Collider))]
    public class DamageableSensor : MonoBehaviour
    {
        // State
        private readonly HashSet<IDamageable> damageables = new();
        public IList<IDamageable> GetDamageables() => damageables.ToList();
        
        // Events
        public delegate void UnitDetectionEvent(IDamageable damageable);
        public event UnitDetectionEvent onUnitEnter;
        public event UnitDetectionEvent onUnitExit;
        
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
    }
}
