using UnityEngine;

namespace GameDevTV.RTS.Environment
{
    public class GatherableSupply : MonoBehaviour, IGatherable
    {
        #region Interface
        [field: SerializeField] public SupplySO supply {  get; private set; }
        public int amount { get; private set; }
        public bool isBusy { get; private set; }

        public bool BeginGather()
        {
            if (isBusy || amount <= 0) { return false; }

            isBusy = true;
            return true;
        }

        public int EndGather()
        {
            if (supply == null) { return 0; }

            isBusy = false;
            int amountGathered = Mathf.Min(amount, supply.amountPerGather);
            amount -= amountGathered;

            if (amount <= 0) { Destroy(gameObject); }

            return amountGathered;
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
