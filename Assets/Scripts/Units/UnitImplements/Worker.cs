using GameDevTV.RTS.Behavior;
using GameDevTV.RTS.Environment;
using GameDevTV.RTS.Utilities;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using Unity.Behavior;
using UnityEngine;
using GameDevTV.RTS.Commands;

namespace GameDevTV.RTS.Units
{
    [RequireComponent(typeof(BehaviorGraphAgent))]
    public class Worker : AbstractUnit, IBuildingBuilder
    {
        [SerializeField] private ActionBase CancelBuildingCommand;

        #region ComputedProperties
        public bool HasSupplies 
        { 
            get 
            {
                behaviorAgent.GetVariable(BehaviorConstants.gatherAmountRef, out BlackboardVariable<int> heldVariable);
                return heldVariable.Value > 0;
            }
        }

        public bool IsBuilding
        {
            get
            {
                behaviorAgent.GetVariable(BehaviorConstants.commandRef, out BlackboardVariable<UnitCommands> command);
                return command.Value == UnitCommands.BuildBuilding;
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

            SetCommandOverrides(new ActionBase[] {CancelBuildingCommand});

            return buildingInstance;
        }

        public void CancelBuilding()
        {
            behaviorAgent.GetVariable(BehaviorConstants.ghostBuildingRef, out BlackboardVariable<GameObject> ghostBuilding);
            if (ghostBuilding.Value != null) { Destroy(ghostBuilding.Value); }

            behaviorAgent.GetVariable(BehaviorConstants.buildingUnderConstructionRef, out BlackboardVariable<BaseBuilding> baseBuilding);
            if (baseBuilding.Value != null) { Destroy(baseBuilding.Value.gameObject); }

            SetCommandOverrides(null);
            Stop();
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
