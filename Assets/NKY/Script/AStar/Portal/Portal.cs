using NKY.Script.AStar.Grid;

namespace NKY.Script.AStar.Portal
{
    public class Portal
    {
        public int PortalId { get; }
        
        public INode Node { get; }
        public IChunk Chunk { get; }

        public Portal(int id, INode node, IChunk chunk)
        {
            PortalId = id;
            Node = node;
            Chunk = chunk;
        }
    }
}