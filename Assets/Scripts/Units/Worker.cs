using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

namespace GameDevTV.RTS.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Worker : MonoBehaviour, ISelectable, IMoveable
    {
        // Tunables
        [SerializeField] private DecalProjector decalProjector;

        // State
        private Transform target;
        private Vector3 targetPosition;

        // Cached References
        private NavMeshAgent navMeshAgent;

        public void Deselect()
        {
            if (decalProjector != null)
            {
                decalProjector.gameObject.SetActive(false);
            }

            Bus<UnitDeselectedEvent>.Raise(new UnitDeselectedEvent(this));
        }

        public void Select()
        {
            if (decalProjector != null)
            {
                decalProjector.gameObject.SetActive(true);
            }

            Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(this));
        }

        public void SetMoveTarget(Transform target)
        {
            if (target == null) { return; }
            this.target = target;
        }

        public void MoveTo(Vector3 position)
        {
            if (target != null) { target = null; }
            targetPosition = position;
            navMeshAgent.SetDestination(targetPosition);
        }

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            targetPosition = transform.position;
        }

        private void Update()
        {
            if (target != null) {
                targetPosition = target.position;
                navMeshAgent.SetDestination(targetPosition); 
            }
            
            if (navMeshAgent.isStopped) { targetPosition = transform.position; }
        }
    }
}
