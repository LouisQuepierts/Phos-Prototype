using System.Collections.Generic;
using UnityEngine;

namespace Phos.Optical {
    public abstract class BaseLens : OpticalDevice, ILightAcceptable {
        [Header("光学参数")]
        public float RefractiveIndex = 1.5f; // 折射率（玻璃≈1.5）
        public float FocalLength = 2.0f;     // 焦距（凸透镜为正，凹透镜为负）
        public float LensThickness = 0.1f;   // 中心厚度
        public float Aperture = 1.0f;        // 孔径直径

        protected Vector3 SurfaceNormal;     // 当前计算的表面法线
        protected bool IsEntering;           // 光线进入/离开状态
    
        // 通用折射计算
        protected Vector3 CalculateRefraction(Vector3 inDir, Vector3 normal, float eta)
        {
            float cosTheta = Vector3.Dot(-inDir, normal);
            float sinThetaSq = eta * eta * (1 - cosTheta * cosTheta);
        
            if(sinThetaSq > 1) 
                return Vector3.Reflect(inDir, normal); // 全反射
        
            float cosPhi = Mathf.Sqrt(1 - sinThetaSq);
            return eta * inDir + (eta * cosTheta - cosPhi) * normal;
        }

        public abstract void OnLightHitted(LightData income, RaycastHit hit, out List<LightData> outgo);
    }
}