using System;
using System.Collections.Generic;
using _LumenLib.ServiceLocator;
using PSW._01.Code.CoreSystem;
using UnityEngine;

namespace PSW._01.Code.BlockSystem
{
    public class BlockShape : MonoBehaviour
    {
        [SerializeField] private List<BlockCell> blocks = new();

        private Vector3 _lastTransform;
        private bool _followMouse = false;

        private bool _canSnap = true;

        private void OnEnable()
        {
            foreach (var block in blocks)
            {
                block.OnTriggerEnter += HandleTriggerEnter;
                block.OnTriggerExit += HandleTriggerExit;
            }
        }

        private void OnDisable()
        {
            foreach (var block in blocks)
            {
                block.OnTriggerEnter -= HandleTriggerEnter;
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

            _followMouse = false;
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
            _lastTransform = transform.position;
            _followMouse = true;
        }

        private void HandleTriggerEnter()
        {
            _canSnap = false;
        }

        private void HandleTriggerExit()
        {
            _canSnap = true;
        }
    }
}