Shader "Azure[Sky]/Fog Scattering"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"
			
		//  Start: LuxWater
			#pragma multi_compile __ LUXWATER_DEFERREDFOG

			#if defined(LUXWATER_DEFERREDFOG)
				sampler2D _UnderWaterMask;
				float4 _LuxUnderWaterDeferredFogParams; // x: IsInsideWatervolume?, y: BelowWaterSurface shift, z: EdgeBlend
			#endif
		//  End: LuxWater

            // Constants
			#define PI 3.1415926535
			#define Pi316 0.0596831
			#define Pi14 0.07957747
			#define MieG float3(0.4375f, 1.5625f, 1.5f)
            
            // Inputs
            uniform sampler2D _MainTex;
			uniform sampler2D_float _CameraDepthTexture;
			uniform float4x4  _FrustumCorners;
			uniform float4    _MainTex_TexelSize;
			
			uniform int       _Azure_StylizedTransmittanceMode;
			uniform float3    _Azure_SunDirection, _Azure_MoonDirection;
			uniform float3    _Azure_Br, _Azure_Bm;
			uniform float     _Azure_ScatteringIntensity, _Azure_SkyLuminance, _Azure_Exposure;
			uniform float3    _Azure_RayleighColor, _Azure_MieColor, _Azure_TransmittanceColor;
			uniform float     _Azure_FogScatteringScale, _Azure_GlobalFogSmooth, _Azure_GlobalFogDistance, _Azure_GlobalFogDensity;
			uniform float     _Azure_HeightFogSmooth, _Azure_HeightFogDistance, _Azure_HeightFogDensity, _Azure_HeightFogStart, _Azure_HeightFogEnd;
			
			uniform float4x4  _Azure_SunMatrix, _Azure_MoonMatrix, _Azure_UpDirectionMatrix;
            
			struct appdata
			{
				float4 vertex   : POSITION;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 Position        : SV_POSITION;
    			float2 uv 	           : TEXCOORD0;
				float4 interpolatedRay : TEXCOORD1;
				float2 uv_depth        : TEXCOORD2;
			};

			v2f vert (appdata v)
			{
				v2f Output;
    			UNITY_INITIALIZE_OUTPUT(v2f, Output);

				v.vertex.z = 0.1;
				Output.Position = UnityObjectToClipPos(v.vertex);
				Output.uv       = v.texcoord.xy;
				Output.uv_depth = v.texcoord.xy;
				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					Output.uv.y = 1 - Output.uv.y;
				#endif

				// Based on Unity5.6 GlobalFog
				int index = v.texcoord.x + (2.0 * Output.uv.y);
				Output.interpolatedRay   = _FrustumCorners[index];
				Output.interpolatedRay.xyz = mul((float3x3)_Azure_UpDirectionMatrix, Output.interpolatedRay.xyz);
				Output.interpolatedRay.w = index;

				return Output;
			}

			float4 frag (v2f Input) : SV_Target
			{
			    // Original scene
				float3 screen = tex2D(_MainTex, UnityStereoTransformScreenSpaceTex(Input.uv)).rgb;
			    
			    // Reconstruct world space position and direction towards this screen pixel
			    float depth = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture,UnityStereoTransformScreenSpaceTex(Input.uv_depth))));
			    if(depth == 1.0) return float4(screen, 1.0);
                
                // Initializations
			    float3 transmittance = float3(1.0, 1.0, 1.0);
			    
			    // Directions
				float3 viewDir = normalize(depth * Input.interpolatedRay.xyz);
				float  sunCosTheta = dot(viewDir, _Azure_SunDirection);
				float  moonCosTheta = dot(viewDir, _Azure_MoonDirection);
				float  r = length(float3(0.0, 50.0, 0.0));
				float  sunRise = saturate(dot(float3(0.0, 500.0, 0.0), _Azure_SunDirection) / r);
				float  moonRise = saturate(dot(float3(0.0, 500.0, 0.0), _Azure_MoonDirection) / r);
				
				// Optical depth
				//float zenith = acos(saturate(dot(float3(0.0, 1.0, 0.0), viewDir)));
				//float zenith = acos(length(viewDir.y));
				float zenith = acos(saturate(dot(float3(-1.0, 1.0, -1.0), depth))) * _Azure_FogScatteringScale;
				float z = (cos(zenith) + 0.15 * pow(93.885 - ((zenith * 180.0f) / PI), -1.253));
	            float SR = 8400.0 / z;
	            float SM = 1200.0 / z;
	            
	            // Extinction
                float3 fex = exp(-(_Azure_Br*SR  + _Azure_Bm*SM));
                float  sunset = clamp(dot(float3(0.0, 1.0, 0.0), _Azure_SunDirection), 0.0, 0.5);
				if(_Azure_StylizedTransmittanceMode == 0)
				{
				    transmittance = lerp(fex, (1.0 - fex), sunset);
				    _Azure_TransmittanceColor = float3(1.0, 1.0, 1.0);
				}
				
				// Sun inScattering
                float  rayPhase = 2.0 + 0.5 * pow(sunCosTheta, 2.0);
                float  miePhase = MieG.x / pow(MieG.y - MieG.z * sunCosTheta, 1.5);
                
                float3 BrTheta  = Pi316 * _Azure_Br * rayPhase * _Azure_RayleighColor;
                float3 BmTheta  = Pi14  * _Azure_Bm * miePhase * _Azure_MieColor * sunRise;
                float3 BrmTheta = (BrTheta + BmTheta) * transmittance / (_Azure_Br + _Azure_Bm);
                
                float3 inScatter = BrmTheta * _Azure_TransmittanceColor * _Azure_ScatteringIntensity * (1.0 - fex);
                inScatter *= sunRise;
                
                // Moon inScattering
                rayPhase = 2.0 + 0.5 * pow(moonCosTheta, 2.0);
                miePhase = MieG.x / pow(MieG.y - MieG.z * moonCosTheta, 1.5);
                
                //BrTheta  = Pi316 * _Azure_Br * rayPhase * _Azure_RayleighColor;
                BmTheta  = Pi14  * _Azure_Bm * miePhase * _Azure_MieColor * moonRise;
                BrmTheta = (BrTheta + BmTheta) / (_Azure_Br + _Azure_Bm);
                
                float3 moonInScatter = BrmTheta * _Azure_TransmittanceColor * _Azure_ScatteringIntensity * 0.1 * (1.0 - fex);
                moonInScatter *= moonRise;
                moonInScatter *= 1.0 - sunRise;
                
                // Default night sky - When there is no moon in the sky
                BrmTheta = BrTheta / (_Azure_Br + _Azure_Bm);
                float3 skyLuminance = BrmTheta * _Azure_TransmittanceColor * _Azure_SkyLuminance * (1.0 - fex);
                
                // Output
				float3 OutputColor = inScatter + skyLuminance + moonInScatter;
                
                // Tonemapping
                OutputColor = saturate(1.0 - exp(-_Azure_Exposure * OutputColor));
                
                // Color correction
				OutputColor = pow(OutputColor, 2.2);
			    #ifdef UNITY_COLORSPACE_GAMMA
			    OutputColor = pow(OutputColor, 0.4545);
				#else
				OutputColor = OutputColor;
    			#endif
			    
			    //return float4(OutputColor.rgb, 1.0);
			    
			    // Calcule fog distance
				float globalFog = smoothstep(-_Azure_GlobalFogSmooth, 1.25, depth * _ProjectionParams.z / _Azure_GlobalFogDistance) * _Azure_GlobalFogDensity;
				float heightFogDistance = smoothstep(-_Azure_HeightFogSmooth, 1.25, depth * _ProjectionParams.z / _Azure_HeightFogDistance);

				// Calcule height fog
				float3 worldSpaceDirection = mul((float3x3)_Azure_UpDirectionMatrix, _WorldSpaceCameraPos) + depth * Input.interpolatedRay.xyz;
				float heightFog = saturate((worldSpaceDirection.y - _Azure_HeightFogStart) / (_Azure_HeightFogEnd + _Azure_HeightFogStart));
	                  heightFog = 1.0 - heightFog;
	                  heightFog *= heightFog;
					  heightFog *= heightFogDistance;
					  heightFog *= _Azure_HeightFogDensity;
				float fog = saturate(globalFog + heightFog);

			//  Start: LuxWater
            	#if defined(LUXWATER_DEFERREDFOG)
					half4 fogMask = tex2D(_UnderWaterMask, UnityStereoTransformScreenSpaceTex(Input.uv));
					float watersurfacefrombelow = DecodeFloatRG(fogMask.ba);

				//	Get distance and lower it a bit in order to handle edge blending artifacts (edge blended parts would not get ANY fog)
					float dist = (watersurfacefrombelow - depth) + _LuxUnderWaterDeferredFogParams.y * _ProjectionParams.w;
				//	Fade fog from above water to below water
					float fogFactor = saturate ( 1.0 + _ProjectionParams.z * _LuxUnderWaterDeferredFogParams.z * dist ); // 0.125 
				//	Clamp above result to where water is actually rendered
					fogFactor = (fogMask.r == 1) ? fogFactor : 1.0;
				//  Mask fog on underwarter parts - only if we are inside a volume (bool... :( )
	                if(_LuxUnderWaterDeferredFogParams.x)
	                {
	                    fogFactor *= saturate( 1.0 - fogMask.g * 8.0);
	                    if (dist < -_ProjectionParams.w * 4 && fogMask.r == 0 && fogMask.g < 1.0)
	                    {
	                        fogFactor = 1.0;
	                    }
	                }
				//	Tweak fog factor
					fog *= fogFactor;
            	#endif
        	//  End: LuxWater 

				OutputColor.rgb = lerp(screen.rgb, OutputColor.rgb, fog);
				return float4(OutputColor.rgb, 1.0);
			}
			ENDCG
		}
	}
}