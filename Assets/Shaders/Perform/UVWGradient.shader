Shader "Phos/Perform/UVWGradient"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (1,0,0,1)
        _BottomColor ("Bottom Color", Color) = (0,1,0,1)
        
        _FarTopColor ("Far Top Color", Color) = (0,0,1,1)
        _FarBottomColor ("Far Bottom Color", Color) = (1,0,0,1)
        
        _Top ("Top", Range(0,1)) = 0
        _Bottom ("Bottom", Range(0, 1)) = 1
        
        _Nearest ("Nearest", Float) = 5
        _Far ("Far", Float) = 100
        
        _Position ("Position", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

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
                float2 screen : TEXCOORD1;
            };

            fixed4 _TopColor;
            fixed4 _BottomColor;

            fixed4 _FarTopColor;
            fixed4 _FarBottomColor;

            float4 _Position;

            fixed _Top;
            fixed _Bottom;

            float _Nearest;
            float _Far;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float3 world = mul(unity_ObjectToWorld, v.vertex);
                o.screen.x = smoothstep(_Far, _Nearest, distance(world, _WorldSpaceCameraPos));
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed far = i.screen.x;
                fixed4 color = lerp(
                    _BottomColor,
                    _TopColor,
                    smoothstep(_Bottom, _Top, i.uv.y)
                    );
                color.rgb += _FarBottomColor.rgb * far * _FarBottomColor.a;
                return color;
            }
            ENDCG
        }
    }
}