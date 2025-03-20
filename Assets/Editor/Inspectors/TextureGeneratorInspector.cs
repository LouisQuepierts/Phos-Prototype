using Phos.Utils;
using UnityEditor;
using UnityEngine;

namespace PhosEditor {
    [CustomEditor(typeof(TextureGenerator))]
    public class TextureGeneratorInspector : Editor {
        // ReSharper disable Unity.PerformanceCriticalCodeInvocation
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            TextureGenerator generator = (TextureGenerator)target;
            
            if (GUILayout.Button("Generate")) {
                generator.Generate();
            }
        }
    }
}