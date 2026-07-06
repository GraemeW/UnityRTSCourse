using System.Collections.Generic;
using UnityEngine;

namespace GameDevTV.RTS.Units
{
    public interface ITransporter
    {
        public Transform transform { get; }
        public int maxCapacity {  get; }
        public int usedCapacity {  get; }

        public IList<ITransportable> GetLoadedUnits();

        public void Load(ITransportable transportableUnit);
        public void Load(ITransportable[] transportableUnits);
        
        public bool Unload(ITransportable transportableUnit);
        public bool UnloadAll();
    }
}
