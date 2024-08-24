#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Runtime.UI
{
    [UnityEditor.CustomEditor(typeof(PinchableScrollRect))]
    public class PinchableScrollRectEditor : ScrollRectEditor
    {
        #region Class Methods

        public override void OnInspectorGUI()
        {
            PinchableScrollRect pinchableScrollRect = target as PinchableScrollRect;
            if (pinchableScrollRect.GetComponent<PinchInputDetector>() == null)
            {
                EditorGUILayout.HelpBox("PinchInputDetector script is not attached. Pinching movement will not be detected.", UnityEditor.MessageType.Warning);
                if (GUILayout.Button("Add PinchInputDetector"))
                    Undo.AddComponent<PinchInputDetector>(pinchableScrollRect.gameObject);
            }
            base.OnInspectorGUI();
            var flag = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            foreach (var field in typeof(PinchableScrollRect).GetFields(flag))
            {
                var property = serializedObject.FindProperty(field.Name);
                if (property != null)
                    EditorGUILayout.PropertyField(property);
            }
            serializedObject.ApplyModifiedProperties();
        }

        #endregion Class Methods
    }
}

#endif