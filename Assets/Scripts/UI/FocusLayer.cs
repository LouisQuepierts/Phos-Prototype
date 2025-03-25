using UnityEngine;

namespace Phos.UI {
    [ExecuteAlways]
    public class FocusLayer : MonoBehaviour {
        private static readonly int Radius = Shader.PropertyToID("_Radius");
        
        public Material material;
        
        [Range(0.0f, 5.0f)]
        public float radius = 0.5f;

        public bool debug;

        private void Update() {
            if (!Application.isPlaying && !debug) return;
            material.SetFloat(Radius, radius);
        }
    }
}