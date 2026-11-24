using System.Collections.Generic;
using _LumenLib.ServiceLocator;
using PSW._01.Code.CoreSystem;
using UnityEngine;

namespace PSW._01.Code.BlockSystem
{
    public class BlockShape : MonoBehaviour
    {
        [SerializeField] private BlockDataSO blockData;
        [SerializeField] private LayerMask whatIsBlock;

        [SerializeField, HideInInspector] private List<BlockCell> _blocks = new();
        private Vector3 _lastPos;
        private bool _followMouse = false;
        private bool _canSnap = true;
        private float _cellSize = 1;

        public void InitCell()
        {
            InitCell(ServiceLocator.Get<BlockSnapper>().GetCellSize().x);
        }

        public void InitCell(float cellSize)
        {
            _blocks.Add(GetComponent<BlockCell>());

            _cellSize = cellSize;

            foreach (var cell in blockData.cells)
            {
                BlockCell newCell = Instantiate(cell.cellPrefab, transform);
                newCell.transform.localPosition = cell.cellPos * _cellSize;
                _blocks.Add(newCell);
            }
        }

        private void OnEnable()
        {
            _lastPos = transform.position;
            
            foreach (var block in _blocks)
            {
                block.OnTriggerStay += HandleTriggerStay;
                block.OnTriggerExit += HandleTriggerExit;
            }
        }

        private void OnDisable()
        {
            foreach (var block in _blocks)
            {
                block.OnTriggerStay -= HandleTriggerStay;
                block.OnTriggerExit -= HandleTriggerExit;
            }
        }

        private void Start()
        {
            Snapping();
        }

        public void Snapping()
        {
            foreach (var block in _blocks)
            {
                ServiceLocator.Get<BlockSnapper>().Snap(block.transform);
            }
            
            _canSnap = !CheckBlock();

            if (!_canSnap)
            {
                transform.position = _lastPos;
                _canSnap = true;
                Snapping();
            }

            _followMouse = false;
            _lastPos = transform.position;
        }
        
        private bool CheckBlock()
        {
            Physics2D.SyncTransforms();

            foreach (var block in _blocks)
            {
                var hits = Physics2D.OverlapPointAll(
                    block.transform.position, whatIsBlock);

                foreach (var hit in hits)
                {
                    var shape = hit.GetComponentInParent<BlockShape>();

                    if (shape != null && shape != this)
                        return true;
                }
            }

            return false;
        }

        private void Update()
        {
            if (!_followMouse) return;
            
            FollowMouse();
        }

        public void FollowMouse()
        {
            transform.position = MouseClass.GetScreenToWorldPos();
        }

        public void HoldBlock()
        {
            _lastPos = transform.position;
            _followMouse = true;
        }

        private void HandleTriggerStay()
        {
            // _canSnap = false;
        }

        private void HandleTriggerExit()
        {
            // _canSnap = true;
        }
    }
}