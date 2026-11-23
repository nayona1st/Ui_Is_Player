using System;
using _LumenLib.ModuleSystem;
using NKY.Script.Agent;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveToPoint", story: "Move [Self] to [Point]", category: "Action", id: "34ce48c99026deedc41595d9020666a9")]
public partial class MoveToPointAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Vector2> Point;

    private ModuleOwner _owner;
    private PathAgent _moveModule;

    private Vector2 _endPosition;

    protected override Status OnStart()
    {
        if(!Self.Value.TryGetComponent(out _owner))
            return Status.Failure;
        
        _moveModule = _owner.GetModule<PathAgent>();
        
        if(_moveModule == null)
            return Status.Failure;
        
        _endPosition = Point.Value;
        
        if(!_moveModule.SetDestination(_endPosition))
            return Status.Failure;
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if(_moveModule == null) return Status.Failure;
        
        Vector2 selfPos =  _owner.transform.position;
        
        float distance = Vector2.Distance(selfPos, _endPosition);
        
        if(distance < 0.5f)
            return Status.Success;
        
        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (_moveModule == null) return;
        
        _moveModule.StopMovement();
    }
}

