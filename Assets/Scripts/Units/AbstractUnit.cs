using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Utilities;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

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
            behaviorAgent.SetVariableValue(BehaviorConstants.commandRef, UnitCommands.Stop);
        }

        protected override void Start()
        {
            base.Start();
            Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));
        }

        protected virtual void OnDestroy()
        {
            Bus<UnitDespawnEvent>.Raise(new UnitDespawnEvent(this));
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
            behaviorAgent.SetVariableValue(BehaviorConstants.targetLocationRef, position);
            behaviorAgent.SetVariableValue(BehaviorConstants.commandRef, UnitCommands.Move);
        }

        public void SetMoveTarget(GameObject target)
        {
            behaviorAgent.SetVariableValue(BehaviorConstants.targetRef, target);
            behaviorAgent.SetVariableValue(BehaviorConstants.commandRef, UnitCommands.Move);
        }

        public void Stop()
        {
            behaviorAgent.SetVariableValue(BehaviorConstants.commandRef, UnitCommands.Stop);
        }
        #endregion
    }
}
