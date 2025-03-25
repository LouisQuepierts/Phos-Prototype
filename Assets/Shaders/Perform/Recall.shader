Shader "Phos/Perform/Recall"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _DarkColor ("Dark Color", Color) = (0.8, 0.8, 0.8,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NoiseTex ("Noise", 2D) = "white" {}
        _Dissolve ("Dissolve", Range(0, 1)) = 0
        _DissolveScale ("Dissolve Scale", Range(0, 2)) = 1.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _MainTex_ST;
            
            fixed4 _Color;
            fixed4 _DarkColor;
            fixed _Dissolve;
            fixed _DissolveScale;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = mul(unity_ObjectToWorld, float4(v.normal, 0)).xyz;
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed noise = tex2D(_NoiseTex, i.uv * _DissolveScale).r + _Dissolve * 1.1;
                
                clip(1 - noise);
                
				fixed3 worldNormal = normalize(i.normal);
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
            	fixed lightDot = dot(worldNormal, worldLightDir);

                col *= lerp(_DarkColor, _Color, saturate(lightDot / 2 + 0.5));
                
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}