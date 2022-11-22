Shader "Shader/Checkerboard"
{
	Properties{
		_Scale("Pattern Size", Range(0,10)) = 1
		_EvenColor("Color 1", Color) = (0,0,0,1)
		_OddColor("Color 2", Color) = (1,1,1,1)
		_Offset("Offset", Vector) = (0,0,0,0)
	}

	SubShader{
		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry"}


		Pass{
			CGPROGRAM
			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			float _Scale;

			float4 _EvenColor;
			float4 _OddColor;
			float4 _Offset;

			struct appdata {
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				float4 position : SV_POSITION;
				float3 worldPos : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert(appdata v) {
				v2f o;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.position = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex) - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz + _Offset;
				return o;
			}

			fixed4 frag(v2f i) : SV_TARGET{
				float3 adjustedWorldPos = floor(i.worldPos / _Scale);
				float chessboard = adjustedWorldPos.x + adjustedWorldPos.y + adjustedWorldPos.z;
				chessboard = frac(chessboard * 0.5) * 2;
				float4 color = lerp(_EvenColor, _OddColor, chessboard);
				return color;
			}

			ENDCG
		}
	}
	
		FallBack "Standard" //fallback adds a shadow pass so we get shadows on other objects
}