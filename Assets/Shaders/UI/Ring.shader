Shader "Phos/UI/Ring"
{
    Properties
    {
        _Radius ("Radius", Range(0, 1)) = 0.2
        _Width ("Width", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags {
            "Queue" = "AlphaTest"
            "IgnoreProjector" = "True"
        }
        LOD 100
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            float _Radius;
            float _Width;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = i.color;
                fixed2 center = i.uv.xy * 2 - 1;

                float dist = abs(length(center) - _Radius) - _Width;
                col.a *= 1 - smoothstep(-0.001, 0.001, dist);
                return col;
            }
            ENDCG
        }
    }
}
