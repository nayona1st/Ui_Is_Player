using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGW.game._00.Codes
{
    public class Hide : MonoBehaviour
    {
        private float _interactRange = 10f;
        public Action inRange;


        private void Update()
        {
            CheckingRange();
        }

        private void CheckingRange()
        {
            var hit2D = Physics2D.OverlapCircle(transform.position, _interactRange);
            if(hit2D != null &&  hit2D.gameObject.CompareTag("Player"))
            {
                inRange?.Invoke();
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    
                }
            }
                
        }
    }
}