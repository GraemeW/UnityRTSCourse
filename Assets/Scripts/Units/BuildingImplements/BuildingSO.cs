using UnityEngine;

namespace GameDevTV.RTS.Units
{
    [CreateAssetMenu(fileName = "Building", menuName = "Buildings/Building")]
    public class BuildingSO : AbstractUnitSO
    {
        [field: SerializeField] public Material placementMaterial { get; private set; }
    }
}
