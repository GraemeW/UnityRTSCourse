using UnityEngine;

namespace GameDevTV.RTS.Units
{
    [CreateAssetMenu(fileName = "Unit", menuName = "Unit/Unit")]
    public class UnitSO : ScriptableObject
    {
        [field: SerializeField] public int health { get; private set; }
        [field: SerializeField] public GameObject prefab { get; private set; }
        [field: SerializeField] public float buildTime { get; private set; }
    }
}
