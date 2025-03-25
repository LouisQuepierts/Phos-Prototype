using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Phos.UI {
    public class MenuManager : MonoBehaviour {
        public static MenuManager Instance { get; private set; }
        public List<MenuEntry> menus;

        private GameObject _canvas;
        private Dictionary<string, MenuController> _menus;

        public MenuManager() {
            Instance = this;
        }
        private void Awake() {
            _canvas = GameObject.FindWithTag("MenuOverlay");
            if (_canvas == null) {
                Debug.LogError("No canvas found");
                enabled = false;
                return;
            }
            _menus = new Dictionary<string, MenuController>();
            foreach (var entry in menus.Where(entry => entry.menu != null && !string.IsNullOrEmpty(entry.name))) {
                if (_menus.ContainsKey(entry.name)) {
                    Debug.LogError("Duplicate menu name: " + entry.name);
                    continue;
                }
                _menus.Add(entry.name, entry.menu);
            }
        }

        private void OnDisable() {
            Instance = null;
        }

        public void ToggleMenu(string menuName, bool enable, float speed) {
            if (!_menus.TryGetValue(menuName, out var menu)) return;
            
            if (PrefabUtility.IsPartOfPrefabAsset(menu)) {
                Debug.Log($"Menu {menuName} is part of prefab, instantiating...");
                var instance = PrefabUtility.InstantiatePrefab(menu.gameObject, _canvas.transform);
                menu = instance.GetComponent<MenuController>();
                menu.gameObject.SetActive(true);
                _menus[menuName] = menu;
            }
                
            menu.Toggle(enable, speed);
        }

        [Serializable]
        public class MenuEntry {
            public string name;
            public MenuController menu;
        }
    }
}