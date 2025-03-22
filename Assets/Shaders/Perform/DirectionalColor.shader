Shader "Phos/Perform/DirectionalColor"
{
    Properties
    {
        _Albedo ("Albedo (RGB)", 2D) = "white" {}
        _Emission ("Emission", 2D) = "Black" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        
        _LightColor ("Light Side Color", Color) = (1,1,1,1)    // 受光面颜色
        _DarkColor ("Dark Side Color", Color) = (0.2,0.2,0.2,1) // 背光面颜色
        _TransitionColor ("Transition Color", Color) = (0.5, 0.5, 0.5, 1)
        
    	[Toggle(GRADIENT)] _Gradient ("Gradient", Float) = 0
    	_ScreenHigherAmbient ("Screen Higher Ambient", Color) = (1,1,1,1)
    	_ScreenLowerAmbient ("Screen Lower Ambient", Color) = (1,1,1,1)
    	_ScreenOffset ("Screen Offset", Range(0, 1)) = 0
    	_ScreenFactor ("Screen Factor", Range(0, 2)) = 0
        
        _Transition ("Transition", Range (0,1)) = 0.5
        _Intensity ("Intensity", Range(0,1)) = 0.2
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows
        #pragma shader_feature GRADIENT

        #include "UnityCG.cginc"

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _Albedo;
        sampler2D _Emission;

        struct Input
        {
            float2 uv_Albedo;
            float3 worldNormal;
            float3 worldPos;
            float4 screenPos;
        };

        half _Glossiness;
        half _Metallic;

        half _Transition;
        half _Intensity;

        fixed4 _LightColor;
        fixed4 _DarkColor;
        fixed4 _TransitionColor;

        
        #if defined(GRADIENT)
        fixed4 _ScreenHigherAmbient;
        fixed4 _ScreenLowerAmbient;
        fixed _ScreenOffset;
        fixed _ScreenFactor;
        #endif

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
        

        void surf (Input IN, inout SurfaceOutputStandard o) {
            // 计算光照方向
            float3 lightDir = normalize(UnityWorldSpaceLightDir(IN.worldPos));
            
            // 计算法线与光源的夹角强度
            float NdotL = dot(normalize(IN.worldNormal), lightDir);

            fixed4 ambient = lerp (
                lerp (
                    _DarkColor,
                    _TransitionColor,
                    smoothstep(-1, _Transition, NdotL)
                ),
                _LightColor,
                smoothstep(_Transition, 1, NdotL)
            );

            fixed4 albedo = tex2D(_Albedo, IN.uv_Albedo);
            fixed4 emission = tex2D(_Emission, IN.uv_Albedo);
            fixed intensity = ambient.a * _Intensity;
            ambient = albedo * (1 - intensity) + ambient * intensity;
            
            #if defined(GRADIENT)
            fixed3 screenAmbient = lerp(_ScreenLowerAmbient, _ScreenHigherAmbient, saturate((IN.screenPos.y + _ScreenOffset) * _ScreenFactor));
            ambient.rgb *= screenAmbient;
            #endif
            
            // 输出结果
            o.Albedo = ambient.rgb;
            o.Alpha = 1;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Emission = emission;
        }
        ENDCG
    }
    FallBack "Diffuse"
}