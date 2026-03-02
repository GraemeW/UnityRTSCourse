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
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Self.Value == null) { return Status.Failure; }
        if (accumulatedChaseTime > AttackConfig.Value.maxChaseTime) { abstractUnit.SetNearestEnemyToTarget(true); }
        if (Target.Value == null || targetDamageable.GetCurrentHealth() == 0) { return Status.Success; }
        
        ResetAnimation();
        if (IsMovingToTarget())
        {
            if (animator != null) { AnimationConstants.AnimateMovement(animator, navMeshAgent.speed); }
            accumulatedChaseTime += Time.deltaTime;
            return Status.Running;
        }

        Quaternion lookRotation = Quaternion.LookRotation((targetTransform.position - selfTransform.position).normalized, Vector3.up);
        selfTransform.rotation = Quaternion.Euler(
            selfTransform.rotation.eulerAngles.x,
            lookRotation.eulerAngles.y,
            selfTransform.rotation.eulerAngles.z);
        
        Self.Value.transform.LookAt(Target.Value.transform.position, Self.Value.transform.up);
        if (IsCooldownElapsed())
        {
            if (animator != null) { AnimationConstants.AnimateAttack(animator, true); }
            if (abstractUnit.attackingParticleSystem != null) { abstractUnit.attackingParticleSystem.Play(); }
            targetDamageable.AdjustHealth(-AttackConfig.Value.damage);
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        ResetAnimation();
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

    private void ResetAnimation()
    {
        if (animator == null) { return; }
        AnimationConstants.AnimateMovement(animator, 0f);
        AnimationConstants.AnimateAttack(animator, false);
    }
    
    private bool HasValidInputs()
    {
        bool isSelfValid = Self.Value != null && Self.Value.TryGetComponent(out AbstractUnit _) && Self.Value.TryGetComponent(out NavMeshAgent _);
        bool isTargetValid = Target.Value != null && Target.Value.TryGetComponent(out IDamageable _);
        bool isAttackConfigValid = AttackConfig.Value != null && NearbyEnemies.Value != null;
        return isSelfValid && isTargetValid && isAttackConfigValid;
    }
    #endregion
}

