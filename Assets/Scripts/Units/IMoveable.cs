using UnityEngine;

namespace GameDevTV.RTS.Units
{
    public interface IMoveable
    {
        void MoveTo(UnityEngine.Vector3 position);
        void SetMoveTarget(GameObject target);
        void Stop();
    }
}
