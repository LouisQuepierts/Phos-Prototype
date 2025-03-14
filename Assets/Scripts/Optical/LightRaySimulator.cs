using UnityEngine;

namespace Phos.Optical {
    // DeepSeek generated code
    [ExecuteInEditMode]
    public class LightRaySimulator : MonoBehaviour {
        public const float MaxIntensity = 10.0f;
        
        [Header("光线参数")]
        [Range(0.0f, 10.0f)] public float initialIntensity = 1.0f;    // 初始光强 I0
        [Range(0.0f, 1.0f)] public float absorptionCoefficient = 0.1f; // 吸收系数 α
        [Range(0.0f, 1.0f)] public float scatteringCoefficient = 0.05f; // 散射系数 β
        [Range(0.0f, 1.0f)] public float intensityThreshold = 0.01f; // 强度阈值 I_min

        [Header("Gizmos 设置")]
        public Color gizmoColor = Color.red;     // 光线颜色
        public float gizmoSphereRadius = 0.1f;   // 起点/终点标记大小

        // 计算最大传播距离
        public float CalculateMaxDistance()
        {
            float totalAttenuation = absorptionCoefficient + scatteringCoefficient;
        
            // 避免除以零或无效值
            if (totalAttenuation <= 0 || initialIntensity <= intensityThreshold)
            {
                return 0f;
            }

            // 计算距离 x = -ln(I_min/I0) / (α + β)
            float distance = -Mathf.Log(intensityThreshold / initialIntensity) / totalAttenuation;
            return distance;
        }
        
        public float IntensityToAlpha(float currentIntensity)
        {
            // 归一化到 [0, 1]
            float alpha = currentIntensity / MaxIntensity;

            // 阈值处理：低于阈值时完全透明
            if (currentIntensity < intensityThreshold)
            {
                alpha = 0f;
            }

            return Mathf.Clamp01(alpha); // 确保 Alpha 在 [0, 1]
        }
        
        public float CalculateIntensityAtDistance(float x)
        {
            float totalAttenuation = absorptionCoefficient + scatteringCoefficient;
            float intensity = initialIntensity * Mathf.Exp(-totalAttenuation * x);
            return Mathf.Max(intensity, 0f); // 确保非负
        }

        // 在 Scene 视图中绘制 Gizmos
        private void OnDrawGizmos()
        {
            Vector3 startPoint = transform.position;
            Vector3 direction = transform.forward;

            // 计算最大传播距离
            float maxDistance = CalculateMaxDistance();
            Vector3 endPoint = startPoint + direction * maxDistance;

            // 分段绘制光线（示例：每 1 米一段）
            int segments = Mathf.CeilToInt(maxDistance);
            for (int i = 0; i < segments; i++)
            {
                float xStart = i * 1.0f;
                float xEnd = (i + 1) * 1.0f;
                xEnd = Mathf.Min(xEnd, maxDistance);

                // 计算起始点和结束点的光强
                float iStart = CalculateIntensityAtDistance(xStart);
                float iEnd = CalculateIntensityAtDistance(xEnd);

                // 映射到 Alpha
                float alphaStart = IntensityToAlpha(iStart);
                float alphaEnd = IntensityToAlpha(iEnd);

                // 设置颜色（固定 RGB，动态 Alpha）
                Color colorStart = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, alphaStart);
                Color colorEnd = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, alphaEnd);

                // 绘制线段（需自定义渐变绘制方法）
                DrawGradientLine(startPoint + direction * xStart, startPoint + direction * xEnd, colorStart, colorEnd);
            }

            // 标记起点和终点
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(startPoint, gizmoSphereRadius);
            Gizmos.DrawSphere(endPoint, gizmoSphereRadius);
        }

// 自定义渐变线段绘制（需在 Editor 脚本中实现或使用 Handles）
        private void DrawGradientLine(Vector3 start, Vector3 end, Color startColor, Color endColor)
        {
#if UNITY_EDITOR
            UnityEditor.Handles.DrawBezier(start, end, start, end, startColor, null, 2f);
            UnityEditor.Handles.color = endColor;
            UnityEditor.Handles.DrawLine(end, end);
#endif
        }
    }
}