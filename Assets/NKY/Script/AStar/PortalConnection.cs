using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NKY.Script.AStar
{
    public class PortalConnection
    {
        public int ConnectionId { get; }

        public Portal ConnectedPortalA { get; }
        public Portal ConnectedPortalB { get; }

        public bool IsBidirectional { get; }
        public bool IsPassable { get; set; }
        
        public PortalPathData PathAToB { get; private set; }
        public PortalPathData PathBToA { get; private set; }
        
        public PortalConnection(int connectionId, Portal a, Portal b, bool isBidirectional, bool isPassable)
        {
            if (a == null || b == null)
                throw new ArgumentNullException();
            if (a.PortalId == b.PortalId)
                throw new ArgumentException("A and B are not connected");
            
            ConnectionId = connectionId;
            
            ConnectedPortalA = a;
            ConnectedPortalB = b;
            IsBidirectional = isBidirectional;
            IsPassable = isPassable;
        }

        public bool PassPortal(Portal start, Portal end)
        {
            if(start == null || end == null) return false;
            if(!IsPassable) return false;
            if(start.PortalId == end.PortalId) return false;

            if (start.PortalId == ConnectedPortalA.PortalId &&
                end.PortalId == ConnectedPortalB.PortalId)
            {
                return true;
            }

            if (start.PortalId == ConnectedPortalB.PortalId &&
                end.PortalId == ConnectedPortalA.PortalId)
            {
                return IsBidirectional;
            }
            return false;
        }
        
        public bool Connects(Portal a, Portal b)
        {
            return
                (ConnectedPortalA == a && ConnectedPortalB == b) ||
                (ConnectedPortalA == b && ConnectedPortalB == a);
        }

        public bool HavePortal(Portal portal)
        {
            return  ConnectedPortalA == portal || ConnectedPortalB == portal;
        }
    }
}