using System;
using System.Collections.Generic;
using Phos.Interact;
using Phos.Navigate;
using UnityEngine;

namespace Phos.Perform {
    public class SceneArchive : MonoBehaviour {
        public SpawnPoint spawnPoint;
        public CameraPoint cameraPoint;
        
        public List<GameObjectStatus> gameObjectStatus;
        public List<InteractionControlStatus> interactionControlStatus;
        public List<NavigatePathStatus> navigatePathStatus;

        private bool _loaded = false;
        private PlayerController _player;

        public void LoadArchive(PlayerController player, MultiLayerCamera camera) {
            foreach (var status in gameObjectStatus) {
                status.Load();
            }
            foreach (var status in interactionControlStatus) {
                status.Load();
            }
            
            spawnPoint.Spawn(player);
            if (camera) {
                camera.transform.position = cameraPoint.transform.position;
                camera.transform.rotation = cameraPoint.transform.rotation;
                camera.SetSize(cameraPoint.size);
            }
        }

        private void Update() {
            if (_loaded) {
                
                _loaded = false;
            }
        }
    }

    [Serializable]
    public class GameObjectStatus {
        public GameObject gameObject;
        public bool enable;
        public Vector3 position;
        public Quaternion rotation;
        
        public void Load() {
            gameObject.SetActive(enable);
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;
        }
    }

    [Serializable]
    public class InteractionControlStatus {
        public BaseInteractionControl control;
        public bool active;
        public int segment;
        
        public void Load() {
            control.active = active;
            control.SetSegment(segment);
        }
    }

    [Serializable]
    public class NavigatePathStatus {
        public NavigateNode nodeA;
        public NavigateNode nodeB;

        public bool active;

        public void Load() {
            var path = nodeA.Paths[nodeB];

            if (path != null) {
                path.active = active;
            }
        }
    }
}