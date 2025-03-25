using System;
using UnityEngine;

namespace Phos.Perform {
    [ExecuteAlways]
    public class SceneAmbient : MonoBehaviour {
        private static readonly int ScreenHigherAmbient = Shader.PropertyToID("_ScreenHigherAmbient");
        private static readonly int ScreenLowerAmbient = Shader.PropertyToID("_ScreenLowerAmbient");
        private static readonly int ScreenOffset = Shader.PropertyToID("_ScreenOffset");
        private static readonly int ScreenFactor = Shader.PropertyToID("_ScreenFactor");
        
        public Color sceneHigherColor;
        public Color sceneLowerColor;
        [Range(-1f, 1f)]
        public float sceneGradientOffset = 0.5f;
        [Range(0f, 2f)]
        public float sceneGradientFactor = 0.5f;

        private void Update() {
            Shader.SetGlobalColor(ScreenHigherAmbient, sceneHigherColor);
            Shader.SetGlobalColor(ScreenLowerAmbient, sceneLowerColor);
            Shader.SetGlobalFloat(ScreenOffset, sceneGradientOffset);
            Shader.SetGlobalFloat(ScreenFactor, sceneGradientFactor);
        }
    }
}