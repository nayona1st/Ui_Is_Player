using _LumenLib.ServiceLocator;
using UnityEngine;

namespace PSW._01.Code.BlockSystem
{
    public class BlockSnapper : MonoBehaviour
    {
        private Grid _grid;

        private void Awake()
        {
            _grid = GetComponent<Grid>();

            ServiceLocator.Register<BlockSnapper>(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<BlockSnapper>(this);
        }

        private void Place(Transform target, Vector3Int cell)
        {
            Vector3 position = _grid.GetCellCenterWorld(cell);
            target.position = position;
        }

        public void Snap(Transform target)
        {
            Vector3Int cell = _grid.WorldToCell(target.position);
            Place(target, cell);
        }

        public Vector2 GetCellSize()
        {
            if (_grid == null) _grid = GetComponent<Grid>();

            return _grid.cellSize;
        }
    }
}