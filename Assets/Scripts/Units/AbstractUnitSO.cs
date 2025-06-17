using UnityEngine;

namespace GameDevTV.RTS.Units
{
    public abstract class AbstractUnitSO : ScriptableObject
    {
        [field: SerializeField] public Sprite icon { get; private set; }
        [field: SerializeField] public int health { get; private set; }
        [field: SerializeField] public GameObject prefab { get; private set; }
        [field: SerializeField] public float buildTime { get; private set; }
    }
}
