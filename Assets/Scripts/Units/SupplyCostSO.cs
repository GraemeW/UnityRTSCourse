using UnityEngine;
using GameDevTV.RTS.Environment;

namespace GameDevTV.RTS.Units
{
    [CreateAssetMenu(fileName = "SupplyCost", menuName = "SupplyCost", order = 5)]
    public class SupplyCostSO : ScriptableObject
    {
        [field: SerializeField] public int minerals { get; private set; } = 50;
        [field: SerializeField] public SupplySO mineralsSO { get; private set; }
        [field: SerializeField] public int gas { get; private set; } = 0;
        [field: SerializeField] public SupplySO gasSO { get; private set; }
    }
}
