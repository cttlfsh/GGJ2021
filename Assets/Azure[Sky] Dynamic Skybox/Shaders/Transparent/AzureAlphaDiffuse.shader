// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Azure[Sky]/BuiltIn/Legacy Shaders/Transparent/Diffuse"
{
Properties
{
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

SubShader
{
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 200

	CGPROGRAM

		// Azure[Sky] Start: Including the file that contains the methods and functions needed to apply the fog.
		#include "Assets/Azure[Sky] Dynamic Skybox/Shaders/Transparent/AzureFogCore.cginc"
		// Azure[Sky] End.

		#pragma surface surf Lambert finalcolor:ApplyFog alpha:fade vertex:vert

		sampler2D _MainTex;
		fixed4 _Color;

		struct Input
		{
			float2 uv_MainTex;

			// Azure[Sky] Start: Adding the extra TEXCOORD to store the vertex world position.
			float3 worldPos;
			// Azure[Sky] End.
		};

		// Azure[Sky] Start: In this case it was necessary to create a vertex function to calculate the vertex world position because this surface shader...
		// did not provide this by default.
		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			// Calculating the vertex world position and storing in the extra TEXCOORD.
			o.worldPos = mul(unity_ObjectToWorld, v.vertex);
		}
		// Azure[Sky] End.

		// Azure[Sky] Start: Using Final Color Modifier to apply the fog scattering.
		void ApplyFog(Input IN, SurfaceOutput o, inout fixed4 color)
		{
			color = ApplyAzureFog(color, IN.worldPos);
		}
		// Azure[Sky] End.

		void surf (Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}

	ENDCG
	}
	Fallback "Legacy Shaders/Transparent/VertexLit"
}
