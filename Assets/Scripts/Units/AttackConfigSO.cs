using UnityEngine;

namespace GameDevTV.RTS.Units
{
    [CreateAssetMenu(fileName = "AttackConfig", menuName = "Units/AttackConfig", order = 7)]
    public class AttackConfigSO : ScriptableObject
    {
        [field: SerializeField] public float maxChaseTime { get; private set; } = 1.0f;
        [field: SerializeField] public float damage { get; private set; } = 5.0f;
        [field: SerializeField] public float attackRange { get; private set; } = 1.5f;
        [field: SerializeField] public float attackDelay { get; private set; } = 1.0f;
        [field: SerializeField] public bool hasProjectileAttack { get; private set; } = false;
        [field: SerializeField] public bool isAreaOfEffect { get; private set; } = false;
        [field: SerializeField] public float areaOfEffectRadius { get; private set; } = 2.0f;
        [field: SerializeField] public int maxEnemiesHitPerAttack { get; private set; } = 5;
        [field: SerializeField] public LayerMask damageableLayers { get; private set; }

        #region PublicMethods
        public void ApplyDamage(Vector3 position, IDamageable targetDamageable, ref Collider[] enemyColliders)
        {
            if (!isAreaOfEffect)
            {
                if (targetDamageable == null) { return; }
                targetDamageable.AdjustHealth(-damage);
            }
            else
            {
                if (Physics.OverlapSphereNonAlloc(position, areaOfEffectRadius, enemyColliders, damageableLayers) == 0) { return; }
            
                foreach (Collider enemyCollider in enemyColliders)
                {
                    if (enemyCollider == null || !enemyCollider.TryGetComponent(out IDamageable damageable)) { continue; }
                    float scaledDamage = CalculateAreaOfEffectDamage(position, enemyCollider.transform.position);
                    damageable.AdjustHealth(-scaledDamage);
                }
            }
        }
        #endregion
        
        #region PrivateMethods
        private float CalculateAreaOfEffectDamage(Vector3 impactPoint, Vector3 targetPosition)
        {
            if (!isAreaOfEffect) { return 0; }
            
            float distance = Vector3.Distance(impactPoint, targetPosition);
            return damage * Mathf.Clamp01(1f - distance / areaOfEffectRadius);
        }
        #endregion
    }
}
