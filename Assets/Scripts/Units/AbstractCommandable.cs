using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Commands;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GameDevTV.RTS.Units
{
    public abstract class AbstractCommandable : MonoBehaviour, ISelectable
    {
        [field: SerializeField] public int currentHealth { get; private set; }
        [field: SerializeField] public int maxHealth { get; private set; }
        [field: SerializeField] public ActionBase[] availableCommands { get; private set; }

        [Header("Hookups")]
        [SerializeField] private DecalProjector decalProjector;
        [Header("Unit Properties")]
        [SerializeField] private UnitSO unitSO;

        #region UnityMethods
        protected virtual void Start()
        {
            currentHealth = unitSO.health;
            maxHealth = unitSO.health;
        }
        #endregion

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
