using System.Collections.Generic;
using DevLib.TileAstar;
using NUnit.Framework;
using UnityEngine;

namespace NKY.Script.AStar
{
    public class AStarPathfinder
    {
        public IGridProvider Grid { get; private set; }


        public AStarPathfinder(IGridProvider grid)
        {
            Grid = grid;
        }

        public List<INode> FindPath(Vector2Int start, Vector2Int goal)
        {
            List<INode> path = new List<INode>(); //반환할 리스트 생성 (null을 반환하지 않는 이유는 받는곳에서 따로 null검사하기 귀찮아서)
            
            //시작 노드, 종점 노드 찾고 갈 수 있는지 검사
            INode startNode = Grid.GetNodeByGrid(start);
            INode goalNode = Grid.GetNodeByGrid(goal);
            if(startNode == null || goalNode == null)
                return path;
            if (!startNode.CanMove || !goalNode.CanMove)
                return path;
            
            //노드에 대한 코스트를 관리하는 PathNodeData 딕셔너리 생성
            Dictionary<INode, PathNodeData> searchData = new();
            
            //시작 노드 비용 데이터 만들어서 넣기
            float startHCost = PathNodeData.GetHCost(start, goal);
            PathNodeData startNodeData = new PathNodeData(0, startHCost, null); // g값은 0으로 시작, parentNode는 이곳이 시작점이니 null
            searchData.Add(startNode, startNodeData);
            
            //이동가능한 노드를 담을 리스트(우선순위 큐) 생성 후 시작 노드 넣기 (따로 AStarNodeEntry를 만든 이유는 길찾을때 사용하는 데이터를 나누기 위해)
            PriorityQueue<AStarNodeEntry>  openList = new();
            AStarNodeEntry startNodeEntry = new AStarNodeEntry(startNode, 0, startHCost);
            openList.Push(startNodeEntry);
            
            //이미 다녀갔던 노드들을 담아서 중복으로 검색하지 않도록 하는 closedList
            HashSet<INode> closedList = new();
            
            while (openList.Count > 0)
            {
                // 1. FCost가 가장 낮은 노드 꺼내기
                AStarNodeEntry rowEntry = openList.Pop();
                INode rowNode = rowEntry.Node;
                if(rowEntry.GCost > searchData[rowNode].GCost)
                    continue;
                
                if (!closedList.Add(rowNode))
                    continue;

                // 2. 목표 노드인지 검사
                if (rowNode.NodeGridPos == goalNode.NodeGridPos)
                {
                    INode current = rowNode;

                    while (current != null)
                    {
                        path.Add(current);
                        current = searchData[current].ParentNode;
                    }

                    path.Reverse();
                    break;
                }
                // 3. 현재 노드의 이웃 탐색
                List<INode> neighbors = Grid.GetNeighbors(rowNode.NodeGridPos);
                foreach (var neighbor in neighbors)
                {             
                    if (closedList.Contains(neighbor))
                        continue;
                    
                    float stepCost = Vector2Int.Distance(rowNode.NodeGridPos, neighbor.NodeGridPos) * neighbor.MoveCost;
                    float newGCost = searchData[rowNode].GCost + stepCost;

                    float hCost = PathNodeData.GetHCost(neighbor.NodeGridPos, goal);
                    PathNodeData neighborData;

                    if (searchData.TryGetValue(neighbor, out PathNodeData neighborPath))
                    {
                        neighborData = neighborPath;
                    }
                    else
                    {
                        neighborData = new PathNodeData(float.PositiveInfinity, hCost, null);
                        searchData.Add(neighbor, neighborData);
                    }


                    if (newGCost < neighborData.GCost)
                    {
                        neighborData.SetPathData(newGCost, rowNode);
                        AStarNodeEntry neighborEntry = new AStarNodeEntry(neighbor, newGCost, newGCost + hCost);
                        openList.Push(neighborEntry);
                    }
                    
                }
            }
            return path;
        }
    }
}