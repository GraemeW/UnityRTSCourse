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
        
        public void EnableFor(AbstractCommandable commandable)
        {
            ClearState();
            if (commandable == null || commandable.unitSO == null) { return; }
            
            gameObject.SetActive(true);
            if (commandable.unitSO.icon != null) { icon.sprite = commandable.unitSO.icon; }
            healthText.text = string.Format(_healthTextFormat, commandable.currentHealth, commandable.maxHealth);
        }

        public void Disable()
        {
            ClearState();
            gameObject.SetActive(false);
        }

        private void ClearState()
        {
            icon.sprite = null;
            healthText.text = string.Empty;
        }
    }
}
