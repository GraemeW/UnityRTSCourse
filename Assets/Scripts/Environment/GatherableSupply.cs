using GameDevTV.RTS.Units;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace GameDevTV.RTS.Environment
{
    public class GatherableSupply : MonoBehaviour, IGatherable, ISelectable
    {
        // Fixed
        public static string suppliesLayerMaskRef = "Supplies";


        #region Interface
        [field: SerializeField] public SupplySO supplySO {  get; private set; }
        [field: SerializeField] public int amount { get; private set; }
        [field: SerializeField] public bool isBusy { get; private set; }

        public bool BeginGather()
        {
            if (amount <= 0) { return false; }
            isBusy = true;
            return true;
        }

        public int EndGather()
        {
            isBusy = false;

            int amountGathered = Mathf.Min(amount, supplySO.amountPerGather);
            amount -= amountGathered;

            if (amount <= 0) { Destroy(gameObject); }

            return amountGathered;
        }

        public void ResetGather() => isBusy = false;

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
            if (supplySO == null) { return; }

            amount = supplySO.maxAmount;
            isBusy = false;
        }

        #endregion
    }
}
