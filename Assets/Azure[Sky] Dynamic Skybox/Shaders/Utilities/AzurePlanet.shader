Shader "Azure[Sky]/Planet"
{
	Properties
	{
		_MainTex("Texture Map", 2D) = ""{}
		_Saturation("Saturation", Range(0.5,2)) = 1.0
		_Penumbra("Penumbra", Range(0,4))       = 0.5
		_Shadow("Shadow", Range(0,0.25))  = 0.0
	}
	SubShader
	{
		Tags { "Queue"="Geometry" "RenderType"="Opaque" "IgnoreProjector"="True" }
		LOD 200

		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
			};

			#define           _pi	3.14159265359f
			uniform float3    _Azure_SunDirection;
			uniform sampler2D _MainTex;
			float     		  _Saturation, _Penumbra, _Shadow;

			v2f vert (appdata v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				o.vertex = UnityObjectToClipPos(v.vertex);
			    o.normal = v.normal;
				return o;
			}
			
			float4 frag (v2f IN) : SV_Target
			{
				float4 outColor = float4(1.0, 1.0, 1.0,1.0);
				float2 uv  = float2(atan2(IN.normal.z, IN.normal.x), acos(IN.normal.y)) / float2(2.0 * _pi, _pi);
			    float3 moonTex = tex2D( _MainTex, uv).rgb;
			    
			    float3 lightDir = normalize(_Azure_SunDirection);
			    float  light = max(dot(IN.normal, lightDir), 0.0) + _Shadow;
			    
			    outColor.rgb *= moonTex * pow(light, _Penumbra);
			    outColor.rgb = pow(outColor.rgb, _Saturation);
			    return outColor;
			}
			ENDCG
		}
	}
	Fallback "Legacy Shaders/VertexLit"
}