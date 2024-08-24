using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Navigation
{
    public class PathNode : IHeapItem<PathNode>
    {
        #region Members

        public Vector2 position;
        public List<int> links;
        public float gCost;
        public float hCost;
        public PathNode parent;

        #endregion Members

        #region Properties

        public float FCost
        {
            get
            {
                return gCost + hCost;
            }
        }

        int IHeapItem<PathNode>.HeapIndex { get; set; }

        #endregion Properties

        #region Struct Methods

        public PathNode(Vector2 position)
        {
            this.position = position;
            this.links = new List<int>();
            this.gCost = 1f;
            this.hCost = 0f;
            this.parent = null;
        }

        int IComparable<PathNode>.CompareTo(PathNode other)
        {
            int compare = FCost.CompareTo(other.FCost);
            if (compare == 0)
                compare = hCost.CompareTo(other.hCost);
            return -compare;
        }

        #endregion Struct Methods
    }
}