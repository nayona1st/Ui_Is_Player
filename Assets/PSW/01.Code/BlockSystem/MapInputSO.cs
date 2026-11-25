using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PSW._01.Code.BlockSystem
{
    [CreateAssetMenu(fileName = "MiniMap Input", menuName = "So/Map/Input", order = 0)]
    public class MapInputSO : ScriptableObject, MapControls.IPlayerActions
    {
        public event Action OnRotateInput;
        public event Action OnMouseDown;
        public event Action OnMouseUp;
        
        private MapControls _controls;
        private Camera _mainCam;
        
        public void SetEnable()
        {
            if(_mainCam == null)
                _mainCam = Camera.main;
            
            if(_controls == null)
                _controls = new MapControls();
            _controls.Enable();
            _controls.Player.SetCallbacks(this);
        }

        public void SetDisable()
        {
            if(_controls != null)
                _controls.Disable();
            _controls = null;
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            if(context.started)
                OnMouseDown?.Invoke();
            if(context.canceled)
                OnMouseUp?.Invoke();
        }

        public void OnRotate(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnRotateInput?.Invoke();
        }
    }
}