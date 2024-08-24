using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Navigation
{
    public static class AStar
    {
        #region Class Methods

        public static void CalculatePath(PathNode startNode, PathNode endNode, List<PathNode> allNodes, Action<List<Vector2>> callback)
        {
            var openList = new Heap<PathNode>(allNodes.Count);
            var closedList = new HashSet<PathNode>();
            var success = false;

            openList.Add(startNode);

            while (openList.Count > 0)
            {
                var currentNode = openList.RemoveFirst();
                if (currentNode == endNode)
                {
                    success = true;
                    break;
                }

                closedList.Add(currentNode);
                var linkIndeces = currentNode.links;
                for (var i = 0; i < linkIndeces.Count; i++)
                {
                    var neighbour = allNodes[linkIndeces[i]];
                    if (closedList.Contains(neighbour))
                        continue;

                    var costToNeighbour = currentNode.gCost + (currentNode.position - neighbour.position).magnitude;
                    if (costToNeighbour < neighbour.gCost || !openList.Contains(neighbour))
                    {
                        neighbour.gCost = costToNeighbour;
                        neighbour.hCost = (neighbour.position - endNode.position).magnitude;
                        neighbour.parent = currentNode;

                        if (!openList.Contains(neighbour))
                            openList.Add(neighbour);
                    }
                }
            }

            if (success)
            {
                var path = new List<Vector2>();
                var currentNode = endNode;
                while (currentNode != startNode)
                {
                    path.Add(currentNode.position);
                    currentNode = currentNode.parent;
                }
                path.Add(startNode.position);
                path.Reverse();
                callback(path);
            }
            else callback(null);
        }

        #endregion Class Methods
    }
}