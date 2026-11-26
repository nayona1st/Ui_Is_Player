using System;
using System.Collections.Generic;
using System.Threading;
using _LumenLib.ModuleSystem;
using Cysharp.Threading.Tasks;
using NKY.Script.AStar.Core;
using NKY.Script.AStar.Graph;
using NKY.Script.AStar.Grid;
using NKY.Script.AStar.Nodes;
using UnityEngine;

namespace NKY.Script.Agent
{
    public enum MovementState
    {
        Idle,
        Moving,
        Arrived,
        PathFailed
    }
    
    public class MovementModule : MonoModule
    { 

        [Header("Movement Settings")]
        [Tooltip("초당 회전할수 있는 각도")]
        [SerializeField] private float rotationSpeed = 720f;
        [field: SerializeField] public float MoveSpeed { get; private set; } = 5f;
        [SerializeField] private float nodeReachThreshold = 0.05f;

        private Collider2D _agentCollider;
        private Transform _ownerTrm;
        private Rigidbody2D _ownerRb;
        private float _cachedRadius;
        private GraphAStar _aStar;
        private List<IPathNode> _currentPath = new();
        private int _currentPathIndex;
        private bool _isMoving;
        private Vector2 _destination;
        private bool _hasDestination;
        
        public float CurrentSpeed { get; private set; }
        public MovementState State { get; private set; }

        public bool IsMoving => _isMoving;
        public event Action OnDestinationReached;
        public event Action OnPathFailed;
        
        private PathGridManager _gridManager;

        public override void Initialize(ModuleOwner owner)
        {
            base.Initialize(owner);
            _ownerTrm = owner.transform;
            _ownerRb = owner.GetComponent<Rigidbody2D>();
            _agentCollider = owner.GetComponent<Collider2D>();
            if(_agentCollider == null)
                Debug.LogWarning("owner collider not found");
        }

        private void Awake()
        {
            if (_agentCollider == null)
                _agentCollider = GetComponent<Collider2D>();

            _cachedRadius = CalculateWorldRadius();
            _gridManager = PathGridManager.Instance;
            CurrentSpeed = MoveSpeed;
        }

        private void Start()
        {
            if (_gridManager == null)
                _gridManager = FindFirstObjectByType<PathGridManager>();

            if (_gridManager != null && _gridManager.GraphProvider != null)
            {
                _aStar = new GraphAStar(_gridManager.GraphProvider);
            }
        }

        private void FixedUpdate()
        {
            if (!_isMoving || _currentPath == null ||
                _currentPathIndex >= _currentPath.Count)
                return;

            MoveAlongPath();
        }

        public void SetSpeed(float speed)
        {
            CurrentSpeed = speed;
        }

        public void InitSpeed()
        {
            CurrentSpeed = MoveSpeed;
        }

        public async UniTaskVoid TargetChaseDestination(
            GameObject target, CancellationToken token)
        {
            Vector2Int lastTargetCell = new Vector2Int(int.MinValue, int.MinValue);

            while (!token.IsCancellationRequested && target != null)
            {
                Vector2Int currentTargetCell = _gridManager.Grid.WorldToGrid(target.transform.position);

                bool targetMoved = currentTargetCell != lastTargetCell;
                bool pathFailed = State == MovementState.PathFailed;

                if (targetMoved || pathFailed)
                {
                    SetDestination(target.transform.position);
                    lastTargetCell = currentTargetCell;
                }

                bool isCanceled = await UniTask.Delay(100, cancellationToken: token)
                    .SuppressCancellationThrow();

                if (isCanceled)
                    break;
            }
        }
        
        public bool SetDestination(Vector2 destination)
        {
            _destination = destination;
            _hasDestination = true;

            return RefreshDestination();
        }

