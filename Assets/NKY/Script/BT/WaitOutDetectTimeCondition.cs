using System;
using GGW.game._00.Codes.CCTV;
using NKY.Script.Agent;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "WaitOutDetectTime", story: "Wait [Self] OutDetect for [OutDetectTime] over", category: "Conditions", id: "951cdda405c86642f19c07c33c92b03b")]
public partial class WaitOutDetectTimeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> OutDetectTime;


    private CctvRange _cctvRange;

    public override bool IsTrue()
    {
        if (_cctvRange == null) return false;
        return Time.time - _cctvRange.LastDetectTime >= OutDetectTime.Value;
    }

    public override void OnStart()
    {
        _cctvRange = Self.Value.GetComponentInChildren<CctvRange>();
        if (_cctvRange == null)
            Debug.LogError("CctvRange not found", Self.Value);
        
    }
}
