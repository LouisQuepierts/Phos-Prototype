using Phos.Predicate;
using Phos.Utils;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace PhosEditor.PropertyDrawers {
    [CustomPropertyDrawer(typeof(BasePredicate))]
	public class BasePredicatePropertyDrawer : PropertyDrawer {
        private static readonly SingletonFactory<PredicateType, System.Type> _factory;
        private PredicateType selected;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginChangeCheck();
            
            EditorGUI.LabelField(position, label);
            EditorGUILayout.PropertyField(property, new GUIContent("Predicate"));

            if (property.objectReferenceValue == null) {

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Create Predicate", EditorStyles.boldLabel);
                selected = (PredicateType)EditorGUILayout.EnumPopup("Type", selected);

                if (GUILayout.Button("Create")) {
                    Object target = property.serializedObject.targetObject;
                    if (target is MonoBehaviour @object) {
                        var instance = @object.gameObject.AddComponent(_factory.GetInstance(selected));
                        property.objectReferenceInstanceIDValue = instance.GetInstanceID();
                    }
                    //property.objectReferenceValue = (Object)PredicateHolder.Factory.GetInstance(selected);
                }
            }

            if (EditorGUI.EndChangeCheck()) {
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        private static GameObject CreateInstance(PredicateType type) {
            GameObject instance = new GameObject(type.ToString() + " Predicate");
            instance.AddComponent(_factory.GetInstance(type));
            return instance;
        }

        static BasePredicatePropertyDrawer() {
            _factory = new SingletonFactory<PredicateType, System.Type>(typeof(TogglePredicate), System.Enum.GetValues(typeof(PredicateType)).Length);
            _factory.Register(PredicateType.Toggle, typeof(TogglePredicate));
            _factory.Register(PredicateType.Structure, typeof(StructurePredicate));
            _factory.Register(PredicateType.Interact, typeof(InteractionPredicate));
            _factory.Register(PredicateType.Position, typeof(TogglePredicate));
            _factory.Register(PredicateType.Auto, typeof(AutoCheckPredicate));
        }
    }
}