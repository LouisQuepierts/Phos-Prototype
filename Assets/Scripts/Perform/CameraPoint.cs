using System;
using UnityEngine;

namespace Phos.Perform {
    [ExecuteAlways]
    public class CameraPoint : MonoBehaviour {
        public float size;
        
        public bool debug;
        public Camera debugCamera;

        public void Update() {
            if (debug && !Application.isPlaying) {
                debugCamera.transform.position = transform.position;
                debugCamera.orthographicSize = size;
            }
        }
    }
}