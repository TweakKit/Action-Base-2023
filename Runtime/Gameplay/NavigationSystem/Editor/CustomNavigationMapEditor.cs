#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Runtime.Navigation
{
    [CustomEditor(typeof(CustomNavigationMap))]
    public class CustomNavigationMapEditor : Editor
    {
        #region Properties

        private CustomNavigationMap NavigationMap
        {
            get { return target as CustomNavigationMap; }
        }

        #endregion Properties

        #region Class Methods

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUI.changed)
                EditorApplication.delayCall += CheckChangeType;

            if (Application.isPlaying)
                EditorGUILayout.LabelField("Nodes Count", NavigationMap.NodesCount.ToString());
        }

        private void CheckChangeType()
        {
            var collider = NavigationMap.GetComponent<Collider2D>();
            var rigidbody = NavigationMap.GetComponent<Rigidbody2D>();
            if (NavigationMap.shapeType == CustomNavigationShapeType.Polygon && !(collider is PolygonCollider2D))
            {
                if (collider != null)
                    Undo.DestroyObjectImmediate(collider);
                if (rigidbody != null)
                    Undo.DestroyObjectImmediate(NavigationMap.GetComponent<Rigidbody2D>());

                var polygonCollider = NavigationMap.gameObject.AddComponent<PolygonCollider2D>();
                Undo.RegisterCreatedObjectUndo(polygonCollider, "Change Shape Type");
            }
            else if (NavigationMap.shapeType == CustomNavigationShapeType.Box && !(collider is BoxCollider2D))
            {
                if (collider != null)
                    Undo.DestroyObjectImmediate(collider);
                if (rigidbody != null)
                    Undo.DestroyObjectImmediate(NavigationMap.GetComponent<Rigidbody2D>());

                var boxCollider = NavigationMap.gameObject.AddComponent<BoxCollider2D>();
                Undo.RegisterCreatedObjectUndo(boxCollider, "Change Shape Type");
            }
        }

        #endregion Class Methods
    }
}

#endif