using System;
using System.Collections.Generic;
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

        public MultiLayerCamera sceneCamera;

        public static SceneController Instance { get; private set; }

        private const float MaxRaycastDistance = 64f;

        public PlayerController Player { get; private set; }
        
        private ClickHighlight _clickHighlight;
        private NavigateNode _clickedNode;

        private CameraPoint _cameraFollow;
        
        private int _layerMask;

        // search in saves
        private int _progress = 0;
        
        private Queue<Action> _actions = new Queue<Action>();

        public SceneController() {
            Instance = this;
        }
        
        private void Awake() {
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
            
            var clickHighlightObj = PrefabUtility.InstantiatePrefab(clickHighlight.gameObject);
            _clickHighlight = clickHighlightObj.GetComponent<ClickHighlight>();

            _layerMask = 0 | 1 << LayerMask.NameToLayer("Node");
        }

        private void Start() {
            LoadArchive();
        }

        private void LoadArchive() {
            Debug.Log($"Loading archive {_progress}");
            archives[_progress].LoadArchive(Player, sceneCamera);
        }

        private void OnDestroy() {
            Instance = null;
        }

        private void Update() {
            HandleMouseInput();

            if (_cameraFollow) {
                sceneCamera.transform.position = _cameraFollow.transform.position;
                
                if (sceneCamera.size != _cameraFollow.size)
                    sceneCamera.SetSize(_cameraFollow.size);
            }
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

        public void CameraFollow(CameraPoint point) {
            _cameraFollow = point;
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

        public void AddAction(Action action) {
            _actions.Enqueue(action);
        }

        public bool NextAction() {
            if (_actions.Count == 0) {
                return false;
            }
            
            _actions.Dequeue().Invoke();
            return true;
        }

        public void ClearActions() {
            _actions.Clear();
        }
    }
}