using System.Collections.Generic;
using _LumenLib.ServiceLocator;
using PSW._01.Code.CoreSystem;
using TMPro;
using Unity.Android.Gradle;
using UnityEngine;

namespace PSW._01.Code.BlockSystem
{
    public class BlockShape : MonoBehaviour
    {
        [SerializeField] private BlockDataSO blockData;
        [SerializeField] private LayerMask whatIsBlock;

        [SerializeField, HideInInspector] private List<BlockCell> _blocks = new();
        private Vector3 _lastPos;
        private Quaternion _lastRotation;
        private bool _followMouse = false;
        private bool _canSnap = true;

        public BlockDataSO BlockData => blockData;

        public void SetCells(IEnumerable<BlockCell> cells)
        {
            bool updateSubscriptions = Application.isPlaying && isActiveAndEnabled;
            if (updateSubscriptions) UnsubscribeFromCells();

            _blocks.Clear();
            _blocks.AddRange(cells);

            if (updateSubscriptions) SubscribeToCells();
        }

        private void OnEnable()
        {
            _lastPos = transform.position;
            _lastRotation  = transform.rotation;
            SubscribeToCells();
        }

        private void OnDisable()
        {
            UnsubscribeFromCells();
        }

        private void SubscribeToCells()
        {
            foreach (var block in _blocks)
            {
                if (block == null) continue;

                block.OnTriggerStay += HandleTriggerStay;
                block.OnTriggerExit += HandleTriggerExit;
            }
        }

        private void UnsubscribeFromCells()
        {
            foreach (var block in _blocks)
            {
                if (block == null) continue;

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
                transform.rotation = _lastRotation;
                _canSnap = true;
                _followMouse = false;
                return;
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
            _lastRotation = transform.rotation;
            
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
