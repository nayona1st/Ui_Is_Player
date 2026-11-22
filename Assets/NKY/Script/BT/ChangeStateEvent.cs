using NKY.Script.BT;
using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/ChangeStateEvent")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "ChangeStateEvent", message: "Change [State]", category: "Events", id: "eba26bdfde85ed60fa8ac1189dbc51d7")]
public sealed partial class ChangeStateEvent : EventChannel<PoliceState> { }

