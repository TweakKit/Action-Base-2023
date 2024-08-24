#if UNITY_EDITOR

using Runtime.Definition;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Runtime.Common.UI
{
    [CustomEditor(typeof(PerfectButton))]
    public class PerfectButtonEditor : ButtonEditor
    {
        private PerfectButton _target;
        protected override void OnEnable()
        {
            base.OnEnable();
            _target = (PerfectButton)target;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            _target.buttomSourceType = (ButtonSourceType)EditorGUILayout.EnumPopup("Button Source Type", _target.buttomSourceType);
            _target.buttonType = (ButtonType)EditorGUILayout.EnumPopup("ButtonType", _target.buttonType);
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_target);
            }
        }
    }
}

#endif