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