        public bool RefreshDestination()
        {
            if (!_hasDestination || _gridManager == null ||
                _gridManager.Grid == null)
                return false;

            if (_aStar == null)
                _aStar = new GraphAStar(_gridManager.GraphProvider);

            StopPathOnly();

            Vector2Int startGridPos =
                _gridManager.Grid.WorldToGrid(_ownerTrm.position);

            Vector2Int goalGridPos =
                _gridManager.Grid.WorldToGrid(_destination);

            if (!_gridManager.GraphProvider.TryGetCellNode(
                    startGridPos, out CellPathNode startNode) ||
                !_gridManager.GraphProvider.TryGetCellNode(
                    goalGridPos, out CellPathNode goalNode))
            {
                State = MovementState.PathFailed;
                OnPathFailed?.Invoke();
                return false;
            }

            List<IPathNode> path = startGridPos == goalGridPos
                ? new List<IPathNode> { goalNode }
                : _aStar.FindPath(startNode, goalNode, _cachedRadius);
            
            if (path == null || path.Count == 0)
            {
                State = MovementState.PathFailed;
                OnPathFailed?.Invoke();
                return false;
            }
            if (path.Count > 1)
            {
                Vector2Int firstNodeGridPos =
                    _gridManager.Grid.WorldToGrid(path[0].WorldPosition);

                if (firstNodeGridPos == startGridPos)
                {
                    path.RemoveAt(0);
                }
            }

            _currentPath = path;
            _currentPathIndex = 0;

            _isMoving = true;
            State = MovementState.Moving;

            return true;
        }

        // 이동 중이면 지금 향하고 있는 노드의 셀, 아니면 현재 위치의 셀
        private Vector2Int GetPathStartGridPos()
        {
            if (_isMoving && _currentPath != null &&
                _currentPathIndex < _currentPath.Count &&
                _currentPath[_currentPathIndex] is CellPathNode heading)
            {
                return heading.SourceNode.NodeGridPos;
            }

            return _gridManager.Grid.WorldToGrid(_ownerTrm.position);
        }
        
        private void StopPathOnly()
        {
            _isMoving = false;
            _ownerRb.linearVelocity = Vector2.zero;
            _currentPathIndex = 0;
            _currentPath.Clear();
        }

        public void StopMovement()
        {
            StopPathOnly();

            _hasDestination = false;
            State = MovementState.Idle;
        }
        
        private void MoveAlongPath()
        {
            Vector2 targetWorldPos = _currentPath[_currentPathIndex].WorldPosition;
            Vector2 currentPos = _ownerTrm.position;

            Vector2 direction = targetWorldPos - currentPos;
            float distance = direction.magnitude;
            
            float moveDistance = CurrentSpeed * Time.fixedDeltaTime;
            

            if (distance <= nodeReachThreshold + moveDistance)
            {
                _ownerTrm.position = targetWorldPos;

                _currentPathIndex++;

                if (_currentPathIndex >= _currentPath.Count)
                {
                    _isMoving = false;
                    _ownerRb.linearVelocity = Vector2.zero;

                    State = MovementState.Arrived;
                    _hasDestination = false;

                    OnDestinationReached?.Invoke();
                }

                return;
            }
            RotateTowards(direction);
            _ownerRb.linearVelocity = direction.normalized * CurrentSpeed;
        }
        
        private void RotateTowards(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.01f)
                return;

            float targetAngle = Mathf.Atan2(direction.y, direction.x)
                                * Mathf.Rad2Deg;

            float currentAngle = _ownerRb.rotation;

            float nextAngle = Mathf.MoveTowardsAngle(
                currentAngle,
                targetAngle,
                rotationSpeed * Time.fixedDeltaTime
            );

            _ownerRb.MoveRotation(nextAngle);
        }

        private float CalculateWorldRadius()
        {
            if (_agentCollider is CircleCollider2D circle)
            {
                Vector3 scale = circle.transform.lossyScale;
                float maxScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y));
                return circle.radius * maxScale * 0.6f;
            }

            if (_agentCollider != null)
            {
                Vector3 extents = _agentCollider.bounds.extents;
                return Mathf.Max(extents.x, extents.y) * 0.6f;
            }

            return 0.5f * 0.6f;
        }
    }
}