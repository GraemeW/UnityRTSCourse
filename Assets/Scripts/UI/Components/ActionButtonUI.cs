using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using GameDevTV.RTS.Commands;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;

namespace GameDevTV.RTS.UI.Components
{
    [RequireComponent(typeof(Button))]
    public class ActionButtonUI : MonoBehaviour, IUIElement<BaseCommand, UnityAction>
    {
        // Hookups
        [SerializeField] private Image icon;
        
        // Cached References
        private Button button;

        #region UnityMethods
        private void Awake()
        {
            button = GetComponent<Button>();
        }
        #endregion

        #region PublicMethods
        public void EnableFor(BaseCommand action, UnityAction onClick)
        {
            ClearButtonState();
            if (action == null) { return; }
            
            icon.gameObject.SetActive(true);
            icon.sprite = action.icon;
            button.interactable = !action.IsLocked(new CommandContext());
            button.onClick.AddListener(onClick);
        }

        public void Disable()
        {
            ClearButtonState();
            icon.gameObject.SetActive(false);
        }
        #endregion
        
        #region PrivateMethods
        private void ClearButtonState()
        {
            icon.sprite = null;
            button.interactable = false;
            button.onClick.RemoveAllListeners();
        }
        #endregion
    }
}
