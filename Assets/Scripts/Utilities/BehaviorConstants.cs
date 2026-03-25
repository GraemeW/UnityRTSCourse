using System.Collections.Generic;
using System.Linq;
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
        private const string _buildingEventChannelRef = "BuildingEventChannel";
        private const string _attackConfigRef = "AttackConfig";
        private const string _nearbyEnemiesRef = "NearbyEnemies";

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

        public static void SetAttackConfig(BehaviorGraphAgent behaviorAgent, AttackConfigSO attackConfig)
        {
            if (behaviorAgent == null) { return; }
            behaviorAgent.SetVariableValue<AttackConfigSO>(_attackConfigRef, attackConfig);
        }

        public static void AddToNearbyEnemies(BehaviorGraphAgent behaviorAgent, IDamageable damageable)
        {
            if (damageable == null) { return; }
            GameObject enemy = damageable.unitGameObject;
            if (enemy == null) { return; }
            
            List<GameObject> nearbyEnemies = GetNearbyEnemies(behaviorAgent);
            nearbyEnemies.Add(enemy);
            nearbyEnemies.Sort(new ClosestGameObjectComparator(behaviorAgent.transform.position));
        }

        public static void RemoveFromNearbyEnemies(BehaviorGraphAgent behaviorAgent, IDamageable damageable)
        {
            if (damageable == null) { return; }
            GameObject enemy = damageable.unitGameObject;
            if (enemy == null) { return; }
            
            List<GameObject> nearbyEnemies = GetNearbyEnemies(behaviorAgent);
            nearbyEnemies.Remove(enemy);
            nearbyEnemies.Sort(new ClosestGameObjectComparator(behaviorAgent.transform.position));
        }
        #endregion

        #region Getters
        public static UnitCommands GetCommand(BehaviorGraphAgent behaviorAgent)
        {
            if (behaviorAgent == null) { return UnitCommands.Stop; }
            return !behaviorAgent.GetVariable(_commandRef, out BlackboardVariable<UnitCommands> command) ? UnitCommands.Stop : command.Value;
        }

        public static Vector3? GetTargetLocation(BehaviorGraphAgent behaviorAgent)
        {
            if (behaviorAgent == null) { return Vector3.zero; }
            return !behaviorAgent.GetVariable(_targetLocationRef, out BlackboardVariable<Vector3> targetLocation) ? null : targetLocation.Value;
        }

        public static GameObject GetTarget(BehaviorGraphAgent behaviorAgent)
        {
            if (behaviorAgent == null) { return null; }
            return !behaviorAgent.GetVariable(_targetRef, out BlackboardVariable<GameObject> target) ? null : target.Value;
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

        public static BuildingEventChannel GetBuildingEventChannel(BehaviorGraphAgent behaviorAgent)
        {
            if (behaviorAgent == null) { return null; }
            return !behaviorAgent.GetVariable(_buildingEventChannelRef, out BlackboardVariable<BuildingEventChannel> buildingEventChannel) ? null : buildingEventChannel.Value;
        }

        public static List<GameObject> GetNearbyEnemies(BehaviorGraphAgent behaviorAgent)
        {
            if (behaviorAgent == null) { return null; }
            return !behaviorAgent.GetVariable(_nearbyEnemiesRef, out BlackboardVariable<List<GameObject>> nearbyEnemies) ? null : nearbyEnemies.Value;
        }

        public static GameObject GetNearestEnemy(BehaviorGraphAgent behaviorAgent)
        {
            if (behaviorAgent == null) { return null; }
            return !behaviorAgent.GetVariable(_nearbyEnemiesRef, out BlackboardVariable<List<GameObject>> nearbyEnemies) ? null : nearbyEnemies.Value.FirstOrDefault();
        }
        #endregion
    }
}
