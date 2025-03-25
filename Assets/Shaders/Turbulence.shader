// from https://github.com/BayatGames/RedRunner/blob/master/Assets/Shaders/UI%20Blur.shader
Shader "Phos/Turbulence"
{
    Properties
    {
		_Size ("Size", Range(0, 1)) = 0.2
    	_NoiseTex ("Noise", 2D) = "black" {}
    }
    SubShader
    {
        Tags {
        	"RenderType"="Transparent"
        	"Queue"="Transparent"
        	
        }
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
                float4 color : COLOR;
            };

			struct v2f
			{
				float4 vertex : SV_POSITION;
                float4 worldPos : TEXCOORD1;
            	float4 screenPos : TEXCOORD2;
                float4 color : COLOR;
			};
            
			sampler2D _GrabTexture;
            sampler2D _NoiseTex;
			float4 _GrabTexture_TexelSize;
			float _Size;

            v2f vert(appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
            	o.worldPos = mul(unity_ObjectToWorld, v.vertex);
            	o.screenPos = ComputeScreenPos(o.vertex);
            	o.color = v.color;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed2 uv = i.screenPos.xy / i.screenPos.w;
				fixed noise = tex2D(_NoiseTex, uv + _Time * 0.02) - 0.5;
				fixed2 dir = fixed2(noise * _Size * 0.1, 0);
				return tex2D(_GrabTexture, uv + dir);
			}
            ENDCG
        }
    }
	Fallback "UI/Unlit/Transparent"
}
