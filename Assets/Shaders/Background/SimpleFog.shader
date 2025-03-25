﻿Shader "Phos/Background/SimpleFog"
{
    Properties
    {
    	_ScreenHigherAmbient ("Screen Higher Ambient", Color) = (1,1,1,1)
    	_ScreenLowerAmbient ("Screen Lower Ambient", Color) = (1,1,1,1)
    	_ScreenOffset ("Screen Offset", Range(-1, 1)) = 0
        
    	_DeepFactor ("Deep Factor", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
        	"Queue"="Transparent"
        }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha

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

            struct v2g
            {
	            float4 vertex : SV_POSITION;
                float4 worldPos : TEXCOORD1;
            	float4 screenPos : TEXCOORD2;
            	float2 uv : TEXCOORD0;
            };

            struct g2f
            {
                float4 vertex : SV_POSITION;
                float4 worldPos : TEXCOORD1;
            	float4 screenPos : TEXCOORD2;
                float3 normal : TEXCOORD3;
            	float2 uv : TEXCOORD0;
            };

            struct v2f
            {
	            float4 vertex : SV_POSITION;
                float4 worldPos : TEXCOORD1;
            	float4 screenPos : TEXCOORD2;
            };

            sampler2D _CameraDepthTexture;
            float4 _CameraDepthTexture_ST;

            fixed4 _ScreenHigherAmbient;
            fixed4 _ScreenLowerAmbient;
            fixed _ScreenOffset;
            
            float _DeepFactor;

            float2 unity_gradientNoise_dir(float2 p)
            {
                p = p % 289;
                float x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }

            inline float unity_gradientNoise(float2 p)
            {
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(unity_gradientNoise_dir(ip), fp);
                float d01 = dot(unity_gradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(unity_gradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(unity_gradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
            }

            void Unity_GradientNoise_float(fixed2 UV, float Scale, out float Out)
            {
            	Out = unity_gradientNoise(UV * Scale);
            }

            inline float calculate_depth(float2 uv)
            {
				float _Camera_NearPlane = _ProjectionParams.y;
				float _Camera_FarPlane = _ProjectionParams.z;

            	float _raw_depth = SAMPLE_RAW_DEPTH_TEXTURE(_CameraDepthTexture, uv).r;
            	return - lerp(_Camera_FarPlane, _Camera_NearPlane, _raw_depth);
            }

            inline float4 depth_2_world_pos(float2 view_position, float depth)
            {
            	float4 world_position = mul(UNITY_MATRIX_I_V, float4(view_position.xy, depth, 1));

            	// float noise = tex2D(_FoamNoiseTex, world_position.xz * _DepthNoiseScale + _Time.y * 0.01);
            	// world_position.y += noise * _DepthNoiseAmplifier;
            	return world_position;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
            	o.worldPos = mul(unity_ObjectToWorld, v.vertex);
            	o.screenPos = ComputeScreenPos(o.vertex);
            	// o.uv = v.uv;
                //o.normal = UnityWorldToObjectDir(wave.normal);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
            	float4 view_pos = mul(UNITY_MATRIX_V, i.worldPos);
            	float4 world_pos = depth_2_world_pos(view_pos.xy, calculate_depth(i.screenPos.xy));

            	float height = smoothstep(0, 10 * _DeepFactor, i.worldPos.y - world_pos.y);
            	fixed4 color = lerp(_ScreenLowerAmbient, _ScreenHigherAmbient, saturate(i.screenPos.y + _ScreenOffset));
            	
            	color.a = height;
            	// color = refract_height;
            	
                return color;
            }
            ENDCG
        }
    }
}