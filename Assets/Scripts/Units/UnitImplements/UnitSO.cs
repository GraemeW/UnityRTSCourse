using UnityEngine;

namespace GameDevTV.RTS.Units
{
    [CreateAssetMenu(fileName = "Unit", menuName = "Units/Unit")]
    public class UnitSO : AbstractUnitSO
    {
        [field: SerializeField] public AttackConfigSO attackConfig { get; private set; }
        [field: SerializeField] public TransportConfigSO transportConfig { get; private set; }
    }
}
