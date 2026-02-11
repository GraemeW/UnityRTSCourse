using System.Collections.Generic;
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
        public bool HasSupplies => BehaviorConstants.GetGatherAmount(behaviorAgent) > 0;

        public bool IsBuilding => BehaviorConstants.GetCommand(behaviorAgent) == UnitCommands.BuildBuilding;
        #endregion

        #region StaticMethods
        private static void HandleGatherSupplies(GameObject worker, int amount, SupplySO supplyType)
        {
            Bus<SupplyEvent>.Raise(new SupplyEvent(supplyType, amount));
        }
        #endregion
        
        #region UnityMethods
        protected override void Start()
        {
            base.Start();
   
            if (behaviorAgent.BlackboardReference != null)
            {
                GatherSuppliesEventChannel gatherSuppliesEventChannel = BehaviorConstants.GetGatherSuppliesEventChannel(behaviorAgent);
                if (gatherSuppliesEventChannel != null) { gatherSuppliesEventChannel.Event += HandleGatherSupplies; }
            }
        }

        protected override void OnDestroy()
        {
            if (behaviorAgent.BlackboardReference != null)
            {
                GatherSuppliesEventChannel gatherSuppliesEventChannel = BehaviorConstants.GetGatherSuppliesEventChannel(behaviorAgent);
                if (gatherSuppliesEventChannel != null) { gatherSuppliesEventChannel.Event -= HandleGatherSupplies; }
            }
            base.OnDestroy();
        } 
        #endregion

        #region PublicMethods
        public void ResetCommandList()
        {
            SetCommandOverrides(null);
        }
        
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
            GameObject buildingInstance = Instantiate(buildingSO.prefab, targetLocation, Quaternion.identity);
            if (!buildingInstance.TryGetComponent(out BaseBuilding baseBuilding)) { return null; }
            baseBuilding.ShowGhostVisuals(true);

            BehaviorConstants.SetGhostBuilding(behaviorAgent, buildingInstance);
            BehaviorConstants.SetBuildingSO(behaviorAgent, buildingSO);
            BehaviorConstants.SetCommand(behaviorAgent, UnitCommands.BuildBuilding);

            SetCommandOverrides(null);
            AppendToCommands(new List<ActionBase> { CancelBuildingCommand });

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

            SetCommandOverrides(null);
            AppendToCommands(new List<ActionBase> { CancelBuildingCommand });
        }

        public void CancelBuilding()
        {
            GameObject ghostBuilding = BehaviorConstants.GetGhostBuilding(behaviorAgent);
            if (ghostBuilding != null) { Destroy(ghostBuilding); }

            BaseBuilding baseBuilding = BehaviorConstants.GetBuildingUnderConstruction(behaviorAgent);
            if (baseBuilding != null) { Destroy(baseBuilding.gameObject); }

            SetCommandOverrides(null);
            Stop();
        }
        #endregion
    }
}
