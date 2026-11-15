    using PSW._01.Code.CoreSystem;
    using UnityEngine;

namespace PSW._01.Code.BlockSystem
{
    public class BlockController : MonoBehaviour
    {
        [SerializeField] private MapInputSO input;
        [SerializeField] private LayerMask whatIsBlock;

        private BlockShape _holdingBlock;

        private void OnEnable()
        {
            input.SetEnable();

            input.OnMouseDown += HandleMouseDown;
            input.OnMouseUp += HandleMouseUp;
        }

        private void OnDisable()
        {
            input.OnMouseDown -= HandleMouseDown;
            input.OnMouseUp -= HandleMouseUp;
            
            input.SetDisable();
        }

        private void HandleMouseDown()
        {
            var block = Physics2D.Raycast(MouseClass.GetScreenToWorldPos(), Vector2.zero, whatIsBlock);

            if (CheckBlock(block, out BlockShape blockShape))
            {
                _holdingBlock = blockShape;
                _holdingBlock.HoldBlock();
            }
        }

        private void HandleMouseUp()
        {
            if (_holdingBlock == null) return;
            
            _holdingBlock.Snapping();
            _holdingBlock = null;
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