﻿using Phos.Interact;
using UnityEditor;
using UnityEngine;

namespace PhosEditor {
    [CustomEditor(typeof(BaseInteractionControl), true)]
	public class InteractionControlInspector : UnityEditor.Editor {
		private bool foldout = false;
        public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			BaseInteractionControl control = (BaseInteractionControl)target;
            EditorGUILayout.Space(10);
            foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, "Shared Properties");
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (foldout) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Highlight");
                EditorGUILayout.LabelField($"{control._highlight.Value}");
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Segment");
                EditorGUILayout.LabelField($"{control._segment.Value}");
                EditorGUILayout.EndHorizontal();
            }
        }
	}
}