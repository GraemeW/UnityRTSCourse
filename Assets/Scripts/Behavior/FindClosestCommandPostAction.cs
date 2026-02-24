using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using GameDevTV.RTS.Units;
using System.Collections.Generic;

namespace GameDevTV.RTS.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "FindClosestCommandPost", story: "[Agent] finds nearest [CommandPost]", category: "Action/Units", id: "88281cabee38df78f39a249759bfa399")]
    public partial class FindClosestCommandPostAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> CommandPost;
        [SerializeReference] public BlackboardVariable<float> SearchRadius = new(10.0f);

        protected override Status OnStart()
        {
            Collider[] colliders = Physics.OverlapSphere(
                Agent.Value.transform.position, 
                SearchRadius.Value, 
                LayerMask.GetMask(BaseBuilding.buildingsLayerMaskRef));

            var nearbyCommandPosts = new List<CommandPost>();
            foreach (Collider collider in colliders)
            {
                if (!collider.TryGetComponent(out CommandPost commandPost)) { continue; }
                if (commandPost.GetBuildingProgress().state != BuildingProgress.BuildingState.Completed) { continue; }
                nearbyCommandPosts.Add(commandPost);
            }
            
            // Simple exit criteria
            if (nearbyCommandPosts.Count == 0) { return Status.Failure; }
            else if (nearbyCommandPosts.Count == 1)
            {
                CommandPost.Value = nearbyCommandPosts[0].gameObject;
                return Status.Success;
            }

            // Find best post otherwise
            float minimumDistance = Mathf.Infinity;
            CommandPost closestCommandPost = null;
            foreach (CommandPost commandPost in nearbyCommandPosts)
            {
                float checkDistance = Vector3.Distance(commandPost.transform.position, Agent.Value.transform.position);
                if (checkDistance < minimumDistance)
                {
                    minimumDistance = checkDistance;
                    closestCommandPost = commandPost;
                }
            }
            CommandPost.Value = closestCommandPost.gameObject;
            return Status.Success;

        }
    }
}
