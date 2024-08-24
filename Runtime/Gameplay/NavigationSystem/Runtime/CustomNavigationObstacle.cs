using System;
using Runtime.Gameplay.Map;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Navigation
{
    [DisallowMultipleComponent]
    public class CustomNavigationObstacle : MonoBehaviour
    {
        #region Members

        public bool isFog;
        [ReadOnly]
        public bool isDisabled;
        private PolygonCollider2D _collider;

        #endregion Members

        #region Properties

        private PolygonCollider2D Collider
        {
            get
            {
                return _collider != null ? _collider : _collider = GetComponent<PolygonCollider2D>();
            }
        }

        #endregion Properties

        #region API Methods

        private void Awake()
            => transform.hasChanged = false;

        private void Reset()
        {
            if (Collider == null)
                gameObject.AddComponent<PolygonCollider2D>();
            Collider.isTrigger = true;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            isFog = gameObject.GetComponent<MapFog>() != null;
        }
#endif

        #endregion API Methods

        #region Class Methods

        public void DisableObstacle()
            => isDisabled = true;

        public void EnableObstacle()
            => isDisabled = false;

        public int GetPathCount()
            => Collider.pathCount;

        public Vector2[] GetPathPoints(int index)
        {
            var points = Collider.GetPath(index);
            var isClockwiseWindingOrder = IsClockwiseWindingOrder();
            if (!isClockwiseWindingOrder && points != null)
                Array.Reverse(points);
            return points;
        }

        private bool IsClockwiseWindingOrder()
        {
            var vertices = Collider.points;
            var sum = 0.0f;
            for (int i = 0; i < vertices.Length; i++)
            {
                var currentVertice = vertices[i];
                var nextVertice = vertices[(i + 1) % vertices.Length];
                sum += (nextVertice.x - currentVertice.x) * (nextVertice.y + currentVertice.y);
            }
            return sum > 0.0f;
        }

        #endregion Class Methods
    }
}