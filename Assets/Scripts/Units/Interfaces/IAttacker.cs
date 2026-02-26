using UnityEngine;

namespace GameDevTV.RTS.Units
{
    public interface IAttacker
    {
        // Properties
        public GameObject unitGameObject { get; }
        public Transform unitTransform { get; }
        
        // Interface Methods
        public void Attack(IDamageable damageable);
        public void Attack(Vector3 targetLocation);
    }
}
