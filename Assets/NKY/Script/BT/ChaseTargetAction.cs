using System;
using System.IO;
using System.Threading;
using _LumenLib.ModuleSystem;
using Cysharp.Threading.Tasks;
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

    private CancellationTokenSource _cts;

    protected override Status OnStart()
    {
        _owner = Self.Value.GetComponent<ModuleOwner>();
        _target = Target.Value;
        
        if(_owner == null)
            return Status.Failure;
        
        _moveModule = _owner.GetModule<MovementModule>();
        if(_moveModule == null)
            return Status.Failure;
        
        if (_cts != null)
        {
            CleanupCts();
        }
        
        _cts = CancellationTokenSource.CreateLinkedTokenSource(
            _owner.GetCancellationTokenOnDestroy()
        );
        
        _moveModule.TargetChaseDestination(_target, _cts.Token).Forget();
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if(_target == null) return Status.Failure;
        if(_moveModule == null) return Status.Failure;
        
        Vector2 selfPos =  _owner.transform.position;
        Vector2 targetPos =  _target.transform.position;
        
        float distance = Vector2.Distance(selfPos, targetPos);

        if (distance < 0.5f)
        {
            return Status.Success;
        }
        
        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (_moveModule == null) return;

        CleanupCts();
        _moveModule.StopMovement();
    }
    
    private void CleanupCts()
    {
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }
}

