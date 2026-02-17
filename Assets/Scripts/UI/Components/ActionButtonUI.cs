using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using GameDevTV.RTS.Commands;

namespace GameDevTV.RTS.UI.Components
{
    [RequireComponent(typeof(Button))]
    public class ActionButtonUI : MonoBehaviour, IUIElement<ActionBase, UnityAction>
    {
        // Hookups
        [SerializeField] private Image icon;

        // Cached References
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void EnableFor(ActionBase action, UnityAction onClick)
        {
            button.onClick.RemoveAllListeners();

            icon.gameObject.SetActive(true);
            icon.sprite = action.icon;
            button.interactable = true;
            button.onClick.AddListener(onClick);
        }

        public void Disable()
        {
            icon.sprite = null;
            icon.gameObject.SetActive(false);
            button.interactable = false;
            button.onClick.RemoveAllListeners();
        }
    }
}
