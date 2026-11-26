using System;
using _LumenLib.CoreSystem.EventChannelSystem.Runtime;
using NKY.Script.Agent;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

public class CatchEvt : GameEvent
{
    public GameObject CatchTarget;
    
    public CatchEvt(GameObject target)
    {
        CatchTarget = target;
    }
}

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CatchTarget", story: "[Self] Catch [Target]", category: "Action", id: "dd32236fa4e02b71a748d181a68be187")]
public partial class CatchTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private TestPolice _owner;

    protected override Status OnStart()
    {
        if(Self.Value == null)
            return Status.Failure;
        if(!Self.Value.TryGetComponent(out _owner))
            return Status.Failure;
        
        GameEvent evt = new CatchEvt(Target.Value);
        _owner.CatchEvent.RaiseEvent(evt);
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

