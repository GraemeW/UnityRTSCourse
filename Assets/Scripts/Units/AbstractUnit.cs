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
        public static string targetLocationRef = "TargetLocation";
        public static string targetRef = "Target";

        // Cached References
        private NavMeshAgent navMeshAgent;
        public float agentRadius => navMeshAgent.radius;
        private BehaviorGraphAgent behaviorAgent;

        #region UnityMethods
        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            behaviorAgent = GetComponent<BehaviorGraphAgent>();
            MoveTo(transform.position);
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

        public void MoveTo(Vector3 position)
        {
            behaviorAgent.SetVariableValue(targetLocationRef, position);
        }

        public void SetMoveTarget(Transform target)
        {
            behaviorAgent.SetVariableValue(targetRef, target);
        }
        #endregion
    }
}
