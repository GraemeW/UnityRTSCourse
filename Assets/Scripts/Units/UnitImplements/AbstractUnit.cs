using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Utilities;

namespace GameDevTV.RTS.Units
{
    [RequireComponent(typeof(NavMeshAgent), typeof(BehaviorGraphAgent))]
    public abstract class AbstractUnit : AbstractCommandable, IMoveable
    {
        // Cached References
        private NavMeshAgent navMeshAgent;
        public float agentRadius => navMeshAgent.radius;
        protected BehaviorGraphAgent behaviorAgent;

        #region UnityMethods
        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            behaviorAgent = GetComponent<BehaviorGraphAgent>();
            BehaviorConstants.SetCommand(behaviorAgent, UnitCommands.Stop);
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

        #region Movement
        public void WarpTo(Vector3 position)
        {
            navMeshAgent.Warp(position);
        }

        public void ToggleAvoidance(bool enable)
        {
            if (enable)
            {
                navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            }
            else
            {
                navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            }
        }

        public void MoveTo(Vector3 position)
        {
            BehaviorConstants.SetTargetLocation(behaviorAgent, position);
            BehaviorConstants.SetCommand(behaviorAgent, UnitCommands.Move);
        }

        public void SetMoveTarget(GameObject target)
        {
            BehaviorConstants.SetTarget(behaviorAgent, target);
            BehaviorConstants.SetCommand(behaviorAgent, UnitCommands.Move);
        }

        public void Stop()
        {
            BehaviorConstants.SetCommand(behaviorAgent, UnitCommands.Stop);
        }
        #endregion
    }
}
