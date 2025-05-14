using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using UnityEngine;
using UnityEngine.AI;

namespace GameDevTV.RTS.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class AbstractUnit : AbstractCommandable, IMoveable
    {
        // State
        private Transform target;
        private Vector3 targetPosition;

        // Cached References
        private NavMeshAgent navMeshAgent;
        public float agentRadius => navMeshAgent.radius;

        #region UnityMethods
        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            targetPosition = transform.position;
        }

        private void Start()
        {
            Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));
        }

        private void OnDestroy()
        {
            Bus<UnitDespawnEvent>.Raise(new UnitDespawnEvent(this));
        }

        private void Update()
        {
            if (target != null)
            {
                targetPosition = target.position;
                navMeshAgent.SetDestination(targetPosition);
            }

            if (navMeshAgent.isStopped) { targetPosition = transform.position; }
        }
        #endregion

        #region Movement
        public void MoveTo(Vector3 position)
        {
            if (target != null) { target = null; }
            targetPosition = position;
            navMeshAgent.SetDestination(targetPosition);
        }

        public void SetMoveTarget(Transform target)
        {
            if (target == null) { return; }
            this.target = target;
        }
        #endregion
    }
}
