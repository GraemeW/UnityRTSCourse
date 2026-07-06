using System;
using UnityEngine;

namespace GameDevTV.RTS.Units
{
    [CreateAssetMenu(fileName = "TransportConfig", menuName = "Units/TransportConfig", order = 6)]
    public class TransportConfigSO : ScriptableObject
    {
        [field: SerializeField] public int capacity { get; private set; }
        [field: SerializeField] public TransportSize transportSize { get; private set; }

        public int GetTransportCapacityUsage()
        {
            return transportSize switch
            {
                TransportSize.Small => 1,
                TransportSize.Medium => 2,
                TransportSize.Large => 4,
                _ => int.MaxValue
            };
        }
        
        public enum TransportSize
        {
            Small,
            Medium,
            Large,
            Untransportable
        }
        
    }
}