Shader "Phos/Background/WaveWater"
{
    Properties
    {
    	_SurfaceColor ("Surface Color", Color) = (0.54,0.87,1,1)
        _LighterColor ("Lighter Color", Color) = (0.86,1,0.98,1)
    	_DarkerColor ("Darker Color", Color) = (0.73,0.21,0.82,1)
    	_DeepColor ("Deep Color", Color) = (0.44,0.40,0.65,1)
    	
    	_DeepFactor ("Deep Factor", Range(0, 1)) = 0.5
    	
    	_FoamNoiseTex ("Foam Noise Tex", 2D) = "white" {}
    	_FoamColor ("Foam Color", Color) = (1,1,1,1)
    	_FoamWidth ("Foam Width", Range(0, 1)) = 0.3
    	_FoamNoiseScale ("Foam Noise Scale", Range(0, 1)) = 0.05
    	_FoamNoiseAmplifier ("Foam Noise Amplifier", Range(0, 1)) = 0.3
    	_FoamNoiseSpeed ("Foam Noise Speed", Range(0, 1)) = 0.01
    	
        _WaveNoiseTex ("Wave Noise Tex", 2D) = "white" {}
    	_WaveNoiseScale ("Wave Noise Scale", Range(0, 20)) = 0.5
    	_WaveNoiseAmplifier ("Wave Noise Amplifier", Range(0, 1)) = 0.5
    	
//    	_TurbulenceTex ("Turbulence Noise Tex", 2D) = "white" {}
//    	_TurbulenceScale ("Turbulence Noise Scale", Range(0, 5)) = 0.05
//    	_TurbulenceAmplifier ("Turbulence Noise Amplifier", Range(0, 1)) = 0.5
    	
    	// length, amplitude, speed
    	
    	[Toggle(GRADIENT)] _Gradient ("Gradient", Float) = 0
    	
    	_Wave1 ("Wave 1", Vector) = (0, 0, 0, 0)
    	_Wave2 ("Wave 2", Vector) = (0, 0, 0, 0)
    	_Wave3 ("Wave 3", Vector) = (0, 0, 0, 0)
    	_Wave4 ("Wave 4", Vector) = (0, 0, 0, 0)
    	
    	[Toggle] _EnableWave1 ("Enable Wave 1", Float) = 0
    	[Toggle] _EnableWave2 ("Enable Wave 2", Float) = 0
    	[Toggle] _EnableWave3 ("Enable Wave 3", Float) = 0
    	[Toggle] _EnableWave4 ("Enable Wave 4", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
        }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom
            // make fog work
            #pragma multi_compile_fog
            #pragma shader_feature GRADIENT

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

            
            sampler2D _WaveNoiseTex;
            sampler2D _FoamNoiseTex;
            // sampler2D _TurbulenceTex;

            sampler2D _CameraDepthTexture;
            float4 _CameraDepthTexture_ST;

            fixed4 _SurfaceColor;
            fixed4 _LighterColor;
            fixed4 _DarkerColor;
            fixed4 _DeepColor;

            fixed4 _FoamColor;
            float _FoamWidth;
            float _FoamNoiseScale;
            float _FoamNoiseAmplifier;
            float _FoamNoiseSpeed;
            
            float _DeepFactor;

            float _WaveNoiseScale;
            float _WaveNoiseAmplifier;

            // fixed _TurbulenceScale;
            // fixed _TurbulenceAmplifier;
            
            float4 _Wave1;
            float4 _Wave2;
            float4 _Wave3;
            float4 _Wave4;

            fixed _EnableWave1;
            fixed _EnableWave2;
            fixed _EnableWave3;
            fixed _EnableWave4;

            #include "../ambient_gradient.cginc"

            float calculate_wave(float3 pos, float amplitude, float wavelength, float speed, float direction)
            {
				float time = _Time.y;
            	float frequency = 2 / max(wavelength, 1e-6);
				direction = radians(direction);
				half2 forward = normalize(half2(cos(direction), sin(direction)));

            	//float noise = tex2Dlod(_WaveNoiseTex, float4(pos.xz * _WaveNoiseScale + time * 0.01, 0, 0)) * _WaveNoiseAmplifier;
            	
            	float d = dot(pos.xz, forward);
            	return amplitude * sin((d + time * speed) * frequency);
            }
            
			float calculate_wave_at(float3 vertex)
            {
				float wave = 0;
            	
            	float w1 = calculate_wave(vertex, _Wave1.x, _Wave1.y, _Wave1.z, _Wave1.w);
            	wave += w1 * _EnableWave1;
            	float w2 = calculate_wave(vertex, _Wave2.x, _Wave2.y, _Wave2.z, _Wave2.w);
            	wave += w2 * _EnableWave2;
            	float w3 = calculate_wave(vertex, _Wave3.x, _Wave3.y, _Wave3.z, _Wave3.w);
            	wave += w3 * _EnableWave3;
            	float w4 = calculate_wave(vertex, _Wave4.x, _Wave4.y, _Wave4.z, _Wave4.w);
            	wave += w4 * _EnableWave4;
            	
				return wave / (_EnableWave1 + _EnableWave2 + _EnableWave3 + _EnableWave4);
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

            	// float noise = tex2D(_FoamNoiseTex, world_position.xz * _FoamNoiseScale + _Time.y * 0.01);
            	// world_position.y += noise * _FoamNoiseAmplifier;
            	return world_position;
            }

    //         inline fixed2 turbulence_dir(fixed2 uv)
    //         {
				// fixed noise = tex2D(_TurbulenceTex, uv * _TurbulenceScale + _Time * 0.02) - 0.5;
				// fixed2 dir = fixed2(noise * _TurbulenceAmplifier * 0.1, 0);
    //         	return dir;
    //         }

            v2g vert(appdata v)
            {
                v2g o;
            	float wave = calculate_wave_at(mul(unity_ObjectToWorld, v.vertex).xyz);
            	v.vertex.y += wave;
                o.vertex = UnityObjectToClipPos(v.vertex);
            	o.worldPos = mul(unity_ObjectToWorld, v.vertex);
            	o.screenPos = ComputeScreenPos(o.vertex);
            	o.uv = v.uv;
                //o.normal = UnityWorldToObjectDir(wave.normal);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            [maxvertexcount(3)]
			void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream) {
			    float3 A = IN[1].worldPos.xyz - IN[0].worldPos.xyz;
			    float3 B = IN[2].worldPos.xyz - IN[0].worldPos.xyz;
			    float3 fn = normalize(cross(A, B));

			    g2f o;
			    o.vertex = IN[0].vertex;
			    o.worldPos = IN[0].worldPos;
            	o.screenPos = IN[0].screenPos;
            	o.uv = IN[0].uv;
			    o.normal = fn;
			    triStream.Append(o);

			    o.vertex = IN[1].vertex;
			    o.worldPos = IN[1].worldPos;
            	o.screenPos = IN[1].screenPos;
            	o.uv = IN[1].uv;
			    o.normal = fn;
			    triStream.Append(o);

			    o.vertex = IN[2].vertex;
			    o.worldPos = IN[2].worldPos;
            	o.screenPos = IN[2].screenPos;
            	o.uv = IN[2].uv;
			    o.normal = fn;
			    triStream.Append(o);
			}

            fixed4 frag(g2f i) : SV_Target
            {
				fixed3 worldNormal = normalize(i.normal);
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);

            	float4 view_pos = mul(UNITY_MATRIX_V, i.worldPos);
            	
            	float4 world_pos = depth_2_world_pos(view_pos.xy, calculate_depth(i.screenPos.xy));
            	float height = smoothstep(0, 10 * _DeepFactor, i.worldPos.y - world_pos.y);

            	float foamNoise = tex2D(_FoamNoiseTex, world_pos.xz * _FoamNoiseScale + _Time.y *_FoamNoiseSpeed);
            	float foam = step(height + foamNoise * _FoamNoiseAmplifier, _FoamWidth);
            	
            	fixed lightDot = dot(worldNormal, worldLightDir);
				fixed3 diffuse = lerp(_DarkerColor, _SurfaceColor, saturate(lightDot / 2 + 0.5));
            	
            	// fixed2 turbulenced_dir = turbulence_dir(i.screenPos.xy);
            	// fixed2 turbulenced_uv = i.screenPos.xy + turbulenced_dir;
            	// float turbulenced_y = depth_2_world_pos(view_pos.xy, calculate_depth(turbulenced_uv)).y;
	            //
            	// float y = lerp(world_pos.y, turbulenced_y, step(0, i.worldPos.y - turbulenced_y));
            	// float turbulenced_height = step(10 * _DeepFactor, i.worldPos.y - y);
            	
            	diffuse = lerp(diffuse, _LighterColor, smoothstep(0.75, 1.0, lightDot));
				fixed3 color = lerp(_DeepColor, diffuse, height);
            	color = phos_gradient(i.screenPos.y, fixed4(color, 1)).rgb;

            	fixed3 surface = lerp(color, _FoamColor, foam);
            	
                return fixed4(surface, 1);
            }
            ENDCG
        }
    }
}