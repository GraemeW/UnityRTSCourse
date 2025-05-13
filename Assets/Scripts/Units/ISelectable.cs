using System.Numerics;
using UnityEngine;

namespace GameDevTV.RTS.Units
{
    public interface ISelectable
    {
        void Select();
        void Deselect();

        void SetTarget(Transform target);

        void SetPosition(UnityEngine.Vector3 position);
    }
}
