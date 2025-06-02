using UnityEngine;

namespace GameDevTV.RTS.Environment
{
    [CreateAssetMenu(fileName = "Supply", menuName = "Supply", order = 5)]
    public class SupplySO : ScriptableObject
    {
        // Tunables
        [field: SerializeField] public int maxAmount { get; private set; } = 1500;
        [field: SerializeField] public int amountPerGather { get; private set; } = 8;
        [field: SerializeField] public float baseGatherTime { get; private set; } = 1.5f;
    }
}
