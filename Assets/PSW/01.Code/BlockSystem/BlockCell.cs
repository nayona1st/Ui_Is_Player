using System;
using UnityEngine;

namespace PSW._01.Code.BlockSystem
{
    public class BlockCell : MonoBehaviour
    {
        public event Action OnTriggerEnter;
        public event Action OnTriggerExit;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            OnTriggerEnter?.Invoke();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            OnTriggerExit?.Invoke();
        }
    }
}