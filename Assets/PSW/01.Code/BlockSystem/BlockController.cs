    
    using _LumenLib.CoreSystem.EventChannelSystem.Runtime;
    using NKY.Script.AStar.Grid;
    using PSW._01.Code.CoreSystem;
    using UnityEngine;

    namespace PSW._01.Code.BlockSystem
{
    public class BlockController : MonoBehaviour
    {
        [SerializeField] private EventChannelSO evtChannel;
        
        [SerializeField] private MapInputSO input;
        [SerializeField] private LayerMask whatIsBlock;

        private BlockShape _holdingBlock;

        private void OnEnable()
        {
            input.SetEnable();

            input.OnMouseDown += HandleMouseDown;
            input.OnMouseUp += HandleMouseUp;
            input.OnRotateInput += HandleRotateBlock;
        }

        private void OnDisable()
        {
            input.OnMouseDown -= HandleMouseDown;
            input.OnMouseUp -= HandleMouseUp;
            input.OnRotateInput -= HandleRotateBlock;
            
            input.SetDisable();
        }

        private void HandleMouseDown()
        {
            var block = Physics2D.Raycast(MouseClass.GetScreenToWorldPos(), Vector2.zero, whatIsBlock);

            if (block && CheckBlock(block, out BlockShape blockShape))
            {
                _holdingBlock = blockShape;
                _holdingBlock.HoldBlock();
            }
        }
        
        private void HandleRotateBlock()
        {
            if (_holdingBlock == null) return;
            
            Quaternion currentRotation = transform.rotation;

            _holdingBlock.transform.rotation *= 
                Quaternion.Euler(currentRotation.eulerAngles.x, currentRotation.eulerAngles.y, currentRotation.eulerAngles.z + 90);
            
            evtChannel.RaiseEvent(BakeMapEvent.Instance);
        }

        private void HandleMouseUp()
        {
            if (_holdingBlock != null)
            {
                _holdingBlock.Snapping();
                _holdingBlock = null;
            }
            
            evtChannel.RaiseEvent(BakeMapEvent.Instance);
        }

        private bool CheckBlock(RaycastHit2D hit, out BlockShape hitShape)
        {
            if (hit)
            {
                hitShape = hit.collider.GetComponentInParent<BlockShape>();
                return true;
            }
            
            hitShape = null;
            return false;
        }
    }
}