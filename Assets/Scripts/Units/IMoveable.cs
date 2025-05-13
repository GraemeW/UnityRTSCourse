using UnityEngine;

namespace GameDevTV.RTS.Units
{
    public interface IMoveable
    {
        void SetMoveTarget(Transform target);

        void MoveTo(UnityEngine.Vector3 position);
    }
}
