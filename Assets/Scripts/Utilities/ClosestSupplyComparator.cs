using GameDevTV.RTS.Environment;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevTV.RTS.Utilities
{
    public readonly struct ClosestSupplyComparator : IComparer<GatherableSupply>
    {
        private readonly Vector3 targetPosition;

        public ClosestSupplyComparator(Vector3 position)
        {
            targetPosition = position;
        }

        public int Compare(GatherableSupply x, GatherableSupply y)
        {
            if (x == null || y == null) { return 0; }
            return (x.gameObject.transform.position - targetPosition).sqrMagnitude
                .CompareTo((y.gameObject.transform.position - targetPosition).sqrMagnitude);
        }
    }
}
