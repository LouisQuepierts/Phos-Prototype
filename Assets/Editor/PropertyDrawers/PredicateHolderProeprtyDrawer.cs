using Phos.Predicate;
using UnityEditor;
using UnityEngine;

namespace PhosEditor.PropertyDrawers {
    [CustomPropertyDrawer(typeof(PredicateHolder))]
	public class PredicateHolderPropertyDrawer : PropertyDrawer {
        private bool _showInstance;

        public PredicateHolderPropertyDrawer() {
            Debug.Log("New Drawer");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            EditorGUI.BeginChangeCheck();

            SerializedProperty typeProp = property.FindPropertyRelative("type");
            SerializedProperty instance = property.FindPropertyRelative("instance");

            EditorGUI.LabelField(position, label);
            int selected = typeProp.intValue;
            int selection = EditorGUILayout.Popup(typeProp.displayName, typeProp.intValue, typeProp.enumDisplayNames);

            PredicateType type = (PredicateType) selection;
            if (selected != selection) {
                instance.managedReferenceValue = PredicateHolder.Factory.GetInstance(type);
                Debug.Log(instance.managedReferenceFullTypename);
                typeProp.intValue = selection;
            }

            _showInstance = EditorGUILayout.Foldout(_showInstance, "Instance");

            if (_showInstance 
                && instance != null
                && instance.managedReferenceValue != null) {
                DrawChildProperties(instance);
            }

            if (EditorGUI.EndChangeCheck()) {
                Debug.Log("apply");
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawChildProperties(SerializedProperty property) {
            property.isExpanded = true;
            SerializedProperty iterator = property.Copy();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren)) {
                if (iterator.depth == 0 && iterator.name == "m_Script") continue;

                enterChildren = false;
                EditorGUILayout.PropertyField(
                    iterator,
                    new GUIContent("Field")
                );
            }
        }
    }
}