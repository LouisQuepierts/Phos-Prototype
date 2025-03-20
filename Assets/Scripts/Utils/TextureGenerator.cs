#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace Phos.Utils {
    public class TextureGenerator : MonoBehaviour {
        public Material material;
        public int width;
        public int height;
        
        public string fileName;

        public void Generate() {
            if (!material ||
                width <= 0 ||
                height <= 0) return;
            
            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            
            RenderTexture prev = RenderTexture.active;
            RenderTexture rt = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(Texture2D.whiteTexture, rt, material);

            RenderTexture.active = rt;
            texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            texture.Apply();
            
            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);

            string file = Application.dataPath + "\\" + fileName + ".png";
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(file, bytes);
            
            DestroyImmediate(texture);
            Debug.Log("Texture generated: " + file);
        }
    }
}
#endif // UNITY_EDITOR