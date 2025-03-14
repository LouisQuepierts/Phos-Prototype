// from https://github.com/BayatGames/RedRunner/blob/master/Assets/Shaders/UI%20Blur.shader
Shader "Phos/UI/FrostedGlass"
{
    Properties
    {
		_Size ("Size", Range(0, 8)) = 1.0
    	_Color ("Color", Color) = (1, 1, 1, 1)
    	
        _Rect ("Rect", Vector) = (1, 1, 1, 1)
        _Smooth ("Smooth", Range(0, 1)) = 0.05
        _Radius ("Radius", float) = 5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

		GrabPass {Tags{"LightMode" = "Always"}}

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
                float4 vertex	: POSITION;
                float2 uv		: TEXCOORD0;
            };

			struct v2f
			{
				float4 uvgrab : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};
            
			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;
			float _Size;
            half4 _Color;
			
            float4 _Rect;
            fixed _Smooth;
            fixed _Radius;

            float sd_box(fixed2 p, fixed2 b) {
                fixed2 d = abs(p)-b;
                return length(max(d,0.0)) + min(max(d.x,d.y),0.0);
            }

            v2f vert(appdata_base v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
				#else
				float scale = 1.0;
				#endif
				o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
				o.uvgrab.zw = o.vertex.zw;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				half4 sum = half4(0,0,0,0);

				#define GRABPIXEL(weight,kernelx) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x + _GrabTexture_TexelSize.x * kernelx*_Size, i.uvgrab.y, i.uvgrab.z, i.uvgrab.w))) * weight

				sum += GRABPIXEL(0.05, -4.0);
				sum += GRABPIXEL(0.09, -3.0);
				sum += GRABPIXEL(0.12, -2.0);
				sum += GRABPIXEL(0.15, -1.0);
				sum += GRABPIXEL(0.18, 0.0);
				sum += GRABPIXEL(0.15, +1.0);
				sum += GRABPIXEL(0.12, +2.0);
				sum += GRABPIXEL(0.09, +3.0);
				sum += GRABPIXEL(0.05, +4.0);
				sum *= _Color;

				fixed2 rect = fixed2(
                    max(_Rect.x / _Rect.y, 1),
                    max(_Rect.y / _Rect.x, 1)
                );
                float shorter = min(_Rect.x, _Rect.y);
                float radius = _Radius / shorter;

                fixed2 pt = (i.uvgrab.xy * 2.0 - 1.0) * rect;
                fixed2 b = max(rect - radius, 0.0);

                float distance = sd_box(pt, b) - radius;

                float alpha = smoothstep(0, _Smooth, -distance);
				sum.a *= alpha;
				return sum;
			}
            ENDCG
        }

		GrabPass{ Tags{ "LightMode" = "Always" } }

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
			};

			struct v2f
			{
				float4 uvgrab : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;
			float _Size;
			half4 _Color;
			
            float4 _Rect;
            fixed _Smooth;
            fixed _Radius;

            float sd_box(fixed2 p, fixed2 b) {
                fixed2 d = abs(p)-b;
                return length(max(d,0.0)) + min(max(d.x,d.y),0.0);
            }
			
			v2f vert(appdata_base v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
				#else
				float scale = 1.0;
				#endif
				o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
				o.uvgrab.zw = o.vertex.zw;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				half4 sum = half4(0,0,0,0);

				#define GRABPIXEL(weight,kernely) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x, i.uvgrab.y + _GrabTexture_TexelSize.y * kernely*_Size, i.uvgrab.z, i.uvgrab.w))) * weight

				sum += GRABPIXEL(0.05, -4.0);
				sum += GRABPIXEL(0.09, -3.0);
				sum += GRABPIXEL(0.12, -2.0);
				sum += GRABPIXEL(0.15, -1.0);
				sum += GRABPIXEL(0.18, 0.0);
				sum += GRABPIXEL(0.15, +1.0);
				sum += GRABPIXEL(0.12, +2.0);
				sum += GRABPIXEL(0.09, +3.0);
				sum += GRABPIXEL(0.05, +4.0);
				sum *= _Color;

				fixed2 rect = fixed2(
                    max(_Rect.x / _Rect.y, 1),
                    max(_Rect.y / _Rect.x, 1)
                );
                float shorter = min(_Rect.x, _Rect.y);
                float radius = _Radius / shorter;

                fixed2 pt = (i.uvgrab.xy * 2.0 - 1.0) * rect;
                fixed2 b = max(rect - radius, 0.0);

                float distance = sd_box(pt, b) - radius;

                float alpha = smoothstep(0, _Smooth, -distance);
				sum.a *= alpha;
				return sum;
			}
			ENDCG
		}
    }
	Fallback "UI/Unlit/Transparent"
}
