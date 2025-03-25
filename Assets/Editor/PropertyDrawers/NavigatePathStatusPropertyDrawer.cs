using Phos.Interact;
using Phos.Navigate;
using Phos.Perform;
using UnityEditor;
using UnityEngine;

namespace PhosEditor.PropertyDrawers {
    [CustomPropertyDrawer(typeof(NavigatePathStatus))]
    public class NavigatePathStatusPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
            if (!property.isExpanded) return;

            var pNodeA = property.FindPropertyRelative("nodeA");
            var pNodeB = property.FindPropertyRelative("nodeB");
            
            var nodeA = pNodeA.objectReferenceValue as NavigateNode;
            var nodeB = pNodeB.objectReferenceValue as NavigateNode;
            
            EditorGUILayout.PropertyField(pNodeA);
            EditorGUILayout.PropertyField(pNodeB);

            if (!nodeA || !nodeB) {
                return;
            }
            
            var pActive = property.FindPropertyRelative("active");
            
            if (GUILayout.Button("Save")) {
                var path = nodeA.Paths[nodeB];
                if (path == null) {
                    Debug.Log($"No path between {nodeA.transform.position} and {nodeB.transform.position}");
                }
                else {
                    pActive.boolValue = path.active;
                    Debug.Log($"Saved {path.active}");
                }
            }

            EditorGUILayout.PropertyField(pActive);
        }
    }
}