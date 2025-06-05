using GameDevTV.RTS.Environment;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using TMPro;
using UnityEngine;

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

        // Static Variables
        public static int Minerals { get; private set; } = 50;
        public static int Gas { get; private set; } = 50;
        public static int Population { get; private set; }
        public static int PopulationLimit { get; private set; } = 200;

        #region UnityMethods
        private void Start()
        {
            RefreshUI();
        }

        private void OnEnable()
        {
            Bus<SupplyEvent>.OnEvent += HandleSupplyEvent;
        }

        private void OnDisable()
        {
            Bus<SupplyEvent>.OnEvent -= HandleSupplyEvent;
        }
        #endregion

        #region EventHandlers
        private void HandleSupplyEvent(SupplyEvent supplyEvent)
        {
            if (supplyEvent.supplyType == null) { return; }

            if (supplyEvent.supplyType.Equals(mineralsSupply))
            {
                Minerals += supplyEvent.amount;
            }
            else if (supplyEvent.supplyType.Equals (gasSupply))
            {
                Gas += supplyEvent.amount;
            }

            RefreshUI();
        }
        #endregion

        #region UIMethods
        private void RefreshUI()
        {
            mineralsText.SetText(Minerals.ToString());
            gasText.SetText(Gas.ToString());
        }
        #endregion
    }
}
