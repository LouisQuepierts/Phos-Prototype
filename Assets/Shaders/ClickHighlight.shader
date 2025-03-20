Shader "Unlit/ClickHighlight"
{
    Properties
    {
        _Highlight ("Highlight", Range(0.0, 1.0)) = 0.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
        }
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
                fixed4 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 uv : TEXCOORD0;
            };

            fixed _Highlight;
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed2 uv = i.uv.xy * 2 - 1;
                fixed range = smoothstep(0.3, 1.0, _Highlight);
                fixed dist = length(uv);
                fixed4 col = range;
                col.a = smoothstep(0.0, 0.25, _Highlight) * step(dist, 1 - (1 - range) * 0.25);
                clip(col.a);
                return col;
            }
            ENDCG
        }
    }
}