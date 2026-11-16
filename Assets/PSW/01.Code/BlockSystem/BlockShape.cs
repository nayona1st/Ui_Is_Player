using System.Collections.Generic;
using _LumenLib.CoreSystem.EventChannelSystem.Runtime;
using _LumenLib.ServiceLocator;
using NKY.Script.AStar.Grid;
using PSW._01.Code.CoreSystem;
using UnityEngine;

namespace PSW._01.Code.BlockSystem
{
    public class BlockShape : MonoBehaviour
    {
        [SerializeField] private EventChannelSO evtChannel;
        [SerializeField] private List<BlockCell> blocks = new();
        [SerializeField] private LayerMask whatIsBlock;

        private Vector3 _lastPos;
        private bool _followMouse = false;
        [SerializeField] private bool _canSnap = true;

        private void OnEnable()
        {
            _lastPos = transform.position;
            
            foreach (var block in blocks)
            {
                block.OnTriggerStay += HandleTriggerStay;
                block.OnTriggerExit += HandleTriggerExit;
            }
        }

        private void OnDisable()
        {
            foreach (var block in blocks)
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
            foreach (var block in blocks)
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
            
            evtChannel.RaiseEvent(BakeMapEvent.Instance);

            _followMouse = false;
            _lastPos = transform.position;
        }
        
        private bool CheckBlock()
        {
            Physics2D.SyncTransforms();

            foreach (var block in blocks)
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