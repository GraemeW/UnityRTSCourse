using System;
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

    // Cached References
    private NavMeshAgent navMeshAgent;
    private Transform selfTransform;
    private Animator animator;
    private IDamageable targetDamageable;
    private Transform targetTransform;
    
    // State
    private float lastAttackTime;
    
    #region UnityMethods
    protected override Status OnStart()
    {
        if (!HasValidInputs()) { return Status.Failure; }
        
        selfTransform = Self.Value.transform;
        navMeshAgent = selfTransform.GetComponent<NavMeshAgent>();
        animator = selfTransform.GetComponent<Animator>();
        
        targetTransform = Target.Value.transform;
        targetDamageable = targetTransform.GetComponent<IDamageable>();

        if (animator != null) { AnimationConstants.AnimateAttack(animator, true); }
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Self.Value == null) { return Status.Failure; }
        if (Target.Value == null || targetDamageable.GetCurrentHealth() == 0) { return Status.Success; }
        
        if (IsMovingToTarget()) { return Status.Running; }
        if (IsCooldownElapsed()) { targetDamageable.AdjustHealth(-AttackConfig.Value.damage);}

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (animator != null) { AnimationConstants.AnimateAttack(animator, false); }
    }
    #endregion

    #region PrivateMethods

    private bool IsMovingToTarget()
    {
        if (Vector3.Distance(targetTransform.position, selfTransform.position) > AttackConfig.Value.attackRange)
        {
            navMeshAgent.SetDestination(targetTransform.position);
            navMeshAgent.isStopped = false;
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
        bool isSelfValid = Self.Value != null && Self.Value.TryGetComponent(out NavMeshAgent _);
        bool isTargetValid = Target.Value != null && Target.Value.TryGetComponent(out IDamageable _);
        bool isAttackConfigValid = AttackConfig.Value != null;
        return isSelfValid && isTargetValid && isAttackConfigValid;
    }
    #endregion
}

