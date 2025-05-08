using UnityEngine;
using UnityEngine.AI;

namespace GameDevTV.RTS.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Worker : MonoBehaviour
    {
        // Tunables
        [SerializeField] private Transform target;

        // Cached References
        private NavMeshAgent navMeshAgent;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (target != null)
            {
                navMeshAgent.SetDestination(target.position);
            }
        }
    }
}
