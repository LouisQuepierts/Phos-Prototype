Shader "Phos/InteractDevice/Rotary"
{
    Properties
    {
        _MainTex ("Albedo", 2D) = "white" {}
        
        _BaseColor ("Base Color", Color) = (1,0,0,1)
        _RodColor ("Rod Color", Color) = (0,1,0,1)
        _HandleColor ("Handle Color", Color) = (0,0,1,1)
        
        _Highlight ("Highlight", Range(0, 1)) = 0.0
        _HighlightColor ("Highlight Color", Color) = (1,1,1,1)
        
        [Toggle(GRADIENT)] _Gradient ("Gradient", Range(0, 1)) = 0
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

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
        #pragma shader_feature GRADIENT

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float4 screenPos;
        };

        fixed4 _BaseColor;
        fixed4 _RodColor;
        fixed4 _HandleColor;

        fixed _Highlight;
        fixed4 _HighlightColor;

        #include "../ambient_gradient.cginc"

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed3 mask = tex2D(_MainTex, IN.uv_MainTex);
            fixed handle = step(0.5, mask.b);
            fixed4 c = _BaseColor * step(0.5, mask.r)
                + _RodColor * step(0.5, mask.g)
                + _HandleColor * handle;
            
            c = phos_gradient(IN.screenPos.y, c);
            c.rgb = lerp(c.rgb, _HighlightColor.rgb, _Highlight * _HighlightColor.a * handle);
            
            o.Albedo = c.rgb;
            o.Metallic = 0;
            o.Smoothness = 0;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}