
using UnityEditor;
using UnityEngine;

namespace PhosEditor {
    public class LevelEditorWindow : EditorWindow {
        public bool value;

        [MenuItem("Tool/LevelEditor/Show")]
        static void ShowWindow() {
            var window = EditorWindow.GetWindow<LevelEditorWindow>();
            window.Show();
        }

        private void OnGUI() {
            GUILayout.Label("Content");
        }
    }
}