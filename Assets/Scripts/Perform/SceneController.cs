using System;
using Phos.Navigate;
using Phos.Utils;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Phos.Perform {
    public class SceneController : MonoBehaviour, IToggleable {
        public string sceneName;
        public bool active = true;
        
        [Header("Prefabs")]
        public PlayerController player;
        public ClickHighlight clickHighlight;
        
        [Header("Checkpoints")]
        public SpawnPoint[] checkpoints;
        
        [Header("Archives")]
        public SceneArchive[] archives;
        public int debugArchive = -1;

        [Header("Scene Ambient")] 
        public Material[] materials;

        public static SceneController Instance { get; private set; }

        private const float MaxRaycastDistance = 64f;

        public PlayerController Player { get; private set; }
        
        private ClickHighlight _clickHighlight;
        private NavigateNode _clickedNode;
        
        private int _layerMask;

        // search in saves
        private int _progress = 0;
        
        private void OnEnable() {
            HandleMouseInput();
            _progress = PlayerPrefs.GetInt($"{sceneName}.progress", 0);

            #if UNITY_EDITOR
            if (debugArchive > -1 && debugArchive < checkpoints.Length) {
                _progress = debugArchive;
            }
            #endif
            
            if (_progress >= checkpoints.Length) {
                throw new Exception("No checkpoints found");
            }

            var playerObj = PrefabUtility.InstantiatePrefab(player.gameObject);
            Player = playerObj.GetComponent<PlayerController>();

            if (!Player) {
                throw new Exception("No player found");
            }

            archives[_progress].LoadArchive(Player);
            
            var clickHighlightObj = PrefabUtility.InstantiatePrefab(clickHighlight.gameObject);
            _clickHighlight = clickHighlightObj.GetComponent<ClickHighlight>();

            Instance = this;
            _layerMask = 0 | 1 << LayerMask.NameToLayer("Node");
        }

        private void OnDestroy() {
            Instance = null;
        }

        private void Update() {
            HandleMouseInput();
        }
        
        private void HandleMouseInput() {
            if (!active) return;
            
            if (Input.GetMouseButtonDown(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (!Physics.Raycast(ray, out RaycastHit hit, MaxRaycastDistance, _layerMask)) return;
                NavigateNode node = hit.collider.GetComponent<NavigateNode>();

                if (!node || node.hide) return;
                
                _clickHighlight?.Click(node);
                Player?.MoveTo(node);
            }
        }

        public void Toggle(bool enable) {
            active = enable;
        }

        public void RecordArchive(int archiveID) {
            if (archiveID >= archives.Length || archiveID < 0) {
                Debug.LogWarning($"Invalid archive ID {archiveID}");
                return;
            }
            _progress = archiveID;
            PlayerPrefs.SetInt($"{sceneName}.progress", _progress);
        }
    }
}