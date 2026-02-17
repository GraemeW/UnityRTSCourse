using GameDevTV.RTS.Units;
using UnityEngine;
using Unity.Behavior;
using GameDevTV.RTS.Environment;
using GameDevTV.RTS.Behavior;

namespace GameDevTV.RTS.Utilities
{
    public static class BehaviorConstants
    {
        // Static Behaviour References
        // Note:  These MUST match the variables in the behaviour tree blackboard
        private const string _commandRef = "Command";
        private const string _targetLocationRef = "TargetLocation";
        private const string _targetRef = "Target";
        private const string _supplyRef = "Supply";
        private const string _nearbySupplyCountRef = "NearbySupplyCount";
        private const string _gatherAmountRef = "GatherAmount";
        private const string _commandPostRef = "CommandPost";
        private const string _gatherSuppliesEventRef = "GatherSuppliesEvent";
        private const string _ghostBuildingRef = "Ghost";
        private const string _buildingSORef = "BuildingSO";
        private const string _buildingUnderConstructionRef = "BuildingUnderConstruction";

        #region Setters
        public static void SetCommand(BehaviorGraphAgent behaviorAgent, UnitCommands command)
        {
            if  (behaviorAgent == null) { return; }
            behaviorAgent.SetVariableValue<UnitCommands>(_commandRef, command);
        }

        public static void SetTargetLocation(BehaviorGraphAgent behaviorAgent, Vector3 position)
        {
            if (behaviorAgent == null) { return; }
            behaviorAgent.SetVariableValue<Vector3>(_targetLocationRef, position);
        }

        public static void SetTarget(BehaviorGraphAgent behaviorAgent, GameObject target)
        {
            if (behaviorAgent == null) { return; }
            behaviorAgent.SetVariableValue<GameObject>(_targetRef, target);
        }

        public static void SetSupply(BehaviorGraphAgent behaviorAgent, GatherableSupply gatherableSupply)
        {
            if (behaviorAgent == null) { return; }
            behaviorAgent.SetVariableValue<GatherableSupply>(_supplyRef, gatherableSupply);
        }

        public static void SetNearbySupplyCount(BehaviorGraphAgent behaviorAgent, int nearbySupplyCount)
        {
            if (behaviorAgent == null) { return; }
            behaviorAgent.SetVariableValue<int>(_nearbySupplyCountRef, nearbySupplyCount);
        }

        public static void SetCommandPost(BehaviorGraphAgent behaviorAgent, GameObject commandPost)
        {
            if (behaviorAgent == null) { return; }
            behaviorAgent.SetVariableValue<GameObject>(_commandPostRef, commandPost);
        }

        public static void SetGhostBuilding(BehaviorGraphAgent behaviorAgent, GameObject ghostBuilding)
        {
            if (behaviorAgent == null) { return; }
            behaviorAgent.SetVariableValue<GameObject>(_ghostBuildingRef, ghostBuilding);
        }

        public static void SetBuildingSO(BehaviorGraphAgent behaviorAgent, BuildingSO buildingSO)
        {
            if (behaviorAgent == null) { return; }
            behaviorAgent.SetVariableValue<BuildingSO>(_buildingSORef, buildingSO);
        }

        public static void SetBuildingUnderConstruction(BehaviorGraphAgent behaviorAgent, BaseBuilding baseBuilding)
        {
            if (behaviorAgent == null) { return; }
            behaviorAgent.SetVariableValue<BaseBuilding>(_buildingUnderConstructionRef, baseBuilding);
        }
        #endregion

        #region Getters
        public static UnitCommands GetCommand(BehaviorGraphAgent behaviorAgent)
        {
            if (behaviorAgent == null) { return UnitCommands.Stop; }
            return !behaviorAgent.GetVariable(_commandRef, out BlackboardVariable<UnitCommands> command) ? UnitCommands.Stop : command.Value;
        }

        public static int GetGatherAmount(BehaviorGraphAgent behaviorAgent)
        {
            if (behaviorAgent == null) { return 0; }
            return !behaviorAgent.GetVariable(_gatherAmountRef, out BlackboardVariable<int> gatherAmount) ? 0 : gatherAmount.Value;
        }

        public static GatherSuppliesEventChannel GetGatherSuppliesEventChannel(BehaviorGraphAgent behaviorAgent)
        {
            if (behaviorAgent == null) { return null; }
            return !behaviorAgent.GetVariable(_gatherSuppliesEventRef, out BlackboardVariable<GatherSuppliesEventChannel> gatherSuppliesEventChannel) ? null : gatherSuppliesEventChannel.Value;
        }

        public static GameObject GetGhostBuilding(BehaviorGraphAgent behaviorAgent)
        {
            if (behaviorAgent == null) { return null; }
            return !behaviorAgent.GetVariable(_ghostBuildingRef, out BlackboardVariable<GameObject> ghostBuilding) ? null : ghostBuilding.Value;
        }

        public static BaseBuilding GetBuildingUnderConstruction(BehaviorGraphAgent behaviorAgent)
        {
            if (behaviorAgent == null) { return null; }
            return !behaviorAgent.GetVariable(_buildingUnderConstructionRef, out BlackboardVariable<BaseBuilding> baseBuilding) ? null : baseBuilding.Value;
        }
        #endregion
    }
}
