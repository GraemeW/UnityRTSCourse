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
        [field: SerializeField] public AbstractUnitSO unitSO { get; private set; }

        [Header("Hookups")]
        [SerializeField] private DecalProjector decalProjector;
        [SerializeField] private ActionBase[] availableCommands;

        // State
        public ActionBase[] currentCommands { get; private set; }

        #region UnityMethods
        protected virtual void Start()
        {
            currentHealth = unitSO.health;
            maxHealth = unitSO.health;
            currentCommands = availableCommands;
        }
        #endregion

        #region Selection
        public void Deselect()
        {
            SetCommandOverrides(null);

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

        #region Commands
        public void SetCommandOverrides(ActionBase[] commandOverrides)
        {
            if (commandOverrides == null || commandOverrides.Length == 0) { currentCommands = availableCommands; return; }

            currentCommands = commandOverrides;
            Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(this));
        }
        #endregion
    }
}
