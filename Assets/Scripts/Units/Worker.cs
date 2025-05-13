using UnityEditor.UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

namespace GameDevTV.RTS.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Worker : MonoBehaviour, ISelectable
    {
        // Tunables
        [SerializeField] private DecalProjector decalProjector;
        [SerializeField] private Transform target;

        // State
        private Vector3 targetPosition;

        // Cached References
        private NavMeshAgent navMeshAgent;

        public void Deselect()
        {
            if (decalProjector != null)
            {
                decalProjector.gameObject.SetActive(false);
            }
        }

        public void Select()
        {
            if (decalProjector != null)
            {
                decalProjector.gameObject.SetActive(true);
            }
        }

        public void SetTarget(Transform target)
        {
            if (target == null) { return; }
            this.target = target;
        }

        public void SetPosition(Vector3 position)
        {
            if (target != null) { target = null; }
            targetPosition = position;
        }

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            targetPosition = transform.position;
        }

        private void Update()
        {
            if (target != null) { targetPosition = target.position; }
            if (Mathf.Approximately(Vector3.Distance(transform.position, targetPosition), 0.0f)) { return; }

            navMeshAgent.SetDestination(targetPosition);
            if (navMeshAgent.isStopped) { targetPosition = transform.position; }
        }
    }
}
