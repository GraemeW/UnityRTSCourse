using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace GameDevTV.RTS.Units
{
    [RequireComponent(typeof(NavMeshAgent), typeof(BehaviorGraphAgent))]
    public abstract class AbstractUnit : AbstractCommandable, IMoveable
    {
        // Static Behavior References
        // Note:  These MUST match the variables in the behavior tree blackboard
        public static string commandRef { get; private set; } = "Command";
        public static string targetLocationRef { get; private set; } = "TargetLocation";
        public static string targetRef { get; private set; } = "Target";

        // Cached References
        private NavMeshAgent navMeshAgent;
        public float agentRadius => navMeshAgent.radius;
        protected BehaviorGraphAgent behaviorAgent;

        #region UnityMethods
        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            behaviorAgent = GetComponent<BehaviorGraphAgent>();
            behaviorAgent.SetVariableValue(commandRef, UnitCommands.Stop);
        }

        protected override void Start()
        {
            base.Start();
            Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));
        }

        private void OnDestroy()
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
            behaviorAgent.SetVariableValue(targetLocationRef, position);
            behaviorAgent.SetVariableValue(commandRef, UnitCommands.Move);
        }

        public void SetMoveTarget(Transform target)
        {
            behaviorAgent.SetVariableValue(targetRef, target);
            behaviorAgent.SetVariableValue(commandRef, UnitCommands.Move);
        }

        public void Stop()
        {
            behaviorAgent.SetVariableValue(commandRef, UnitCommands.Stop);
        }
        #endregion
    }
}
