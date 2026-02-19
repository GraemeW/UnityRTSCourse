using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using GameDevTV.RTS.Commands;
using GameDevTV.RTS.Player;
using GameDevTV.RTS.Units;
using UnityEngine.EventSystems;

namespace GameDevTV.RTS.UI.Components
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(EventTrigger))]
    public class ActionButtonUI : MonoBehaviour, IUIElement<BaseCommand, UnityAction>, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Hookups")]
        [SerializeField] private Image icon;
        [SerializeField] private Tooltip tooltip;
        
        // Cached References
        private Button button;
        private RectTransform rectTransform;

        #region UnityMethods
        private void Awake()
        {
            button = GetComponent<Button>();
            rectTransform = GetComponent<RectTransform>();
        }
        #endregion

        #region InterfaceMethods
        public void EnableFor(BaseCommand command, UnityAction onClick)
        {
            ClearButtonState();
            if (command == null) { return; }
            
            icon.gameObject.SetActive(true);
            icon.sprite = command.icon;
            button.interactable = !command.IsLocked(new CommandContext());
            button.onClick.AddListener(onClick);
            if (tooltip != null) { tooltip.SetText(GetTooltipText(command)); }
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
            var tooltipPosition = new Vector2(
                rectTransform.position.x + rectTransform.rect.width / 2f,
                rectTransform.position.y + rectTransform.rect.height / 2f);
            tooltip.SetPosition(tooltipPosition);
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

        private string GetTooltipText(BaseCommand command)
        {
            string tooltipText = $"{command.tooltipText}\n";
            
            SupplyCostSO supplyCostSO = command switch
            {
                BuildUnitCommand buildUnitCommand => buildUnitCommand.unitSO.cost,
                BuildBuildingCommand buildBuildingCommand => buildBuildingCommand.buildingSO.cost,
                _ => null
            };

            if (supplyCostSO != null)
            {
                if (supplyCostSO.minerals > 0) { tooltipText += $"{supplyCostSO.minerals} Minerals. "; }
                if (supplyCostSO.gas > 0) { tooltipText += $"{supplyCostSO.gas} Gas."; }
            }

            return tooltipText;
        }
        #endregion
    }
}
