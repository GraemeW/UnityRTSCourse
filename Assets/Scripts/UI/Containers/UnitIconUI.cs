using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.UI.Containers
{
    public class UnitIconUI : MonoBehaviour, IUIElement<AbstractCommandable>
    {
        // Tunables
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI healthText;
        
        // Constants
        private const string _healthTextFormat = "{0} / {1}";
        
        // State
        private AbstractCommandable commandable;
        
        public void EnableFor(AbstractCommandable setCommandable)
        {
            ClearState();
            if (setCommandable == null || setCommandable.unitSO == null) { return; }
            
            commandable = setCommandable;
            gameObject.SetActive(true);
            commandable.onHealthUpdated += HandleHealthUpdateEvent;
            
            RefreshUI();
            UpdateHealth();
        }

        public void Disable()
        {
            ClearState();
            gameObject.SetActive(false);
        }

        private void ClearState()
        {
            if (commandable != null) { commandable.onHealthUpdated -= HandleHealthUpdateEvent; }
            commandable = null;
            icon.sprite = null;
            healthText.text = string.Empty;
        }

        private void RefreshUI()
        {
            if (commandable == null || commandable.unitSO == null) { return; }
            UpdateIcon();
            UpdateHealth();
        }

        private void UpdateIcon()
        {
            if (commandable.unitSO.icon == null) { return; }
            icon.sprite = commandable.unitSO.icon;
        }

        private void UpdateHealth()
        {
            healthText.text = string.Format(_healthTextFormat, commandable.GetCurrentHealth(), commandable.maxHealth);
        }

        private void HandleHealthUpdateEvent(AbstractCommandable passCommandable, int lastHealth, int newHealth)
        {
            if (commandable != passCommandable) { return; }
            RefreshUI();
        }
    }
}
