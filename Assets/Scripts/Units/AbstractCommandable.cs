using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GameDevTV.RTS.Units
{
    public abstract class AbstractCommandable : MonoBehaviour, ISelectable
    {
        [Header("Hookups")]
        [SerializeField] private DecalProjector decalProjector;
        [Header("Game Properties")]
        [field: SerializeField] public int health { get; private set; }

        #region Selection
        public void Deselect()
        {
            if (decalProjector != null)
            {
                decalProjector.gameObject.SetActive(false);
            }

            Bus<UnitDeselectedEvent>.Raise(new UnitDeselectedEvent(this));
        }

        public void Select()
        {
            if (decalProjector != null)
            {
                decalProjector.gameObject.SetActive(true);
            }

            Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(this));
        }
        #endregion
    }
}
