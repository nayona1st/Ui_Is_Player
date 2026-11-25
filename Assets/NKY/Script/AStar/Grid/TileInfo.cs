namespace NKY.Script.AStar.Grid
{
    public struct TileInfo
    {
        public int Cost;
        public bool CanMove;

        public TileInfo(int cost, bool canMove)
        {
            Cost = cost;
            CanMove = canMove;
        }
    }
}