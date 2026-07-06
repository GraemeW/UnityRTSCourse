using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/LoadUnitEventChannel")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "LoadUnitEventChannel", message: "[Self] loads [Target] into itself.", category: "Events", id: "dbf86965b459efbba2be0480923f0987")]
public sealed partial class LoadUnitEventChannel : EventChannel<GameObject, GameObject> { }

