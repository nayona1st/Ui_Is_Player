using System;
using System.Collections.Generic;
using NKY.Script.AStar.Core;
using NKY.Script.AStar.Graph;
using NKY.Script.AStar.Grid;
using NKY.Script.AStar.Nodes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NKY.Script
{
[RequireComponent(typeof(Collider2D))]
    public class PathAgent : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Collider2D agentCollider;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float nodeReachThreshold = 0.05f;

        private float _cachedRadius;
        private GraphAStar _aStar;

        private List<IPathNode> _currentPath = new();
        private int _currentPathIndex;
        private bool _isMoving;

        public bool IsMoving => _isMoving;
        public event Action OnDestinationReached;
        public event Action OnPathFailed;
        
        private PathGridManager _gridManager;

        private void Awake()
        {
            if (agentCollider == null)
                agentCollider = GetComponent<Collider2D>();

            _cachedRadius = CalculateWorldRadius();
            _gridManager = PathGridManager.Instance;
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

        private void Update()
        {
            if (!_isMoving || _currentPath == null || _currentPathIndex >= _currentPath.Count)
                return;

            MoveAlongPath();
        }
        
        public bool SetDestination(Vector2 destination)
        {
            if (_gridManager == null || _gridManager.Grid == null)
            {
                return false;
            }

            if (_aStar == null)
            {
                _aStar = new GraphAStar(_gridManager.GraphProvider);
            }

            Vector2Int startGridPos = GetPathStartGridPos();
            Vector2Int goalGridPos = _gridManager.Grid.WorldToGrid(destination);

            if (!_gridManager.GraphProvider.TryGetCellNode(startGridPos, out CellPathNode startNode) ||
                !_gridManager.GraphProvider.TryGetCellNode(goalGridPos, out CellPathNode goalNode))
            {
                StopMovement();
                OnPathFailed?.Invoke();
                return false;
            }

            // 시작 셀과 목표 셀이 같으면 A*가 빈 리스트를 반환하므로 노드 하나짜리 경로로 처리
            List<IPathNode> path = startGridPos == goalGridPos
                ? new List<IPathNode> { goalNode }
                : _aStar.FindPath(startNode, goalNode, _cachedRadius);

            if (path == null || path.Count == 0)
            {
                StopMovement();
                OnPathFailed?.Invoke();
                return false;
            }

            _currentPath = path;
            _currentPathIndex = 0;
            _isMoving = true;
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

            return _gridManager.Grid.WorldToGrid(transform.position);
        }

        public void StopMovement()
        {
            _isMoving = false;
            _currentPathIndex = 0;
            _currentPath?.Clear();
        }

        private void MoveAlongPath()
        {
            Vector2 targetWorldPos = _currentPath[_currentPathIndex].WorldPosition;
            Vector2 currentPos = transform.position;
            
            transform.position = Vector2.MoveTowards(currentPos, targetWorldPos, moveSpeed * Time.deltaTime);
            
            if (Vector2.Distance(transform.position, targetWorldPos) <= nodeReachThreshold)
            {
                _currentPathIndex++;

                if (_currentPathIndex >= _currentPath.Count)
                {
                    _isMoving = false;
                    OnDestinationReached?.Invoke();
                }
            }
        }

        private float CalculateWorldRadius()
        {
            if (agentCollider is CircleCollider2D circle)
            {
                Vector3 scale = circle.transform.lossyScale;
                float maxScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y));
                return circle.radius * maxScale * 0.6f;
            }

            if (agentCollider != null)
            {
                Vector3 extents = agentCollider.bounds.extents;
                return Mathf.Max(extents.x, extents.y) * 0.6f;
            }

            return 0.5f * 0.6f;
        }

        private void OnDrawGizmosSelected()
        {
            if (!_isMoving || _currentPath == null || _currentPath.Count == 0)
                return;

            Gizmos.color = Color.green;
            for (int i = _currentPathIndex; i < _currentPath.Count - 1; i++)
            {
                Gizmos.DrawLine(_currentPath[i].WorldPosition, _currentPath[i + 1].WorldPosition);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_currentPath[_currentPathIndex].WorldPosition, 0.2f);
        }
    }
}