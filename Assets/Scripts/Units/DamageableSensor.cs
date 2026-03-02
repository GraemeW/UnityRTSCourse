using System;
using System.Collections.Generic;
using System.Linq;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
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
            Bus<BuildingDeathEvent>.UnsubscribeFromEvent(HandleDamageableDeath);
            Bus<BuildingDeathEvent>.UnsubscribeFromEvent(HandleDamageableDeath);
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
        private void HandleTriggerEvent(IDamageable damageable, bool isEntry)
        {
            if (damageable == null) { return; }
            if (isEntry)
            {
                damageables.Add(damageable);
                onUnitEnter?.Invoke(damageable);
                
                if (damageables.Count == 1)
                {
                    Bus<UnitDeathEvent>.SubscribeToEvent(HandleDamageableDeath);
                    Bus<BuildingDeathEvent>.SubscribeToEvent(HandleDamageableDeath);
                }
            }
            else
            {
                damageables.Remove(damageable);
                onUnitExit?.Invoke(damageable);
                
                if (damageables.Count == 0)
                {
                    Bus<BuildingDeathEvent>.UnsubscribeFromEvent(HandleDamageableDeath);
                    Bus<BuildingDeathEvent>.UnsubscribeFromEvent(HandleDamageableDeath);
                }
            }
        }

        private void HandleDamageableDeath(UnitDeathEvent unitDeathEvent)
        {
            IDamageable damageable = unitDeathEvent.unit;
            if (!damageables.Contains(damageable)) { return; }
            HandleTriggerEvent(damageable, false);
        }

        private void HandleDamageableDeath(BuildingDeathEvent buildingDeathEvent)
        {
            IDamageable damageable = buildingDeathEvent.building;
            if (!damageables.Contains(damageable)) { return; }
            HandleTriggerEvent(damageable, false);
        }
        #endregion
    }
}
