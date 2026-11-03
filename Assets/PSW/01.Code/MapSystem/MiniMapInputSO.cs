using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PSW._01.Code.MapSystem
{
    [CreateAssetMenu(fileName = "MiniMap Input", menuName = "So/Map/Input", order = 0)]
    public class MiniMapInputSO : ScriptableObject, MapControls.IPlayerActions
    {
        public event Action OnMouseDown;
        public event Action OnMouseUp;
        
        private Vector2 _mousePos;

        public Vector2 MousePos => Camera.main.ScreenToWorldPoint(_mousePos);

        private MapControls _controls;
        
        public void SetEnable()
        {
            _controls ??= new MapControls();
            _controls.Enable();
            _controls.Player.SetCallbacks(this);
        }

        public void SetDisable()
        {
            if(_controls != null)
                _controls.Disable();
            _controls = null;
        }

        public void OnMouse(InputAction.CallbackContext context)
        {
            _mousePos = context.ReadValue<Vector2>();
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            if(context.started)
                OnMouseDown?.Invoke();
            if(context.canceled)
                OnMouseUp?.Invoke();
        }
    }
}