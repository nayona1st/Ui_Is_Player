using UnityEngine;

namespace PSW._01.Code.MapSystem
{
    public class GridPlacement : MonoBehaviour
    {
        private Grid _grid;

        public void Init(Grid g)
        {
            _grid = g;
        }

        public void Place(Transform target, Vector2Int cell)
        {
            Vector3 position = _grid.GetCellCenterWorld(new Vector3Int(cell.x, cell.y, 0));
            
            position.z = target.position.z;
            target.position = position;
        }
        
        public void Snap(Transform target)
        {
            Vector3Int cell = _grid.WorldToCell(target.position);
            Place(target, new Vector2Int(cell.x, cell.y));
        }
    }
}