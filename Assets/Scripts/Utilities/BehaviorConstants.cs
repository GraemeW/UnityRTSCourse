using GameDevTV.RTS.Units;
using UnityEngine;
using Unity.Behavior;
using GameDevTV.RTS.Environment;
using GameDevTV.RTS.Behavior;

namespace GameDevTV.RTS.Utilities
{
    public static class BehaviorConstants
    {
        // Static Behavior References
        // Note:  These MUST match the variables in the behavior tree blackboard
        private static string commandRef = "Command";
        private static string targetLocationRef = "TargetLocation";
        private static string targetRef = "Target";
        private static string supplyRef = "Supply";
        private static string nearbySupplyCountRef = "NearbySupplyCount";
        private static string gatherAmountRef = "GatherAmount";
        private static string commandPostRef = "CommandPost";
        private static string gatherSuppliesEventRef = "GatherSuppliesEvent";
        private static string ghostBuildingRef = "Ghost";
        private static string buildingSORef = "BuildingSO";
        private static string buildingUnderConstructionRef = "BuildingUnderConstruction";

        #region Setters
        public static void SetCommand(BehaviorGraphAgent behaviorAgent, UnitCommands command)
        {
            if  (behaviorAgent == null) { return; }
            behaviorAgent.SetVariableValue(commandRef, command);
        }

        public static void SetTargetLocation(BehaviorGraphAgent behaviorAgent, Vector3 position)
        {
            if (behaviorAgent == null) { return; }
            behaviorAgent.SetVariableValue(targetLocationRef, position);
        }

        public static void SetTarget(BehaviorGraphAgent behaviorAgent, GameObject target)
        {
            if (behaviorAgent == null) { return; }
            behaviorAgent.SetVariableValue(targetRef, target);
        }

        public static void SetSupply(BehaviorGraphAgent behaviorAgent, GatherableSupply gatherableSupply)
        {
            if (behaviorAgent == null) { return; }
            behaviorAgent.SetVariableValue(supplyRef, gatherableSupply);
        }

        public static void SetNearbySupplyCount(BehaviorGraphAgent behaviorAgent, int nearbySupplyCount)
        {
            if (behaviorAgent == null) { return; }
            behaviorAgent.SetVariableValue(nearbySupplyCountRef, nearbySupplyCount);
        }

        public static void SetCommandPost(BehaviorGraphAgent behaviorAgent, GameObject commandPost)
        {
            if (behaviorAgent == null) { return; }
            behaviorAgent.SetVariableValue(commandPostRef, commandPost);
        }

        public static void SetGhostBuilding(BehaviorGraphAgent behaviorAgent, GameObject ghostBuilding)
        {
            if (behaviorAgent == null) { return; }
            behaviorAgent.SetVariableValue(ghostBuildingRef, ghostBuilding);
        }

        public static void SetBuildingSO(BehaviorGraphAgent behaviorAgent, BuildingSO buildingSO)
        {
            if (behaviorAgent == null) { return; }
            behaviorAgent.SetVariableValue(buildingSORef, buildingSO);
        }
        #endregion

        #region Getters
        public static UnitCommands GetCommand(BehaviorGraphAgent behaviorAgent)
        {
            if (behaviorAgent == null) { return UnitCommands.Stop; }
            if (!behaviorAgent.GetVariable(commandRef, out BlackboardVariable<UnitCommands> command)) { return UnitCommands.Stop; }
            return command.Value;
        }

        public static int GetGatherAmount(BehaviorGraphAgent behaviorAgent)
        {
            if (behaviorAgent == null) { return 0; }
            if (!behaviorAgent.GetVariable(gatherAmountRef, out BlackboardVariable<int> gatherAmount)) { return 0; }
            return gatherAmount.Value;
        }

        public static GatherSuppliesEventChannel GetGatherSuppliesEventChannel(BehaviorGraphAgent behaviorAgent)
        {
            if (behaviorAgent == null) { return null; }
            if (!behaviorAgent.GetVariable(gatherSuppliesEventRef, out BlackboardVariable<GatherSuppliesEventChannel> gatherSuppliesEventChannel)) { return null; }
            return gatherSuppliesEventChannel.Value;
        }

        public static GameObject GetGhostBuilding(BehaviorGraphAgent behaviorAgent)
        {
            if (behaviorAgent == null) { return null; }
            if (!behaviorAgent.GetVariable(ghostBuildingRef, out BlackboardVariable<GameObject> ghostBuilding)) { return null; }
            return ghostBuilding.Value;
        }

        public static BaseBuilding GetBaseBuilding(BehaviorGraphAgent behaviorAgent)
        {
            if (behaviorAgent == null) { return null; }
            if (!behaviorAgent.GetVariable(buildingUnderConstructionRef, out BlackboardVariable<BaseBuilding> baseBuilding)) { return null; }
            return baseBuilding.Value;
        }
        #endregion
    }
}
