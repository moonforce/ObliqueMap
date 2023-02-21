using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Battlehub.Utils;

namespace Battlehub.UIControls.MenuControl
{
    [CustomEditor(typeof(Menu))]
    public class MenuEditor : Editor
    {
        private ReorderableList list;

        private string m_pathName = Strong.MemberInfo((MenuItemInfo mi) => mi.Path).Name;
      
        private void OnEnable()
        {
            list = new ReorderableList(serializedObject,
                    serializedObject.FindProperty("m_items"),
                    true, true, true, true);

            list.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Items Order");
            };

            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);

                var path = element.FindPropertyRelative(m_pathName);
                EditorGUI.LabelField(rect, path.stringValue);
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();   
        }
    }
}

