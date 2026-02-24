using UnityEngine;
using TMPro;
using GameDevTV.RTS.Environment;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.Player
{
    public class Supplies : MonoBehaviour
    {
        [Header("Hookups")]
        [SerializeField] private TextMeshProUGUI mineralsText;
        [SerializeField] private TextMeshProUGUI gasText;
        [SerializeField] private TextMeshProUGUI populationText;

        [Header("SupplyTypes")]
        [SerializeField] private SupplySO mineralsSupply;
        [SerializeField] private SupplySO gasSupply;

        #region Static
        public static int minerals { get; private set; } = 1000;
        public static int gas { get; private set; } = 1000;
        public static int population { get; private set; }
        public static int populationLimit { get; private set; } = 200;
        
        public static bool HasEnoughSuppliesToBuild(AbstractUnitSO unitSO)
        {
            if (unitSO == null) { return false; }
            if (unitSO.cost == null) { return true; }
            return minerals >= unitSO.cost.minerals && gas >= unitSO.cost.gas;
        }
        #endregion
        
        #region UnityMethods
        private void Start()
        {
            RefreshUI();
        }

        private void OnEnable()
        {
            Bus<SupplyEvent>.SubscribeToEvent(HandleSupplyEvent);
        }

        private void OnDisable()
        {
            Bus<SupplyEvent>.UnsubscribeFromEvent(HandleSupplyEvent);
        }
        #endregion

        #region EventHandlers
        private void HandleSupplyEvent(SupplyEvent supplyEvent)
        {
            if (supplyEvent.supplyType == null) { return; }

            if (supplyEvent.supplyType.Equals(mineralsSupply))
            {
                minerals += supplyEvent.amount;
            }
            else if (supplyEvent.supplyType.Equals (gasSupply))
            {
                gas += supplyEvent.amount;
            }

            RefreshUI();
        }
        #endregion

        #region UIMethods
        private void RefreshUI()
        {
            mineralsText.SetText(minerals.ToString());
            gasText.SetText(gas.ToString());
        }
        #endregion
    }
}
