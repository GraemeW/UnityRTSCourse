using UnityEngine;

namespace GameDevTV.RTS.UI.Components
{
    public class ProgressBar : MonoBehaviour
    {
        // Tunables
        [SerializeField] private RectTransform mask;
        [SerializeField] private Vector2 padding = new(9, 8);

        // Cached References
        private RectTransform maskParentRectTransform;

        // State
        private bool isHookedUp;

        #region UnityMethods
        private void Awake()
        {
            if (mask == null) { return; }
            isHookedUp = mask.parent.TryGetComponent(out maskParentRectTransform);
        }
        #endregion

        public void SetProgress(float progress)
        {
            if (!isHookedUp) { return; }

            float parentWidth = maskParentRectTransform.sizeDelta.x - padding.x;
            float targetWidth = parentWidth;

            targetWidth *= Mathf.Clamp01(progress);

            mask.offsetMin = padding;
            mask.offsetMax = new Vector2(targetWidth - parentWidth, -padding.y);
        }
    }
}
