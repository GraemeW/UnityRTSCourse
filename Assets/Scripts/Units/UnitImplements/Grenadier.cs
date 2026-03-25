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
            if (grenade == null || explosionParticles == null) { return; }

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
            StartCoroutine(AnimateGrenadeMovement(startPosition, endPosition));
        }

        private IEnumerator AnimateGrenadeMovement(Vector3 startPosition, Vector3 endPosition)
        {
            float currentGrenadeThrowTime = 0f;
            while (currentGrenadeThrowTime < throwTimeSeconds)
            {
                float normalizedTime = Mathf.Clamp01(currentGrenadeThrowTime / throwTimeSeconds);
                grenade.transform.position = Vector3.Lerp(startPosition, endPosition, normalizedTime);
                
                currentGrenadeThrowTime += Time.deltaTime;
                yield return null;
            }

            if (explosionParticles != null)
            {
                explosionParticles.transform.SetParent(null);
                explosionParticles.transform.position = endPosition;
                explosionParticles.Play();
            }
            
            grenade.transform.SetParent(grenadeParent);
            grenade.transform.localPosition = defaultGrenadePosition;
        }
        #endregion
    }
}
