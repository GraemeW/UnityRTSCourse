using System.Collections.Generic;
using UnityEngine;

namespace GameDevTV.RTS.Utilities
{
    public readonly struct ClosestGameObjectComparator : IComparer<GameObject>
    {
        private readonly Vector3 targetPosition;

        public ClosestGameObjectComparator(Vector3 position)
        {
            targetPosition = position;
        }

        public int Compare(GameObject x, GameObject y)
        {
            if (x == null || y == null) { return 0; }
            return (x.gameObject.transform.position - targetPosition).sqrMagnitude
                .CompareTo((y.gameObject.transform.position - targetPosition).sqrMagnitude);
        }
    }
}
