using System;
using Phos.Navigate;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Phos.Perform {
    public class SceneController : MonoBehaviour {
        public string sceneName;
        
        [Header("Prefabs")]
        public PlayerController player;
        public ClickHighlight clickHighlight;
        
        [Header("Checkpoints")]
        public Checkpoint[] checkpoints;

        public static SceneController Instance { get; private set; }

        private const float MaxRaycastDistance = 64f;

        public PlayerController Player { get; private set; }
        
        private ClickHighlight _clickHighlight;
        private NavigateNode _clickedNode;
        
        private int _layerMask;

        // search in saves
        private int _checkpoint = 0;
        
        private void Start() {
            HandleMouseInput();
            if (_checkpoint >= checkpoints.Length) {
                throw new Exception("No checkpoints found");
            }

            var playerObj = PrefabUtility.InstantiatePrefab(player.gameObject);
            Player = playerObj.GetComponent<PlayerController>();

            if (!Player) {
                throw new Exception("No player found");
            }

            Player.Spawn(checkpoints[_checkpoint].GetPosition());
            
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
            if (Input.GetMouseButtonDown(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (!Physics.Raycast(ray, out RaycastHit hit, MaxRaycastDistance, _layerMask)) return;
                NavigateNode node = hit.collider.GetComponent<NavigateNode>();

                if (!node || node.hide) return;
                
                _clickHighlight?.Click(node);
                Player?.MoveTo(node);
            }
        }
    }
}