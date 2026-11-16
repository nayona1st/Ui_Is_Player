using System;
using UnityEngine;

namespace PSW._01.Code.BlockSystem
{
    public class BlockCell : MonoBehaviour
    {
        public event Action OnTriggerStay;
        public event Action OnTriggerExit;

        private void OnTriggerStay2D(Collider2D other)
        {
            OnTriggerStay?.Invoke();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            OnTriggerExit?.Invoke();
        }
    }
}