namespace GameDevTV.RTS.Units
{
    public class BaseMilitaryUnit : AbstractUnit
    {
        // Cached References
        protected AttackConfigSO attackConfigSO;
        
        #region ProtectedMethods

        protected override void Awake()
        {
            base.Awake();
            if (unitSOImpl != null) { attackConfigSO = unitSOImpl.attackConfig; }
        }
        
        protected override void ReconcileContingentCommands()
        {
            // No special commands
        }
        #endregion
    }
}
