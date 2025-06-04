using GameDevTV.RTS.Environment;
using GameDevTV.RTS.Utilities;
using Unity.Behavior;
using UnityEngine;

namespace GameDevTV.RTS.Units
{
    [RequireComponent(typeof(BehaviorGraphAgent))]
    public class Worker : AbstractUnit
    {
        #region PublicMethods
        public void Gather(GatherableSupply gatherableSupply)
        {
            behaviorAgent.SetVariableValue(BehaviorConstants.supplyRef, gatherableSupply);
            behaviorAgent.SetVariableValue(BehaviorConstants.supplyTypeRef, gatherableSupply.supplySO);
            behaviorAgent.SetVariableValue(BehaviorConstants.targetRef, gatherableSupply.gameObject);
            behaviorAgent.SetVariableValue(BehaviorConstants.commandRef, UnitCommands.Gather);
        }
        #endregion
    }
}
