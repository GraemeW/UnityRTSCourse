using GameDevTV.RTS.Environment;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevTV.RTS.Utilities
{
    public struct ClosestSupplyComparator : IComparer<GatherableSupply>
    {
        private Vector3 targetPosition;

        public ClosestSupplyComparator(Vector3 position)
        {
            targetPosition = position;
        }

        public int Compare(GatherableSupply x, GatherableSupply y)
        {
            return (x.gameObject.transform.position - targetPosition).sqrMagnitude
                .CompareTo((y.gameObject.transform.position - targetPosition).sqrMagnitude);
        }
    }
}
