using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Environment
{
    public class GatherableSupply : MonoBehaviour, IGatherable, ISelectable
    {
        #region Interface
        [field: SerializeField] public SupplySO supply {  get; private set; }
        [field: SerializeField] public int amount { get; private set; }
        [field: SerializeField] public bool isBusy { get; private set; }

        public bool BeginGather()
        {
            if (isBusy || amount <= 0) { return false; }

            isBusy = true;
            return true;
        }

        public int EndGather()
        {
            isBusy = false;

            if (supply == null) { return 0; }

            int amountGathered = Mathf.Min(amount, supply.amountPerGather);
            amount -= amountGathered;

            if (amount <= 0) { Destroy(gameObject); }

            return amountGathered;
        }

        public void Select()
        {
            
        }

        public void Deselect()
        {
            
        }
        #endregion

        #region UnityMethods
        private void Start()
        {
            if (supply == null) { return; }

            amount = supply.maxAmount;
            isBusy = false;
        }

        #endregion
    }
}
