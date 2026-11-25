using System.Threading;

namespace NKY.Script.AStar.Core
{
    public static class PathNodeIdIssuer
    {
        private static int _lastId = -1;
 
        public static int Issue()
        {
            return Interlocked.Increment(ref _lastId);
        }
    }
}