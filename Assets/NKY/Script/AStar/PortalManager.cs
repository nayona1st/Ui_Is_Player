using System;
using System.Collections.Generic;
using UnityEngine;

namespace NKY.Script.AStar
{
    public class PortalManager
    {
        public Dictionary<int, Portal> Portals { get; } = new();
        public Dictionary<int, PortalConnection> Connections { get; } = new();
        
        private readonly IGridProvider _grid;
        private int _nextPortalId = 0;
        private int _nextConnectionId = 0;
        
        public PortalManager(IGridProvider grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        }

        public bool CreatePortal(INode node, IChunk chunk, out Portal portal)
        {
            portal = null;
            if (node == null || chunk == null)
                return false;

            Vector2Int localPos = node.NodeGridPos - chunk.OriginGridPos;

            if (!chunk.GetNode(localPos, out INode chunkNode))
                return false;

            if (!ReferenceEquals(node, chunkNode))
                return false;
            
            if(TryGetPortal(node.NodeGridPos, out var port))
                return false;

            Portal newPortal = new Portal(_nextPortalId++, node, chunk);
            Portals.Add(newPortal.PortalId, newPortal);
            portal = newPortal;
            return true;
        }

        public bool CreateConnection(Portal a, Portal b, bool isBidirectional, bool isPassable)
        {
            if (a == null || b == null) return false;
            if(a.PortalId ==  b.PortalId || ReferenceEquals(a, b)) return false;
            if (!Portals.TryGetValue(a.PortalId, out var registeredA) ||
                !ReferenceEquals(registeredA, a))
                return false;
            if (!Portals.TryGetValue(b.PortalId, out var registeredB) ||
                !ReferenceEquals(registeredB, b))
                return false;

            foreach (var connection in Connections.Values)
            {
                if(connection.Connects(a, b)) return  false;
            }
            
            PortalConnection newPortalConnection = new PortalConnection(_nextConnectionId++, a, b,
                isBidirectional, isPassable);
            Connections.Add(newPortalConnection.ConnectionId, newPortalConnection);
            return true;
        }

        public bool RemovePortal(int portalId)
        {
            if (!Portals.ContainsKey(portalId))
                return false;
            
            List<int> destroyIds = new List<int>();
            foreach (var connection in Connections.Values)
            {
                if (connection.ConnectedPortalA.PortalId == portalId ||
                    connection.ConnectedPortalB.PortalId == portalId)
                {
                    destroyIds.Add(connection.ConnectionId);
                }
            }

            foreach (var id in destroyIds)
            {
                RemoveConnection(id);
            }
            Portals.Remove(portalId);
            return true;
        }

        public bool RemoveConnection(int connectionId)
        {
            return Connections.Remove(connectionId);
        }

        public bool SetConnectionPassable(int connectionId, bool isPassable)
        {
            if(!Connections.TryGetValue(connectionId, out var connection))
                return false;
            connection.IsPassable = isPassable;
            return true;
        }
        
        public List<PortalConnection> GetConnections(Portal portal)
        {
            List<PortalConnection> connections = new();

            if (portal == null)
                return connections;

            foreach (var connection in Connections.Values)
            {
                if (!connection.HavePortal(portal)) continue;
                
                Portal counterpartPortal;
                // 1. 현재 포털에서 연결된 상대 포털 찾기
                if (connection.ConnectedPortalA == portal)
                    counterpartPortal = connection.ConnectedPortalB;
                else
                    counterpartPortal = connection.ConnectedPortalA;

                // 2. 이동 가능한 연결인지 검사하기
                if (!connection.PassPortal(portal, counterpartPortal)) continue;

                // 3. 이동 가능하다면 목록에 추가하기
                connections.Add(connection);
            }

            return connections;
        }

        public bool TryGetPortal(Vector2Int gridPos, out Portal portal)
        {
            portal = null;

            foreach (var port in Portals.Values)
            {
                if (port.Node.NodeGridPos == gridPos)
                {
                    portal = port;
                    return true;
                }
            }
            return false;
        }
    }
}