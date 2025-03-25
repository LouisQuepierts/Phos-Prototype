using Phos.Interact;
using Phos.Perform;
using UnityEditor;
using UnityEngine;

namespace PhosEditor.PropertyDrawers {
    [CustomPropertyDrawer(typeof(InteractionControlStatus))]
    public class InteractionControlStatusPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
            if (!property.isExpanded) return;

            var pObj = property.FindPropertyRelative("control");
            var ctrl = pObj.objectReferenceValue as BaseInteractionControl;
            EditorGUILayout.PropertyField(pObj);

            if (!ctrl) {
                return;
            }
            
            var pActive = property.FindPropertyRelative("active");
            var pSegment = property.FindPropertyRelative("segment");
            
            if (GUILayout.Button("Save")) {
                pActive.boolValue = ctrl.active;
                pSegment.intValue = Mathf.RoundToInt(ctrl.Segment);
                Debug.Log($"Saved {ctrl.name}");
            }

            EditorGUILayout.PropertyField(pActive);
            EditorGUILayout.PropertyField(pSegment);
        }
    }
}