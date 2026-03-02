using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Utilities;

namespace GameDevTV.RTS.Units
{
    [RequireComponent(typeof(NavMeshAgent), typeof(BehaviorGraphAgent))]
    public abstract class AbstractUnit : AbstractCommandable, IMoveable, IAttacker
    {
        // Hookups
        [field: SerializeField] public ParticleSystem attackingParticleSystem { get; private set; }
        [SerializeField] private DamageableSensor damageableSensor;
        
        // Cached References
        private UnitSO unitSOImpl;
        private NavMeshAgent navMeshAgent;
        public float agentRadius => navMeshAgent.radius;
        protected BehaviorGraphAgent behaviorAgent;

        #region UnityMethods
        private void Awake()
        {
            unitSOImpl = unitSO as UnitSO;
            navMeshAgent = GetComponent<NavMeshAgent>();
            behaviorAgent = GetComponent<BehaviorGraphAgent>();
            BehaviorConstants.SetCommand(behaviorAgent, UnitCommands.Stop);
            if (unitSOImpl != null) { BehaviorConstants.SetAttackConfig(behaviorAgent, unitSOImpl.attackConfig); }
        }

        private void OnEnable()
        {
            SetupDamageableSensor(true);
        }

        private void OnDisable()
        {
            SetupDamageableSensor(false);
        }
        
        protected override void Start()
        {
            base.Start();
            currentHealth = unitSO.health;
            maxHealth = unitSO.health;
            Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));
        }

        protected virtual void OnDestroy()
        {
            Bus<UnitDeathEvent>.Raise(new UnitDeathEvent(this));
        }
        #endregion
        
        #region Sensors
        private void SetupDamageableSensor(bool enable)
        {
            if (damageableSensor == null) { return; }

            if (enable)
            {
                damageableSensor.onUnitEnter += HandleUnitEnter;
                damageableSensor.onUnitExit += HandleUnitExit;
                if (unitSOImpl != null) { damageableSensor.SetupFrom(unitSOImpl.attackConfig); }
            }
            else
            {
                damageableSensor.onUnitEnter -= HandleUnitEnter;
                damageableSensor.onUnitExit -= HandleUnitExit;
            }
        }
        
        private void HandleUnitEnter(IDamageable damageable)
        {
            // Note:  Do not early exit on null damageable here (handled downstream)
            if (behaviorAgent == null) { return; }
            BehaviorConstants.AddToNearbyEnemies(behaviorAgent, damageable);
            
            if (BehaviorConstants.GetCommand(behaviorAgent) != UnitCommands.Attack) { return; }

            SetNearestEnemyToTarget();
        }
        
        private void HandleUnitExit(IDamageable damageable)
        {
            // Note:  Do not early exit on null damageable here (handled downstream)
            if (behaviorAgent == null) { return; }
            BehaviorConstants.RemoveFromNearbyEnemies(behaviorAgent, damageable);
            
            if (BehaviorConstants.GetCommand(behaviorAgent) != UnitCommands.Attack) { return; }
            if (!IsDamageableCurrentTarget(damageable)) { return; }

            SetNearestEnemyToTarget();
        }

        private void SetNearestEnemyToTarget()
        {
            GameObject nearestTarget = BehaviorConstants.GetNearestEnemy(behaviorAgent);
            BehaviorConstants.SetTarget(behaviorAgent, nearestTarget);
        }

        private bool IsDamageableCurrentTarget(IDamageable damageable)
        {
            GameObject currentTarget = BehaviorConstants.GetTarget(behaviorAgent);
            return currentTarget != null && damageable != null && currentTarget == damageable.unitGameObject;
        }
        #endregion

        #region Movement
        public void WarpTo(Vector3 position)
        {
            navMeshAgent.Warp(position);
        }

        public void ToggleAvoidance(bool enable)
        {
            navMeshAgent.obstacleAvoidanceType = enable ? ObstacleAvoidanceType.HighQualityObstacleAvoidance : ObstacleAvoidanceType.NoObstacleAvoidance;
        }

        public void MoveTo(Vector3 position)
        {
            BehaviorConstants.SetTarget(behaviorAgent, null);
            BehaviorConstants.SetTargetLocation(behaviorAgent, position);
            BehaviorConstants.SetCommand(behaviorAgent, UnitCommands.Move);
        }

        public void SetMoveTarget(GameObject target)
        {
            BehaviorConstants.SetTargetLocation(behaviorAgent, transform.position);
            BehaviorConstants.SetTarget(behaviorAgent, target);
            BehaviorConstants.SetCommand(behaviorAgent, UnitCommands.Move);
        }

        public void Stop()
        {
            BehaviorConstants.SetCommand(behaviorAgent, UnitCommands.Stop);
        }
        #endregion
        
        #region Attack

        public void Attack(IDamageable damageable)
        {
            if (damageable == null) { return; }
            if (damageable == (IDamageable)this) { return; }
            
            BehaviorConstants.SetTargetLocation(behaviorAgent, transform.position);
            BehaviorConstants.SetTarget(behaviorAgent, damageable.unitGameObject);
            BehaviorConstants.SetCommand(behaviorAgent, UnitCommands.Attack);
        }

        public void Attack(Vector3 targetLocation)
        {
            BehaviorConstants.SetTarget(behaviorAgent, null);
            BehaviorConstants.SetTargetLocation(behaviorAgent, targetLocation);
            BehaviorConstants.SetCommand(behaviorAgent, UnitCommands.Attack);
            HandleUnitEnter(null);
        }
        #endregion
    }
}
