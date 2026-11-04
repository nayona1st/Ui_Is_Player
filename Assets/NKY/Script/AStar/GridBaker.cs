using UnityEngine;

namespace NKY.Script.AStar
{
    public class GridBaker
    {
        public LayerMask obstacleLayerMask;

        public GridBaker(LayerMask obstacleMask)
        {
            obstacleLayerMask = obstacleMask;
        }
    
        public void Bake(IGridProvider grid)
        {
            var boxSize = new Vector2(grid.CellSize, grid.CellSize);
            
            foreach (var chunk in grid.Chunks.Values)
            {
                for (int x = 0; x < chunk.SizeX; x++)
                for (int y = 0; y < chunk.SizeY; y++)
                {
                    INode node = chunk.Nodes[x, y];
                    bool isObstacle = Physics2D.OverlapBox(
                        node.NodeWorldPos, boxSize, 0, obstacleLayerMask);

                    node.SetNode(1, !isObstacle);
                }
            }
        }
    }
}