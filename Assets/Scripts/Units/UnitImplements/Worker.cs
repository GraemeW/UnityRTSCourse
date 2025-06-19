using GameDevTV.RTS.Behavior;
using GameDevTV.RTS.Environment;
using GameDevTV.RTS.Utilities;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using Unity.Behavior;
using UnityEngine;

namespace GameDevTV.RTS.Units
{
    [RequireComponent(typeof(BehaviorGraphAgent))]
    public class Worker : AbstractUnit, IBuildingBuilder
    {
        #region ComputedProperties
        public bool HasSupplies 
        { 
            get 
            {
                behaviorAgent.GetVariable(BehaviorConstants.gatherAmountRef, out BlackboardVariable<int> heldVariable);
                return heldVariable.Value > 0;
            }
        }
        #endregion

        #region UnityMethods
        protected override void Start()
        {
            base.Start();
   
            if (behaviorAgent.BlackboardReference != null && behaviorAgent.GetVariable(BehaviorConstants.gatherSuppliesEventRef, out BlackboardVariable<GatherSuppliesEventChannel> gatherSuppliesEventChannel))
            {
                gatherSuppliesEventChannel.Value.Event += HandleGatherSupplies;
            }
        }

        protected override void OnDestroy()
        {
            if (behaviorAgent.BlackboardReference != null && behaviorAgent.GetVariable(BehaviorConstants.gatherSuppliesEventRef, out BlackboardVariable<GatherSuppliesEventChannel> gatherSuppliesEventChannel))
            {
                gatherSuppliesEventChannel.Value.Event -= HandleGatherSupplies;
            }
            base.OnDestroy();
        } 

        protected void OnDisable()
        {

        }
        #endregion

        #region PublicMethods
        public void Gather(GatherableSupply gatherableSupply)
        {
            behaviorAgent.SetVariableValue(BehaviorConstants.supplyRef, gatherableSupply);
            behaviorAgent.SetVariableValue(BehaviorConstants.nearbySupplyCountRef, 1);
            behaviorAgent.SetVariableValue(BehaviorConstants.targetRef, gatherableSupply.gameObject);
            behaviorAgent.SetVariableValue(BehaviorConstants.commandRef, UnitCommands.Gather);
        }

        public void ReturnSupplies(CommandPost commandPost)
        {
            behaviorAgent.SetVariableValue(BehaviorConstants.commandPostRef, commandPost.gameObject);
            behaviorAgent.SetVariableValue(BehaviorConstants.commandRef, UnitCommands.ReturnSupplies);
        }

        public GameObject Build(BuildingSO buildingSO, Vector3 targetLocation)
        {
            GameObject buildingInstance = Instantiate(buildingSO.prefab, targetLocation, Quaternion.identity);
            if (!buildingInstance.TryGetComponent(out BaseBuilding baseBuilding)) { return null; }

            baseBuilding.ShowGhostVisuals(true);

            behaviorAgent.SetVariableValue(BehaviorConstants.ghostBuildingRef, buildingInstance);
            behaviorAgent.SetVariableValue(BehaviorConstants.buildingSORef, buildingSO);
            behaviorAgent.SetVariableValue(BehaviorConstants.commandRef, UnitCommands.BuildBuilding);

            return buildingInstance;
        }
        #endregion

        #region PrivateMethods
        private void HandleGatherSupplies(GameObject worker, int amount, SupplySO supplyType)
        {
            Bus<SupplyEvent>.Raise(new SupplyEvent(supplyType, amount));
        }
        #endregion
    }
}
