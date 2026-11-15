using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NKY.Script
{
    public class Agent : MonoBehaviour
    {
        [SerializeField] private PathAgent pathAgent;
        
        private Camera _cam;

        private void Awake()
        {
            _cam = Camera.main;
        }

        private void Update()
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                Vector3 mousePos = Mouse.current.position.ReadValue();
                // 2D Z=0 평면까지의 거리(보통 10f)를 Z에 넣어줍니다.
                mousePos.z = -_cam.transform.position.z; 
                Vector2 worldPos = _cam.ScreenToWorldPoint(mousePos);

                pathAgent.SetDestination(worldPos);
            }
        }
    }
}