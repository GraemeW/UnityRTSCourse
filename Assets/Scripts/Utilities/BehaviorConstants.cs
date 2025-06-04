using Unity.Behavior;

namespace GameDevTV.RTS.Utilities
{
    public static class BehaviorConstants
    {
        // Static Behavior References
        // Note:  These MUST match the variables in the behavior tree blackboard
        public static string commandRef { get; private set; } = "Command";
        public static string targetLocationRef { get; private set; } = "TargetLocation";
        public static string targetRef { get; private set; } = "Target";

        public static string supplyRef { get; private set; } = "Supply";
        public static string supplyTypeRef { get; private set; } = "SupplyType";

        public static string gatherSuppliesEventRef { get; private set; } = "GatherSuppliesEventChannel";
    }
}
