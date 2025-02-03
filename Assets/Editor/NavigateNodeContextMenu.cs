using UnityEngine;
using UnityEditor;
using System;
using Phos.Navigate;

namespace PhosEditor {
    public class NavigateNodeContextMenu : ScriptableObject {
        [MenuItem("CONTEXT/NavigateNode/Connect _#c", true)]
        public static bool ValidateConnect() {
            return Validate();
        }

        [MenuItem("CONTEXT/NavigateNode/Disconnect _#d", true)]
        public static bool ValidateDisconnect() {
            return Validate();
        }

        [MenuItem("CONTEXT/NavigateNode/Create Router _#n", true)]
        public static bool ValidateCreateRouter() {
            return LevelEditor.HighlightPath != null;
        }

        [MenuItem("CONTEXT/NavigateNode/Reset Connections _#r", true)]
        public static bool ValidateDisconnectAll() {
            GameObject[] selected = Selection.gameObjects;

            foreach (var item in selected) {
                if (item != null && item.GetComponent<NavigateNode>() != null) return true;
            }
            return false;
        }

        [MenuItem("CONTEXT/NavigateNode/Connect _#c", false)]
        public static void ConnectNodes() {
            NavigateNodeOperations.ConnectSelectedNodes();
        }

        [MenuItem("CONTEXT/NavigateNode/Disconnect _#d", false)]
        public static void Disconnect() {
            NavigateNodeOperations.DisconnectSelectedNodes();
        }

        [MenuItem("CONTEXT/NavigateNode/Reset Connections _#r", false)]
        static void ResetConnections() {
            NavigateNodeOperations.ResetSelectedNodes();
        }

        [MenuItem("CONTEXT/NavigateNode/Create Router _#n", false)]
        static void CreateRouter() {
            NavigateNodeOperations.CreateVirtualNode();
        }

        [MenuItem("CONTEXT/NavigateNode/Toggle Editor _#e", false)]
        static void ToggleEditor() {
            LevelEditor.Toggle();
        }

        static bool Validate(int amount = 2) {
            GameObject[] selected = Selection.gameObjects;

            if (selected != null
                && selected.Length == amount) {

                int index = Array.FindIndex<GameObject>(selected, item => item.GetComponent<NavigateNode>() == null);
                return index == -1;
            }

            return false;
        }
    }
}