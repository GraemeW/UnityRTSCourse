using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using GameDevTV.RTS.Commands;
using GameDevTV.RTS.Behavior;
using GameDevTV.RTS.Environment;
using GameDevTV.RTS.Utilities;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;

namespace GameDevTV.RTS.Units
{
    [RequireComponent(typeof(BehaviorGraphAgent))]
    public class Worker : AbstractUnit, IBuildingBuilder
    {
        // Tunables
        [SerializeField] private BaseCommand cancelBuildingCommand;
        [SerializeField][Range(0f,1f)] private float cancelBuildingRefundFraction = 0.75f;

        #region ComputedProperties
        public bool HasSupplies => BehaviorConstants.GetGatherAmount(behaviorAgent) > 0;
        public bool IsBuilding => BehaviorConstants.GetCommand(behaviorAgent) == UnitCommands.BuildBuilding;
        #endregion
        
        #region UnityMethods
        protected override void Start()
        {
            base.Start();
   
            if (behaviorAgent.BlackboardReference != null)
            {
                GatherSuppliesEventChannel gatherSuppliesEventChannel = BehaviorConstants.GetGatherSuppliesEventChannel(behaviorAgent);
                if (gatherSuppliesEventChannel != null) { gatherSuppliesEventChannel.Event += HandleGatherSuppliesEvent; }
                
                BuildingEventChannel buildingEventChannel = BehaviorConstants.GetBuildingEventChannel(behaviorAgent);
                if (buildingEventChannel != null) { buildingEventChannel.Event += HandleBuildingEvent; }
            }
        }

        protected override void OnDestroy()
        {
            if (behaviorAgent.BlackboardReference != null)
            {
                GatherSuppliesEventChannel gatherSuppliesEventChannel = BehaviorConstants.GetGatherSuppliesEventChannel(behaviorAgent);
                if (gatherSuppliesEventChannel != null) { gatherSuppliesEventChannel.Event -= HandleGatherSuppliesEvent; }
                
                BuildingEventChannel buildingEventChannel = BehaviorConstants.GetBuildingEventChannel(behaviorAgent);
                if (buildingEventChannel != null) { buildingEventChannel.Event += HandleBuildingEvent; }
            }
            base.OnDestroy();
        } 
        #endregion
        
        #region ProtectedMethods
        protected override void ReconcileContingentCommands()
        {
            if (IsBuilding)
            {
                AppendToCommands(new List<BaseCommand> { cancelBuildingCommand }, false);
            }
        }
        #endregion
        
        #region EventHandlers
        private void HandleGatherSuppliesEvent(GameObject worker, int amount, SupplySO supplyType)
        {
            Bus<SupplyEvent>.Raise(new SupplyEvent(supplyType, amount));
        }

        private void HandleBuildingEvent(GameObject self, BuildingEventType buildingEventType, BaseBuilding baseBuilding)
        {
            switch (buildingEventType)
            {
                case BuildingEventType.ArrivedAt:
                    CancelGhost();
                    break;
                case BuildingEventType.Begin:
                    AppendToCommands(new List<BaseCommand> { cancelBuildingCommand });
                    break;
                case BuildingEventType.Cancel:
                case BuildingEventType.Abort:
                    Abort();
                    break;
                case BuildingEventType.Completed:
                    break;
            }
        }
        #endregion

        #region PublicMethods
        public void ResetCommandList() => SetCommandOverrides(null);
        
        public void Gather(GatherableSupply gatherableSupply)
        {
            BehaviorConstants.SetSupply(behaviorAgent, gatherableSupply);
            BehaviorConstants.SetNearbySupplyCount(behaviorAgent, 1);
            BehaviorConstants.SetTarget(behaviorAgent, gatherableSupply.gameObject);
            BehaviorConstants.SetCommand(behaviorAgent, UnitCommands.Gather);
        }

        public void ReturnSupplies(CommandPost commandPost)
        {
            BehaviorConstants.SetCommandPost(behaviorAgent, commandPost.gameObject);
            BehaviorConstants.SetCommand(behaviorAgent, UnitCommands.ReturnSupplies);
        }

        public GameObject Build(BuildingSO buildingSO, Vector3 targetLocation)
        {
            if (buildingSO == null) { return null; }
            if (IsBuilding) { return null; }
            
            GameObject buildingInstance = Instantiate(buildingSO.prefab, targetLocation, Quaternion.identity);
            if (!buildingInstance.TryGetComponent(out BaseBuilding baseBuilding)) { return null; }
            baseBuilding.ShowGhostVisuals(true);
            baseBuilding.enabled = false;

            BehaviorConstants.SetGhostBuilding(behaviorAgent, buildingInstance);
            BehaviorConstants.SetBuildingSO(behaviorAgent, buildingSO);
            BehaviorConstants.SetCommand(behaviorAgent, UnitCommands.BuildBuilding);
            AppendToCommands(new List<BaseCommand> { cancelBuildingCommand });
            
            buildingSO.ChargeSupplies();

            return buildingInstance;
        }
        public void ResumeBuilding(BaseBuilding baseBuilding)
        {
            if (baseBuilding == null) { return; }
            
            BehaviorConstants.SetGhostBuilding(behaviorAgent, null);
            BehaviorConstants.SetTargetLocation(behaviorAgent, baseBuilding.transform.position);
            BehaviorConstants.SetBuildingSO(behaviorAgent, baseBuilding.GetBuildingSO());
            BehaviorConstants.SetBuildingUnderConstruction(behaviorAgent, baseBuilding);
            BehaviorConstants.SetCommand(behaviorAgent, UnitCommands.BuildBuilding);
        }
        
        public void Abort(bool callStop = true)
        {
            CancelGhost();
            if (TryGetComponent(out Animator animator)) { AnimationConstants.AnimateGathering(animator, false); }
            if (callStop) { Stop(); }
            SetCommandOverrides(null);
        }

        public void CancelBuilding()
        {
            BaseBuilding baseBuilding = BehaviorConstants.GetBuildingUnderConstruction(behaviorAgent);
            if (baseBuilding != null)
            {
                BuildingSO buildingSO = baseBuilding.GetBuildingSO();
                if (buildingSO != null) { buildingSO.RefundSupplies(cancelBuildingRefundFraction); }
                Destroy(baseBuilding.gameObject);
            }
            Abort();
        }
        #endregion
        
        #region PrivateMethods
        private void CancelGhost()
        {
            GameObject ghostBuilding = BehaviorConstants.GetGhostBuilding(behaviorAgent);
            if (ghostBuilding != null) { Destroy(ghostBuilding); }
        }
        #endregion
    }
}
