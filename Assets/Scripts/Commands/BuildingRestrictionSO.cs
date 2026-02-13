using System;
using UnityEngine;
using UnityEngine.AI;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "BuildingRestriction", menuName = "Buildings/Restrictions", order = 7)]
    public class BuildingRestrictionSO : ScriptableObject
    {
        [Header("Properties")]
        [field: SerializeField] public Vector3 Extents { get; private set; } = Vector3.one;
        [Header("LayerDistance Checking")]
        [field: SerializeField] public OverlapStyle HitDetectionStyle { get; private set; } = OverlapStyle.Sphere;
        [field: SerializeField] public float Radius { get; private set; } = 1f;
        [field: SerializeField] public LayerMask LayerMask { get; private set; } 
        [Header("NavMesh Checking")]
        [field: SerializeField] public bool MustBeFullyOnNavMesh { get; private set; } = true;
        [field: SerializeField] public int NavMeshAgentTypeID { get; private set; }
        [field: SerializeField] public float NavMeshTolerance { get; private set; } = 0.25f;

        private readonly Collider[] hitColliders = new Collider[1];
        
        public bool CanPlace(Vector3 position)
        {
            NavMeshQueryFilter queryFilter = new()
            {
                areaMask = NavMesh.AllAreas,
                agentTypeID = NavMeshAgentTypeID
            };
            return IsOutsideLayerRadius(position) && IsFullyOnNavMesh(position, queryFilter);
        }

        private bool IsOutsideLayerRadius(Vector3 position)
        {
            return HitDetectionStyle switch
            {
                OverlapStyle.Sphere => Physics.OverlapSphereNonAlloc(position, Radius, hitColliders, LayerMask) == 0,
                OverlapStyle.Box => Physics.OverlapBoxNonAlloc(position, Extents, hitColliders, Quaternion.identity, LayerMask) == 0,
                _ => false
            };
        }

        private bool IsFullyOnNavMesh(Vector3 position, NavMeshQueryFilter queryFilter)
        {
            if (!MustBeFullyOnNavMesh) { return true; }
            
            bool isOnNavMesh = NavMesh.SamplePosition(position, out NavMeshHit _, NavMeshTolerance, queryFilter);
            isOnNavMesh = isOnNavMesh && NavMesh.SamplePosition(position + new Vector3(Extents.x, 0, Extents.z), out NavMeshHit _, NavMeshTolerance, queryFilter);
            isOnNavMesh = isOnNavMesh && NavMesh.SamplePosition(position + new Vector3(Extents.x, 0, -Extents.z), out NavMeshHit _, NavMeshTolerance, queryFilter);
            isOnNavMesh = isOnNavMesh && NavMesh.SamplePosition(position + new Vector3(-Extents.x, 0, -Extents.z), out NavMeshHit _, NavMeshTolerance, queryFilter);
            isOnNavMesh = isOnNavMesh && NavMesh.SamplePosition(position + new Vector3(-Extents.x, 0, Extents.z), out NavMeshHit _, NavMeshTolerance, queryFilter);
            return isOnNavMesh;
        }

        public enum OverlapStyle
        {
            Sphere,
            Box
        }
    }
}
