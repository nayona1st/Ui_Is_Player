using System;
using _LumenLib.ModuleSystem;
using NKY.Script.Agent;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Patrol", story: "Patrol [Self] (close path wait [BlockedWaitTime])", category: "Action", id: "13a7729cc7b920a5de327d4edc7a26be")]
public partial class PatrolAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> BlockedWaitTime;

    private float _blockedTimer;
    
    private ModuleOwner _owner;
    private MovementModule _moveModule;
    private PatrolPathModule _patrolModule;
    
    private Transform _currentPatrolPoint;

    protected override Status OnStart()
    {
        if(!Self.Value.TryGetComponent(out _owner)) 
            return Status.Failure; 
        
        _moveModule = _owner.GetModule<MovementModule>();
        _patrolModule = _owner.GetModule<PatrolPathModule>();
        
        if(_moveModule == null ||  _patrolModule == null)
            return Status.Failure;

        _currentPatrolPoint = _patrolModule.GetNextPatrolPoint();
        if(!_moveModule.SetDestination(_currentPatrolPoint.position))
            return Status.Failure;
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_moveModule == null || _patrolModule == null)
            return Status.Failure;
        
        if (_moveModule.State == MovementState.PathFailed)
        {
            _blockedTimer += Time.deltaTime;

            if (_blockedTimer >= BlockedWaitTime.Value)
            {
                _blockedTimer = 0f;
                MoveToNextPoint();
            }

            return Status.Running;
        }

        _blockedTimer = 0f;

        if (_moveModule.State == MovementState.Arrived)
        {
            _owner.transform.position = _currentPatrolPoint.position;

            if (!MoveToNextPoint())
            {
                _blockedTimer = 0f;
            }
        }

        return Status.Running;
    }

    private bool MoveToNextPoint()
    {
        _moveModule.StopMovement();

        _currentPatrolPoint = _patrolModule.GetNextPatrolPoint();

        if (_currentPatrolPoint == null)
            return false;

        return _moveModule.SetDestination(
            _currentPatrolPoint.position);
    }

    protected override void OnEnd()
    {
        if (_moveModule == null) return;
        
        _moveModule.StopMovement();
    }
}

