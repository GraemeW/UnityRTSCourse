using UnityEngine;

namespace GameDevTV.RTS.Units
{
    public interface ITransportable
    {
        public Transform transform { get; }
        public int transportCapacityUsage { get; }

        public void LoadInto(ITransporter transporter);
    }
}
