using System;
using _LumenLib.ModuleSystem;
using GGW.game._00.Codes.CCTV;
using NKY.Script.Agent;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "FIndTargetFoward", story: "Find [Self] in [Range] and [pov] out [TargetGo]", category: "Conditions", id: "7b4957a00eb45e8975b502d2c8bf97c4")]
public partial class FIndTargetFowardCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> Range;
    [SerializeReference] public BlackboardVariable<float> Pov;
    [SerializeReference] public BlackboardVariable<GameObject> TargetGo;
    
    private TestPolice _owner;
    
    private CctvRange _cctvRange;

    public override bool IsTrue()
    {
        return _cctvRange != null && _cctvRange.HasTarget;
    }

    public override void OnStart()
    {
        _owner = Self.Value.GetComponent<TestPolice>();
        if (_owner == null) return;

        _cctvRange = _owner.GetComponentInChildren<CctvRange>();
        if (_cctvRange == null) return;
        
        _cctvRange.InitRange(Pov.Value, Range.Value, _owner.TargetLayer);
        TargetGo.Value = _cctvRange.CurrentTarget?.gameObject;
    }

    public override void OnEnd()
    {
        
    }
}
