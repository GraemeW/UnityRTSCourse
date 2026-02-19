using UnityEngine;
using TMPro;

namespace GameDevTV.RTS.UI.Components
{
    [RequireComponent(typeof(RectTransform))]
    public class Tooltip : MonoBehaviour
    {
        // Tunables
        [Header("Hookups")]
        [SerializeField] private TextMeshProUGUI tooltipTMP;
        [SerializeField] RectTransform rectTransform; // Hook up in Editor because game object disabled by default
        [Header("Properties")]
        [SerializeField] private float delayToShowTooltip = 0.5f;

        [SerializeField] private float xOffset = 50f;

        // State
        private string tooltipText;
 
        #region UnityMethods
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }
        #endregion
        
        #region PublicMethods
        public void SetPosition(Vector2 position)
        {
            rectTransform.position = position;
        }
        
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
            
            if (tooltipTMP != null)
            {
                Vector2 preferredSize = tooltipTMP.GetPreferredValues();
                rectTransform.sizeDelta = new Vector2(preferredSize.x + xOffset, rectTransform.sizeDelta.y);
            }
        }

        private void HideTooltip()
        {
            tooltipTMP.text = string.Empty;
            gameObject.SetActive(false);
        }
        #endregion
    }
}
