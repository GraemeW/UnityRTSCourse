using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.UI.Containers
{
    public class SingleUnitSelectedUI : MonoBehaviour, IUIElement<AbstractCommandable>
    {
        // Tunables
        [SerializeField] private TextMeshProUGUI nameText;
        
        public void EnableFor(AbstractCommandable commandable)
        {
            ClearState();
            if (commandable == null || commandable.unitSO == null) { return; }
            
            gameObject.SetActive(true);
            nameText.text = Regex.Replace(commandable.unitSO.name, "([A-Z])", " $1", RegexOptions.Compiled);
        }

        public void Disable()
        {
            ClearState();
            gameObject.SetActive(false);
        }
        
        private void ClearState()
        {
            nameText.text = string.Empty;
        }
    }
}
