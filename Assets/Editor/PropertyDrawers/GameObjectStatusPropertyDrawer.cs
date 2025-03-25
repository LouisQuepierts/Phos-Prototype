using Phos.Perform;
using UnityEditor;
using UnityEditor.ShaderGraph.Drawing.Inspector;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PhosEditor.PropertyDrawers {
    [CustomPropertyDrawer(typeof(GameObjectStatus))]
    public class GameObjectStatusPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
            if (!property.isExpanded) return;

            var pObj = property.FindPropertyRelative("gameObject");
            var obj = pObj.objectReferenceValue as GameObject;
            EditorGUILayout.PropertyField(pObj);

            if (!obj) {
                return;
            }
            
            var pEnable = property.FindPropertyRelative("enable");
            var pPosition = property.FindPropertyRelative("position");
            var pRotation = property.FindPropertyRelative("rotation");
            
            if (GUILayout.Button("Save")) {
                pEnable.boolValue = obj.activeSelf;
                pPosition.vector3Value = obj.transform.position;
                pRotation.quaternionValue = obj.transform.rotation;
                Debug.Log($"Saved {obj.name}");
            }
            
            EditorGUILayout.PropertyField(pEnable);
            EditorGUILayout.PropertyField(pPosition);
            EditorGUILayout.PropertyField(pRotation);
        }
    }
}