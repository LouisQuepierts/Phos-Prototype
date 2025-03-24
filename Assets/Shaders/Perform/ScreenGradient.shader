Shader "Phos/Perform/ScreenGradient"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (1,0,0,1)
        _BottomColor ("Bottom Color", Color) = (0,1,0,1)
        
        _Top ("Top", Range(0,1)) = 0
        _Bottom ("Bottom", Range(0, 1)) = 1
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _TopColor;
            fixed4 _BottomColor;

            fixed _Top;
            fixed _Bottom;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return lerp(_BottomColor, _TopColor, smoothstep(_Bottom, _Top, i.uv.y));
            }
            ENDCG
        }
    }
}