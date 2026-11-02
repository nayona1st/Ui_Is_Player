using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PSW._01.Code.MapSystem
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private MiniMapInputSO Input;
        
        [SerializeField] private LayerMask chunkLayerMask;
        [SerializeField] private GridPlacement chunkPrefab;
        [SerializeField] private Transform chunkParent;
        [SerializeField] private Grid grid;
        private GridPlacement _holdGrid;

        private void OnEnable()
        {
            Input.SetEnable();
            
            Input.OnMouseDown += HandleMouseDown;
            Input.OnMouseUp += HandleMouseUp;
        }

        private void OnDisable()
        {
            Input.OnMouseDown -= HandleMouseDown;
            Input.OnMouseUp -= HandleMouseUp;
            
            Input.SetDisable();
        }

        private void Start()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    GridPlacement chunk = Instantiate(chunkPrefab, chunkParent);
                    chunk.Init(grid);
                    chunk.Place(chunk.transform, new Vector2Int(i, j));
                }
            }
        }

        private void Update()
        {
            if (_holdGrid != null)
            {
                _holdGrid.transform.position = Input.MousePos;
            }
        }

        private void HandleMouseDown()
        {
            var cell = Physics2D.Raycast(Input.MousePos, Vector2.zero);
            if (cell.collider != null)
            {
                _holdGrid = cell.collider.GetComponent<GridPlacement>();
            }
        }

        private void HandleMouseUp()
        {
            if (_holdGrid == null) return;
            
            _holdGrid.Snap(_holdGrid.transform);
            _holdGrid = null;
        }
    }
}
