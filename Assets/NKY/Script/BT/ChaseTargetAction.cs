using System;
using System.IO;
using _LumenLib.ModuleSystem;
using NKY.Script.Agent;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Chase Target", story: "Chase [Self] to [Target]", category: "Action", id: "87a6b97d65de4848261571852a4eacf4")]
public partial class ChaseTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private ModuleOwner _owner;
    private GameObject _target;
    
    private MovementModule _moveModule;

    protected override Status OnStart()
    {
        _owner = Self.Value.GetComponent<ModuleOwner>();
        _target = Target.Value;
        
        if(_owner == null)
            return Status.Failure;
        
        _moveModule = _owner.GetModule<MovementModule>();
        if(_moveModule == null)
            return Status.Failure;
        
        Vector2 targetPos =  _target.transform.position;
        if(!_moveModule.SetDestination(targetPos))
            return Status.Failure;
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if(_moveModule == null) return Status.Failure;
        
        Vector2 selfPos =  _owner.transform.position;
        Vector2 targetPos =  _target.transform.position;
        
        float distance = Vector2.Distance(selfPos, targetPos);
        
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

