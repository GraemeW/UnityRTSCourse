using UnityEngine;
using TMPro;

namespace GameDevTV.RTS.UI.Components
{
    public class Tooltip : MonoBehaviour
    {
        // Tunables
        [Header("Hookups")]
        [SerializeField] private TextMeshProUGUI tooltipTMP;
        [Header("Properties")]
        [SerializeField] private float delayToShowTooltip = 0.5f;

        // State
        private string tooltipText;
        
        #region PublicMethods
        public void SetText(string setTooltipText)
        {
            tooltipText = setTooltipText;
            if (gameObject.activeSelf) { DelayedShowTooltip(); }
        }

        public void DelayedShowTooltip()
        {
            Invoke(nameof(ShowTooltip), delayToShowTooltip);
        }

        public void DelayedHideTooltip()
        {
            CancelInvoke();
            Invoke(nameof(HideTooltip), delayToShowTooltip);
        }
        #endregion
        
        #region PrivateMethods
        private void ShowTooltip()
        {
            if (string.IsNullOrWhiteSpace(tooltipText)) { return; }
            tooltipTMP.text = tooltipText;
            gameObject.SetActive(true);
        }

        private void HideTooltip()
        {
            tooltipTMP.text = string.Empty;
            gameObject.SetActive(false);
        }
        #endregion
    }
}
