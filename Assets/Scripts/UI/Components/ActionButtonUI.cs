using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using GameDevTV.RTS.Commands;
using UnityEngine.EventSystems;

namespace GameDevTV.RTS.UI.Components
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(EventTrigger))]
    public class ActionButtonUI : MonoBehaviour, IUIElement<BaseCommand, UnityAction>, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Hookups")]
        [SerializeField] private Image icon;
        [SerializeField] private Tooltip tooltip;
        
        // State
        private string actionTooltipText;
        
        // Cached References
        private Button button;

        #region UnityMethods
        private void Awake()
        {
            button = GetComponent<Button>();
        }
        #endregion

        #region InterfaceMethods
        public void EnableFor(BaseCommand action, UnityAction onClick)
        {
            ClearButtonState();
            if (action == null) { return; }
            
            icon.gameObject.SetActive(true);
            icon.sprite = action.icon;
            button.interactable = !action.IsLocked(new CommandContext());
            button.onClick.AddListener(onClick);
            if (tooltip != null) { tooltip.SetText(action.tooltipText); }
        }

        public void Disable()
        {
            ClearButtonState();
            icon.gameObject.SetActive(false);
        }
        
        public void OnPointerEnter(PointerEventData _)
        {
            if (tooltip == null) { return;  }
            tooltip.DelayedShowTooltip();
        }

        public void OnPointerExit(PointerEventData _)
        {
            if (tooltip == null) { return; }
            tooltip.DelayedHideTooltip();
        }
        #endregion
        
        #region PrivateMethods
        private void ClearButtonState()
        {
            icon.sprite = null;
            button.interactable = false;
            button.onClick.RemoveAllListeners();
            if (tooltip != null) { tooltip.SetText(string.Empty); }
        }
        #endregion
    }
}
