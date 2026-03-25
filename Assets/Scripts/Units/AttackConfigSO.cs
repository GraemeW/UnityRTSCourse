using UnityEngine;

namespace GameDevTV.RTS.Units
{
    [CreateAssetMenu(fileName = "AttackConfig", menuName = "Units/AttackConfig", order = 7)]
    public class AttackConfigSO : ScriptableObject
    {
        [field: SerializeField] public float attackRange { get; private set; } = 1.5f;
        [field: SerializeField] public float attackDelay { get; private set; } = 1.0f;
        [field: SerializeField] public bool hasProjectileAttack { get; private set; } = false;
        [field: SerializeField] public float damage { get; private set; } = 5.0f;
        [field: SerializeField] public float maxChaseTime { get; private set; } = 1.0f;
    }
}
