using System.Collections;
using GameDevTV.RTS.Utilities;
using UnityEngine;

namespace GameDevTV.RTS.Units
{
    public class Grenadier : BaseMilitaryUnit
    {
        // Tunables
        [Header("Hookups")]
        [SerializeField] private GameObject grenade;
        [SerializeField] private ParticleSystem explosionParticles;
        [Header("Parameters")]
        [SerializeField] private float throwTimeSeconds = 0.5f;
        [SerializeField] private float defaultEndForwardOffset = 3.0f;
        [SerializeField] private float hitEndUpOffset = 1.0f;
        
        // Cached References
        private Transform grenadeParent;
        private Vector3 defaultGrenadePosition;
        
        #region UnityMethods
        protected override void Awake()
        {
            base.Awake();
            if (grenade == null || explosionParticles == null)
            {
                Debug.LogError("Grenadier has not been configured!");
                return;
            }
            
            defaultGrenadePosition = grenade.transform.localPosition;
            grenadeParent = grenade.transform.parent;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (grenade != null) { Destroy(grenade); }
            if (explosionParticles != null) { Destroy(explosionParticles.gameObject); }
        }
        #endregion
        
        #region AnimationEvents
        public void OnThrowGrenade()
        {
            if (grenade == null) { return; }
            
            grenade.transform.SetParent(null); // detach from parent
            Vector3 startPosition = grenade.transform.position;
            Vector3 endPosition = startPosition + grenade.transform.forward * defaultEndForwardOffset;

            GameObject target = BehaviorConstants.GetTarget(behaviorAgent);
            Vector3? targetLocation = BehaviorConstants.GetTargetLocation(behaviorAgent);
            if (target != null)
            {
                endPosition = target.transform.position + Vector3.up * hitEndUpOffset;
            }
            else if (targetLocation != null)
            {
                endPosition = targetLocation.Value;
            }
            
            IDamageable targetDamageable = target != null ? target.GetComponent<IDamageable>() : null;
            StartCoroutine(AnimateGrenadeMovement(startPosition, endPosition, targetDamageable));
        }

        private IEnumerator AnimateGrenadeMovement(Vector3 startPosition, Vector3 endPosition, IDamageable targetDamageable)
        {
            float currentGrenadeThrowTime = 0f;
            while (currentGrenadeThrowTime < throwTimeSeconds)
            {
                float normalizedTime = Mathf.Clamp01(currentGrenadeThrowTime / throwTimeSeconds);
                grenade.transform.position = Vector3.Lerp(startPosition, endPosition, normalizedTime);
                
                currentGrenadeThrowTime += Time.deltaTime;
                yield return null;
            }

            TriggerExplosionEffect(endPosition);
            ApplyDamage(targetDamageable);
            
            grenade.transform.SetParent(grenadeParent);
            grenade.transform.localPosition = defaultGrenadePosition;
        }
        #endregion
        
        #region PrivateMethods
        private void TriggerExplosionEffect(Vector3 position)
        {
            if (explosionParticles == null) { return; }
            explosionParticles.transform.SetParent(null);
            explosionParticles.transform.position = position;
            explosionParticles.Play();
        }

        private void ApplyDamage(IDamageable targetDamageable)
        {
            if (targetDamageable == null || attackConfigSO == null) { return; }
            targetDamageable.AdjustHealth(-attackConfigSO.damage);
        }
        #endregion
    }
}
