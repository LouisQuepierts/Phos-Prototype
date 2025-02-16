using Phos.Navigate;
using UnityEditor;
using UnityEngine;

namespace PhosEditor {
    [CustomEditor(typeof(CurveHelper))]
	public class CurveHelperInspector : Editor {
		public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            CurveHelper curveHelper = (CurveHelper)target;
            GUILayout.Label("Curve Helper", EditorStyles.boldLabel);
            if (GUILayout.Button("Refresh")) {
                curveHelper.Refresh();
            }

            if (GUILayout.Button("Generate")) {
                curveHelper.Generate(LevelEditor.NodeTemplate);
            }
        }
	}
}