using System;
using UnityEngine;

namespace PSW._01.Code.BlockSystem
{
    [Serializable]
    public struct CellData
    {
        public Vector2 cellPos;
        public BlockCell cellPrefab;
    }
    
    [CreateAssetMenu(fileName = "BlockData", menuName = "SO/Block Data", order = 0)]
    public class BlockDataSO : ScriptableObject
    {
        public CellData[] cells = { new() };
    }
}