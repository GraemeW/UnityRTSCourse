using UnityEngine;
using UnityEngine.UI;

namespace GameDevTV.RTS.UI
{
    [RequireComponent(typeof(Button))]
    public class ActionButtonUI : MonoBehaviour
    {
        // Hookups
        [field: SerializeField] public Image icon { get; set; }

        // Cached References
        Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }
    }
}

