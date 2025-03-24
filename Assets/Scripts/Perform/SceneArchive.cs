using System;
using System.Collections.Generic;
using Phos.Interact;
using UnityEngine;

namespace Phos.Perform {
    public class SceneArchive : MonoBehaviour {
        public SpawnPoint spawnPoint;
        public CameraPoint cameraPoint;
        
        public List<GameObjectStatus> gameObjectStatus;
        public List<InteractionControlStatus> interactionControlStatus;

        public void LoadArchive(PlayerController player) {
            foreach (var status in gameObjectStatus) {
                status.Load();
            }
            foreach (var status in interactionControlStatus) {
                status.Load();
            }
            
            player.Spawn(spawnPoint.GetPosition());
            var camera = Camera.main;
            if (camera) {
                camera.transform.position = cameraPoint.transform.position;
                camera.transform.rotation = cameraPoint.transform.rotation;
                camera.orthographicSize = cameraPoint.size;
            }
        }
    }

    [Serializable]
    public class GameObjectStatus {
        public GameObject gameObject;
        public bool enable;
        public Vector3 position;
        public Quaternion rotation;
        
        public void Save() {
            enable = gameObject.activeSelf;
            position = gameObject.transform.position;
            rotation = gameObject.transform.rotation;
        }
        
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
        
        public void Save() {
            active = control.active;
            segment = Mathf.RoundToInt(control.Segment);
        }
        
        public void Load() {
            control.active = active;
            control.SetSegment(segment);
        }
    }
}