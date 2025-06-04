using GameDevTV.RTS.Environment;
using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

namespace GameDevTV.RTS.Behavior
{
#if UNITY_EDITOR
    [CreateAssetMenu(menuName = "Behavior/Event Channels/GatherSuppliesEventChannel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "GatherSuppliesEventChannel", message: "[Agent] gathers [Amount] [Supplies]", category: "Events", id: "1336bfb9b56392703b1c4629dfc28ae5")]
    public sealed partial class GatherSuppliesEventChannel : EventChannel<GameObject, int, SupplySO> { }
}
