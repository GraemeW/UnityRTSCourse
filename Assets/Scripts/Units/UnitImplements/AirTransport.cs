using System.Collections.Generic;
using GameDevTV.RTS.Utilities;
using UnityEngine;

namespace GameDevTV.RTS.Units
{
    public class AirTransport : AbstractUnit, ITransporter
    {
        // Properties
        public int maxCapacity => unitSOImpl != null ? unitSOImpl.transportConfig.capacity : 0;
        public int usedCapacity { get; private set; }
        
        // Cached References
        LoadUnitEventChannel loadUnitEventChannel;
        
        protected override void Start()
        {
            base.Start();

            loadUnitEventChannel = BehaviorConstants.GetLoadUnitEventChannel(behaviorAgent);
            if (loadUnitEventChannel != null)
            {
                loadUnitEventChannel.Event -= HandleLoadUnit;
                loadUnitEventChannel.Event += HandleLoadUnit;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (loadUnitEventChannel != null)
            {
                loadUnitEventChannel.Event -= HandleLoadUnit;
            }
        }
        
        protected override void ReconcileContingentCommands()
        {
            // Not currently used
        }

        #region TransporterMethods
        private void HandleLoadUnit(GameObject self, GameObject target)
        {
            if (self == null || target == null) { return; }
            if (!target.TryGetComponent(out ITransportable transportable)) { return; }
            
            target.SetActive(false);
            target.transform.SetParent(self.transform);
            usedCapacity += transportable.transportCapacityUsage;
            Debug.Log($"Load unit {target.name}");
        }
        public IList<ITransportable> GetLoadedUnits()
        {
            throw new System.NotImplementedException();
        }

        public void Load(ITransportable transportableUnit)
        {
            if (transportableUnit == null || transportableUnit.transform == null) { return; }
            if (usedCapacity + transportableUnit.transportCapacityUsage > maxCapacity) { return; }
            
            BehaviorConstants.SetTarget(behaviorAgent, transportableUnit.transform.gameObject);
            BehaviorConstants.SetCommand(behaviorAgent, UnitCommands.LoadUnits);
        }

        public void Load(ITransportable[] transportableUnits)
        {
            throw new System.NotImplementedException();
        }

        public bool Unload(ITransportable transportableUnit)
        {
            throw new System.NotImplementedException();
        }

        public bool UnloadAll()
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
