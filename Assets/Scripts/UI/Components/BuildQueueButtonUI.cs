using GameDevTV.RTS.Units;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameDevTV.RTS.UI.Components
{
    [RequireComponent(typeof(Button))]
    public class BuildQueueButtonUI : MonoBehaviour, IUIElement<AbstractUnitSO, UnityAction>
    {
        // Hookups
        [SerializeField] private Image icon;

        // Cached References
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void EnableFor(AbstractUnitSO unit, UnityAction onClick)
        {
            icon.gameObject.SetActive(true);
            icon.sprite = unit.icon;
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
