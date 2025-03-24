Shader "Phos/Background/OceanWater"
{
    // Waves from https://blog.csdn.net/u012740992/article/details/104181969
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        
		_WaveLength("WaveLength", float) = 4//水波长度，世界空间中波之间的波峰到波峰的距离
		_WaveAmplitude("WaveAmplitude", float) = 0.4//振幅，从水平面到波峰的高度
		_WindDirection("WindDirection", Range(0, 360)) = 70//风方向
		_WindSpeed("WindSpeed", float) = 0.4//风速系数
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
            #pragma geometry geom
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2g
            {
	            float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float3 normal : TEXCOORD2;
            };

            struct g2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float3 normal : TEXCOORD2;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            	float3 normal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _Color;
            float _WaveLength;
            float _WaveAmplitude;
            float _WindDirection;
            float _WindSpeed;
            
			struct wave {
				float3 vertex;
				float3 normal;
			};

            wave GerstnerWave(half2 pos, float waveCount, half waveLen, half amplitude, half direction, half windSpeed) {
				//方向（D）：垂直于波峰传播的波阵面的水平矢量。
				//波长（L）：世界空间中波之间的波峰到波峰的距离。
				//振幅（A）：从水平面到波峰的高度。
				//速度（S）：波峰每秒向前移动的距离。
				//陡度 (Q) : 控制水波的陡度。
				wave waveOut;
				float time = _Time.y;
				direction = radians(direction);
				half2 D = normalize(half2(sin(direction), cos(direction)));
				half w = 6.28318 / waveLen;
				half A = amplitude;
				half S = windSpeed * sqrt(9.8 * w);
				half Q = 1 / (A * w * waveCount);

				half commonCalc = w * dot(D, pos) + time * S;
		
				half cosC = cos(commonCalc);
				half sinC = sin(commonCalc);
				waveOut.vertex.xz = Q * A * D.xy * cosC;
				waveOut.vertex.y = (A * sinC) / waveCount;
				half WA = w * A;
				waveOut.normal = half3(-(D.xy * WA * cosC), 1 - (Q * WA * sinC));
				waveOut.normal = waveOut.normal/waveCount;
				return waveOut;
			}
            
			wave GenWave(float3 vertex) {
				half2 pos = vertex.xz;
				wave wave_out;
				uint count = 4;
				for (uint i = 0; i < count; i++) {
					wave wave = GerstnerWave(pos, count, _WaveLength, _WaveAmplitude, _WindDirection, _WindSpeed);
					wave_out.vertex += wave.vertex;
					wave_out.normal += wave.normal;
				}
				return wave_out;
			}

            v2g vert(appdata v)
            {
                v2g o;
            	wave wave = GenWave(v.vertex.xyz);
            	v.vertex.xyz += wave.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
            	o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.normal = UnityWorldToObjectDir(wave.normal);
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
			    o.normal = fn;
			    triStream.Append(o);

			    o.vertex = IN[1].vertex;
			    o.worldPos = IN[1].worldPos;
			    o.normal = fn;
			    triStream.Append(o);

			    o.vertex = IN[2].vertex;
			    o.worldPos = IN[2].worldPos;
			    o.normal = fn;
			    triStream.Append(o);
			}

            fixed4 frag(g2f i) : SV_Target
            {
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
				fixed3 worldNormal = normalize(i.normal);
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				fixed3 diffuse = _LightColor0.rgb * _Color.rgb * saturate(dot(worldNormal,worldLightDir));
				fixed3 color = ambient + diffuse;
                return fixed4(color, 1);
            }
            ENDCG
        }
    }
}