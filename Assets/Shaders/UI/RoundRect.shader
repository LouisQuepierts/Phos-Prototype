Shader "Phos/UI/RoundRect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Rect ("Rect", Vector) = (1, 1, 1, 1)
        _Smooth ("Smooth", Range(0, 1)) = 0.05
        _Radius ("Radius", float) = 5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
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
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            float sd_box(fixed2 p, fixed2 b) {
                fixed2 d = abs(p)-b;
                return length(max(d,0.0)) + min(max(d.x,d.y),0.0);
            }

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Rect;

            fixed _Smooth;
            fixed _Radius;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed2 rect = fixed2(
                    _Rect.x / _Rect.y,
                    _Rect.y / _Rect.x
                );
                float shorter = min(_Rect.x, _Rect.y);
                float radius = _Radius / shorter;

                fixed2 pt = (i.uv.xy * 2.0 - 1.0) * rect;
                fixed2 b = max(rect - radius, 0.0);

                fixed2 d = abs(pt) - b;
                float distance = min(max(d.x,d.y),0.0) + length(max(d,0.0)) - radius;

                float alpha = smoothstep(0, _Smooth, -distance);
                fixed4 col = i.color * alpha;
                return col;
            }
            ENDCG
        }
    }
}
