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
        }

        #region Enable/Disable

        public void OnEnable()
        {
            ServiceLocator.Register<BlockSnapper>(this);
        }

        public void OnDisable()
        {
            ServiceLocator.Unregister<BlockSnapper>(this);
        }

        #endregion

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
    }
}