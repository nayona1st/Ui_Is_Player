using UnityEngine;
using UnityEngine.InputSystem;

namespace PSW._01.Code.CoreSystem
{
    public static class MouseClass
    {
        private static Mouse _mouse = Mouse.current;
        private static Camera _mainCam = Camera.main;

        public static Vector2 GetScreenPos()
        {
            return _mouse.position.ReadValue();
        }

        public static Vector2 GetScreenToWorldPos()
        {
            return _mainCam.ScreenToWorldPoint(GetScreenPos());
        }
    }
}