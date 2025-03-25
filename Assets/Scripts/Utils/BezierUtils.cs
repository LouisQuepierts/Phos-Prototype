using UnityEngine;

namespace Phos.Utils {
    public static class BezierUtils {
        public static Vector3 CalculatePoint(Vector3 start, Vector3 end, float t) {
            return Vector3.Lerp(start, end, t);
        }

        public static Vector3 CalculatePoint(Vector3 a, Vector3 b, Vector3 c, float t) {
            return Vector3.Lerp(Vector3.Lerp(a, b, t), Vector3.Lerp(b, c, t), t);
        }
        
        public static Vector3 CalculatePoint(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t) {
            return Vector3.Lerp(Vector3.Lerp(Vector3.Lerp(a, b, t), Vector3.Lerp(b, c, t), t), Vector3.Lerp(Vector3.Lerp(b, c, t), Vector3.Lerp(c, d, t), t), t);
        }
        
        public static Vector2 CalculatePoint(Vector2 start, Vector2 end, float t) {
            return Vector2.Lerp(start, end, t);
        }
        
        public static Vector2 CalculatePoint(Vector2 a, Vector2 b, Vector2 c, float t) {
            return Vector2.Lerp(Vector2.Lerp(a, b, t), Vector2.Lerp(b, c, t), t);
        }
        
        public static Vector2 CalculatePoint(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t) {
            return Vector2.Lerp(Vector2.Lerp(Vector2.Lerp(a, b, t), Vector2.Lerp(b, c, t), t), Vector2.Lerp(Vector2.Lerp(b, c, t), Vector2.Lerp(c, d, t), t), t);
        }
    }
}