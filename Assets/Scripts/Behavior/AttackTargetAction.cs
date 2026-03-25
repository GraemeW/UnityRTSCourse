using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using GameDevTV.RTS.Units;
using GameDevTV.RTS.Utilities;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AttackTarget", story: "[Self] attacks [Target] until it dies", category: "Action", id: "be9d99cb08cdc6bb76b70eecffe04e0b")]
public partial class AttackTargetAction : Action
{
    // Tunables
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<AttackConfigSO> AttackConfig;
    [SerializeReference] public BlackboardVariable<List<GameObject>> NearbyEnemies;

    // Cached References
    private Transform selfTransform;
    private AbstractUnit abstractUnit;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private IDamageable targetDamageable;
    private Transform targetTransform;
    
    // State
    private float lastAttackTime;
    private float accumulatedChaseTime;
    private Collider[] enemyColliders;
    
    #region UnityMethods
    protected override Status OnStart()
    {
        if (!HasValidInputs()) { return Status.Failure; }
        
        selfTransform = Self.Value.transform;
        abstractUnit = selfTransform.GetComponent<AbstractUnit>();
        navMeshAgent = selfTransform.GetComponent<NavMeshAgent>();
        animator = selfTransform.GetComponent<Animator>();
        
        targetTransform = Target.Value.transform;
        targetDamageable = targetTransform.GetComponent<IDamageable>();
        
        enemyColliders = new Collider[AttackConfig.Value.maxEnemiesHitPerAttack];
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Self.Value == null) { return Status.Failure; }
        if (accumulatedChaseTime > AttackConfig.Value.maxChaseTime) { abstractUnit.SetNearestEnemyToTarget(true); }
        if (Target.Value == null || targetDamageable.GetCurrentHealth() == 0) { return Status.Success; }
        
        if (animator != null) { AnimationConstants.AnimateMovement(animator, 0f); }
        if (IsMovingToTarget())
        {
            AnimateMovement();
            accumulatedChaseTime += Time.deltaTime;
            return Status.Running;
        }

        LookAtTarget();
        if (IsCooldownElapsed())
        {
            AnimateAttack();
            if (AttackConfig.Value.hasProjectileAttack) { return Status.Running; }  // Projectile attacks to be handled as a consequence of the animation / in the specific subclass
            AttackConfig.Value.ApplyDamage(targetDamageable.unitTransform.position, targetDamageable, ref enemyColliders);
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (animator != null)
        {
            AnimationConstants.AnimateAttack(animator, false);
            AnimationConstants.AnimateMovement(animator, 0f);
        }
    }
    #endregion

    #region PrivateMethods
    private bool IsMovingToTarget()
    {
        if (!NearbyEnemies.Value.Contains(Target.Value))
        {
            navMeshAgent.SetDestination(targetTransform.position);
            navMeshAgent.isStopped = false;
            lastAttackTime = Time.time;
            return true;
        }
        navMeshAgent.isStopped = true;
        return false;
    }

    private bool IsCooldownElapsed()
    {
        if (Time.time < lastAttackTime + AttackConfig.Value.attackDelay) { return false; }
        lastAttackTime = Time.time;
        return true;
    }
    
    private bool HasValidInputs()
    {
        bool isSelfValid = Self.Value != null && Self.Value.TryGetComponent(out AbstractUnit _) && Self.Value.TryGetComponent(out NavMeshAgent _);
        bool isTargetValid = Target.Value != null && Target.Value.TryGetComponent(out IDamageable _);
        bool isAttackConfigValid = AttackConfig.Value != null && NearbyEnemies.Value != null;
        return isSelfValid && isTargetValid && isAttackConfigValid;
    }

    private void AnimateMovement()
    {
        if (animator == null) return;
        AnimationConstants.AnimateAttack(animator, false);
        AnimationConstants.AnimateMovement(animator, navMeshAgent.speed);
    }

    private void AnimateAttack()
    {
        if (animator != null) { AnimationConstants.AnimateAttack(animator, true); }
        if (abstractUnit.attackingParticleSystem != null) { abstractUnit.attackingParticleSystem.Play(); }
    }

    private void LookAtTarget()
    {
        Quaternion lookRotation = Quaternion.LookRotation((targetTransform.position - selfTransform.position).normalized, Vector3.up);
        selfTransform.rotation = Quaternion.Euler(
            selfTransform.rotation.eulerAngles.x,
            lookRotation.eulerAngles.y,
            selfTransform.rotation.eulerAngles.z);
        Self.Value.transform.LookAt(Target.Value.transform.position, Self.Value.transform.up);
    }
    #endregion
}
