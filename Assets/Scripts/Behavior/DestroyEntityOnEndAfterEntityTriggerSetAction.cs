using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Object = UnityEngine.Object;

namespace GameDevTV.RTS.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "DestroyEntityOnEndAfterEntityTriggerSet", story: "Destroy [Entity] on End after [DestroyToggle] set", category: "Action", id: "89bb289464b03f9fba31d60ccd47a328")]
    public partial class DestroyEntityOnEndAction : Action
    {
    [SerializeReference] public BlackboardVariable<GameObject> Entity;
    [SerializeReference] public BlackboardVariable<bool> DestroyToggle;
        protected override Status OnStart()
        {
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            return DestroyToggle.Value ? Status.Success : Status.Running;
        }

        protected override void OnEnd()
        {
            if (Entity.Value != null) { Object.Destroy(Entity.Value); }
        }
    }
}
