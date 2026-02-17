using UnityEngine;
using UnityEngine.AI;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "BuildingRestriction", menuName = "Buildings/Restrictions", order = 7)]
    public class BuildingRestrictionSO : ScriptableObject
    {
        [Header("Properties")] 
        [field: SerializeField] public Vector3 extents { get; private set; } = Vector3.one;
        [Header("LayerDistance Checking")] 
        [field: SerializeField] public OverlapStyle hitDetectionStyle { get; private set; } = OverlapStyle.Sphere;
        [field: SerializeField] public float radius { get; private set; } = 1f;
        [field: SerializeField] public LayerMask layerMask { get; private set; } 
        [Header("NavMesh Checking")] 
        [field: SerializeField] public bool mustBeFullyOnNavMesh { get; private set; } = true;
        [field: SerializeField] public int navMeshAgentTypeID { get; private set; }
        [field: SerializeField] public float navMeshTolerance { get; private set; } = 0.25f;

        private readonly Collider[] hitColliders = new Collider[1];
        
        public bool CanPlace(Vector3 position)
        {
            NavMeshQueryFilter queryFilter = new()
            {
                areaMask = NavMesh.AllAreas,
                agentTypeID = navMeshAgentTypeID
            };
            return IsOutsideLayerRadius(position) && IsFullyOnNavMesh(position, queryFilter);
        }

        private bool IsOutsideLayerRadius(Vector3 position)
        {
            return hitDetectionStyle switch
            {
                OverlapStyle.Sphere => Physics.OverlapSphereNonAlloc(position, radius, hitColliders, layerMask) == 0,
                OverlapStyle.Box => Physics.OverlapBoxNonAlloc(position, extents, hitColliders, Quaternion.identity, layerMask) == 0,
                _ => false
            };
        }

        private bool IsFullyOnNavMesh(Vector3 position, NavMeshQueryFilter queryFilter)
        {
            if (!mustBeFullyOnNavMesh) { return true; }
            
            bool isOnNavMesh = NavMesh.SamplePosition(position, out NavMeshHit _, navMeshTolerance, queryFilter);
            isOnNavMesh = isOnNavMesh && NavMesh.SamplePosition(position + new Vector3(extents.x, 0, extents.z), out NavMeshHit _, navMeshTolerance, queryFilter);
            isOnNavMesh = isOnNavMesh && NavMesh.SamplePosition(position + new Vector3(extents.x, 0, -extents.z), out NavMeshHit _, navMeshTolerance, queryFilter);
            isOnNavMesh = isOnNavMesh && NavMesh.SamplePosition(position + new Vector3(-extents.x, 0, -extents.z), out NavMeshHit _, navMeshTolerance, queryFilter);
            isOnNavMesh = isOnNavMesh && NavMesh.SamplePosition(position + new Vector3(-extents.x, 0, extents.z), out NavMeshHit _, navMeshTolerance, queryFilter);
            return isOnNavMesh;
        }

        public enum OverlapStyle
        {
            Sphere,
            Box
        }
    }
}
