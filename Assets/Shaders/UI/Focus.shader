Shader "Phos/UI/Focus"
{
    Properties
    {
        _Radius ("Radius", Range(0, 5)) = 0.2
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
                float4 screen : TEXCOORD1;
                float4 color : COLOR;
            };

            float _Radius;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screen = ComputeScreenPos(o.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = i.color;
                fixed2 center = i.uv.xy * 2 - 1;
                center.x *= _ScreenParams.x / _ScreenParams.y;

                float dist = length(center);
                col.a *= smoothstep(_Radius - 0.001, _Radius + 0.001, dist);
                return col;
            }
            ENDCG
        }
    }
}
