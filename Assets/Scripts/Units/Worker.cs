using GameDevTV.RTS.Environment;
using Unity.Behavior;
using UnityEngine;

namespace GameDevTV.RTS.Units
{
    [RequireComponent(typeof(BehaviorGraphAgent))]
    public class Worker : AbstractUnit
    {
        // Static Behavior References
        // Note:  These MUST match the variables in the behavior tree blackboard
        public static string supplyRef { get; private set; } = "Supply";
        public static string supplyTypeRef { get; private set; } = "SupplyType";

        #region PublicMethods
        public void Gather(GatherableSupply gatherableSupply)
        {
            behaviorAgent.SetVariableValue(supplyRef, gatherableSupply);
            behaviorAgent.SetVariableValue(supplyTypeRef, gatherableSupply.supplySO);
            behaviorAgent.SetVariableValue(targetRef, gatherableSupply.gameObject);
            behaviorAgent.SetVariableValue(AbstractUnit.commandRef, UnitCommands.Gather);
        }
        #endregion
    }
}
