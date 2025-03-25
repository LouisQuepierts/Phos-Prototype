using UnityEngine;

namespace Phos.UI {
    [ExecuteAlways]
    public class RingButton : MonoBehaviour {
        private static readonly int Radius = Shader.PropertyToID("_Radius");
        
        public float innerRadius;
        public float outerRadius;

        public Material innerMaterial;
        public Material outerMaterial;
        
        public void Update() {
            innerMaterial?.SetFloat(Radius, innerRadius);
            outerMaterial?.SetFloat(Radius, outerRadius);
        }
    }
}