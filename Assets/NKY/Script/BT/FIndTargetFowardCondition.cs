using System;
using _LumenLib.ModuleSystem;
using GGW.game._00.Codes.CCTV;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "FIndTargetFoward", story: "Find [Self] in [Range] and [pov]", category: "Conditions", id: "7b4957a00eb45e8975b502d2c8bf97c4")]
public partial class FIndTargetFowardCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> Range;
    [SerializeReference] public BlackboardVariable<float> Pov;
    
    private ModuleOwner _owner;
    
    private CctvRange _cctvRange;

    private bool _isSuccess = false;

    public override bool IsTrue()
    {
        return _isSuccess;
    }

    public override void OnStart()
    {
        _isSuccess = false;
        
        _owner = Self.Value.GetComponent<ModuleOwner>();

        if (_owner == null)
            return;
        
        _cctvRange = _owner.GetComponentInChildren<CctvRange>();

        if (_cctvRange == null)
            return;
        
        _cctvRange.InitRange(Pov.Value, Range.Value);
        _cctvRange.FindPlayer += FindingTarget;
        _cctvRange.SetFinding(true);
    }

    public override void OnEnd()
    {
        if (_cctvRange == null)
            return;
        
        _cctvRange.SetFinding(false);
        _cctvRange.FindPlayer -= FindingTarget;
        _isSuccess = false;
    }

    public void FindingTarget(Transform target)
    {
        _isSuccess = true;
    }
}
