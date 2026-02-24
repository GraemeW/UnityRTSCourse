using GameDevTV.RTS.Units;
using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

// Note:
// Moving namespaces currently breaks serialization -- don't do it for new blackboard scripts

#if UNITY_EDITOR
    [CreateAssetMenu(menuName = "Behavior/Event Channels/BuildingEventChannel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "BuildingEventChannel", message: "[Self] [BuildingEventType] on [BaseBuilding]", category: "Events", id: "da96909a7da1baa126657404fc236a46")]
    public sealed partial class BuildingEventChannel : EventChannel<GameObject, BuildingEventType, BaseBuilding>
    {
    }
