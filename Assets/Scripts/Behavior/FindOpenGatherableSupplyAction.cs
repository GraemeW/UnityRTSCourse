using GameDevTV.RTS.Environment;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using GameDevTV.RTS.Units;
using System.Collections.Generic;
using GameDevTV.RTS.Utilities;
using System.Linq;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindOpenGatherableSupply", story: "[Agent] finds unoccupied [Supply] of [SupplyType] near [Target]", category: "Action/Units", id: "21eca44fe1bb9e3b80588c5a202e0331")]
public partial class FindOpenGatherableSupplyAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GatherableSupply> Supply;
    [SerializeReference] public BlackboardVariable<SupplySO> SupplyType;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> SearchRadius = new(10.0f);

    // State
    List<GatherableSupply> nearbySupplies = new List<GatherableSupply>();

    protected override Status OnStart()
    {
        if (Agent.Value == null) { return Status.Failure; }
        if (!ReckonSupplyType()) { return Status.Failure; }

        // Simple check if clicked supply available
        if (Supply.Value != null && !Supply.Value.isBusy)
        {
            Target.Value = Supply.Value.gameObject;
            return Status.Success; 
        }

        // Otherwise populate supplies for checking
        if (FindNearbySupplies() == 0) { return Status.Failure; }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Agent.Value == null) { return Status.Failure; }

        if (CheckNearbySupplies()) { return Status.Success; }
        return Status.Running;
    }

    private bool CheckNearbySupplies()
    {
        GatherableSupply[] sortedSupplies = nearbySupplies.Where(sortedSupply => sortedSupply != null).ToArray();
        if (sortedSupplies.Length == 0) { return false; }

        Array.Sort(sortedSupplies, new ClosestSupplyComparator(Agent.Value.transform.position));
        foreach (GatherableSupply gatherableSupply in sortedSupplies)
        {
            if (!gatherableSupply.isBusy)
            {
                Supply.Value = gatherableSupply;
                Target.Value = gatherableSupply.gameObject;
                return true;
            }
        }
        return false;
    }

    private int FindNearbySupplies()
    {
        nearbySupplies.Clear();
        Vector3 searchPosition = Agent.Value.transform.position;
        if (Supply.Value != null) { searchPosition = Supply.Value.transform.position; }

        Collider[] colliders = Physics.OverlapSphere(
            searchPosition,
            SearchRadius.Value,
            LayerMask.GetMask(GatherableSupply.suppliesLayerMaskRef));

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out GatherableSupply gatherableSupply)
                    && gatherableSupply.supplySO.Equals(SupplyType.Value))
            {
                nearbySupplies.Add(gatherableSupply);
            }
        }
        return nearbySupplies.Count;
    }

    private bool ReckonSupplyType()
    {
        if (SupplyType.Value != null) { return true; } // Already reckoned
        if (Supply.Value == null) { return false; }

        SupplyType.Value = Supply.Value.supplySO;
        return true;
    }
}
