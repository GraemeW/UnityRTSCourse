using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using UnityEngine;

namespace GameDevTV.RTS.Units
{
    public abstract class AbstractUnitSO : ScriptableObject
    {
        [field: SerializeField] public Sprite icon { get; private set; }
        [field: SerializeField] public int health { get; private set; }
        [field: SerializeField] public GameObject prefab { get; private set; }
        [field: SerializeField] public float buildTime { get; private set; }
        [field: SerializeField] public SupplyCostSO cost { get; private set; }

        public void ChargeSupplies(float chargePreMultiplier = 1f)
        {
            chargePreMultiplier = Mathf.Clamp(chargePreMultiplier, 0f, 1f);
            
            if (cost != null && cost.mineralsSO != null) { Bus<SupplyEvent>.Raise(new SupplyEvent(cost.mineralsSO, Mathf.RoundToInt(-chargePreMultiplier * cost.minerals))); }
            if (cost != null && cost.gasSO != null) { Bus<SupplyEvent>.Raise(new SupplyEvent(cost.gasSO, Mathf.RoundToInt(-chargePreMultiplier * cost.gas))); }
        }

        public void RefundSupplies(float chargePreMultiplier = 1f)
        {
            chargePreMultiplier = Mathf.Clamp(chargePreMultiplier, 0f, 1f);
            
            if (cost != null && cost.mineralsSO != null) { Bus<SupplyEvent>.Raise(new SupplyEvent(cost.mineralsSO, Mathf.RoundToInt(chargePreMultiplier * cost.minerals))); }
            if (cost != null && cost.gasSO != null) { Bus<SupplyEvent>.Raise(new SupplyEvent(cost.gasSO, Mathf.RoundToInt(chargePreMultiplier * cost.gas))); }
        }
    }
}
