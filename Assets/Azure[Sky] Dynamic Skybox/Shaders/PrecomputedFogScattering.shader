Shader "Azure[Sky]/Precomputed Fog Scattering"
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
			#define HORIZON_HACK
            static const float3 betaR = float3(5.8e-3, 1.35e-2, 3.31e-2);
            static const float mieG = 0.8;
            static const int RES_R = 32;
            static const int RES_MU = 128;
            static const int RES_MU_S = 32;
            static const int RES_NU = 8;
            
            // Inputs
            uniform sampler2D _MainTex, _Azure_TransmittanceTexture;
			uniform sampler2D_float _CameraDepthTexture;
			uniform sampler3D _Azure_InScatterTexture;
			uniform float4x4  _FrustumCorners;
			uniform float4    _MainTex_TexelSize;
			
			uniform float3    _Azure_SunDirection, _Azure_MoonDirection, _Azure_EarthPosition, _Azure_RayleighColor, _Azure_MieColor;
			uniform float     _Azure_PrecomputedSunIntensity, _Azure_PrecomputedMoonIntensity, _Azure_PrecomputedRg, _Azure_PrecomputedRt, _Azure_Exposure;
			uniform float     _Azure_GlobalFogSmooth, _Azure_GlobalFogDistance, _Azure_GlobalFogDensity;
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
			
			bool iSphere(in float3 origin, in float3 direction, in float3 position, in float radius, out float2 uv, out float3 normalDirection)
            {
                float3 rc = origin - position;
                float c = dot(rc, rc) - (radius * radius);
                float b = dot(direction, rc);
                float d = b * b - c;
                float t = -b - sqrt(abs(d));
                float st = step(0.0, min(t, d));
                
                // Output normal direction and uv
                normalDirection = normalize(-position + (origin + direction * t));
                float theta = (atan2(normalDirection.z, normalDirection.x) + 1.5) / (PI * 2.0);
                float phi = acos(normalDirection.y ) / PI;
                uv = float2(theta, phi);
            
                if (st > 0.0)
                {
                    return true;
                }
                return false;
            }
			
			float SQRT(float f, float err)
			{
                return f >= 0.0 ? sqrt(f) : err;
            }
            
            float inverseLerp(float min, float max, float t)
			{
			    return (t - min) / (max - min);
			}
			
			float4 texture4D(sampler3D table, float r, float mu, float muS, float nu)
            {
                float H = sqrt(_Azure_PrecomputedRt * _Azure_PrecomputedRt - _Azure_PrecomputedRg * _Azure_PrecomputedRg);
                float rho = sqrt(r * r - _Azure_PrecomputedRg * _Azure_PrecomputedRg);
                float rmu = r * mu;
                float delta = rmu * rmu - r * r + _Azure_PrecomputedRg * _Azure_PrecomputedRg;
                float4 cst = rmu < 0.0 && delta > 0.0 ? float4(1.0, 0.0, 0.0, 0.5 - 0.5 / float(RES_MU)) : float4(-1.0, H * H, H, 0.5 + 0.5 / float(RES_MU));
                float uR = 0.5 / float(RES_R) + rho / H * (1.0 - 1.0 / float(RES_R));
                float uMu = cst.w + (rmu * cst.x + sqrt(delta + cst.y)) / (rho + cst.z) * (0.5 - 1.0 / float(RES_MU));
                // paper formula
                //float uMuS = 0.5 / float(RES_MU_S) + max((1.0 - exp(-3.0 * muS - 0.6)) / (1.0 - exp(-3.6)), 0.0) * (1.0 - 1.0 / float(RES_MU_S));
                // better formula
                float uMuS = 0.5 / float(RES_MU_S) + (atan(max(muS, -0.1975) * tan(1.26 * 1.1)) / 1.1 + (1.0 - 0.26)) * 0.5 * (1.0 - 1.0 / float(RES_MU_S));
                float _lerp = (nu + 1.0) / 2.0 * (float(RES_NU) - 1.0);
                float uNu = floor(_lerp);
                _lerp = _lerp - uNu;
                return tex3D(table, float3((uNu + uMuS) / float(RES_NU), uMu, uR)) * (1.0 - _lerp) +
                       tex3D(table, float3((uNu + uMuS + 1.0) / float(RES_NU), uMu, uR)) * _lerp;
            }
            
            // Rayleigh phase function
            float phaseFunctionR(float mu)
            {
                return (3.0 / (16.0 * PI)) * (1.0 + mu * mu);
            }
            
            // Mie phase function
            float phaseFunctionM(float mu)
            {
                return 1.5 * 1.0 / (4.0 * PI) * (1.0 - mieG*mieG) * pow(1.0 + (mieG*mieG) - 2.0*mieG*mu, -3.0/2.0) * (1.0 + mu * mu) / (2.0 + mieG*mieG);
            }
            
            // Approximated single Mie scattering (cf. approximate Cm in paragraph "Angular precision")
            float3 getMie(float4 rayMie)
            { 
                // rayMie.rgb=C*, rayMie.w=Cm,r
                return rayMie.rgb * rayMie.w / max(rayMie.r, 1e-4) * (betaR.r / betaR);
            }
            
            // Transmittance(=transparency) of atmosphere for infinite ray (r,mu)
            // (mu=cos(view zenith angle)), intersections with ground ignored
            float3 transmittance(float r, float mu)
            {
                float uR, uMu;
                uR = sqrt((r - _Azure_PrecomputedRg) / (_Azure_PrecomputedRt - _Azure_PrecomputedRg));
                uMu = atan((mu + 0.15) / (1.0 + 0.15) * tan(1.5)) / 1.5;
                
                return tex2D(_Azure_TransmittanceTexture, float2(uMu, uR)).rgb;
            }
            
            // single scattered sunlight between two points
            // camera=observer
            // point=point on the ground
            // sundir=unit vector towards the sun
            // return scattered light and extinction coefficient
            float3 inScattering(float3 camera, float3 _point, float3 sundir, out float3 extinction)
            {
                float3 result;
                float3 viewdir = _point - camera;
                float d = length(viewdir);
                viewdir = viewdir / d;
                float r = length(camera);
                
                if (r < 0.9 * _Azure_PrecomputedRg)
                {
                    camera += _Azure_EarthPosition;
                    _point += _Azure_EarthPosition;
                    r = length(camera);
                }
                float rMu = dot(camera, viewdir);
                float mu = rMu / r;
                float r0 = r;
                float mu0 = mu;
            
                float deltaSq = SQRT(rMu * rMu - r * r + _Azure_PrecomputedRt*_Azure_PrecomputedRt, 1e30);
                float din = max(-rMu - deltaSq, 0.0);
                if (din > 0.0 && din < d)
                {
                    camera += din * viewdir;
                    rMu += din;
                    mu = rMu / _Azure_PrecomputedRt;
                    r = _Azure_PrecomputedRt;
                    d -= din;
                }
            
                if (r <= _Azure_PrecomputedRt)
                {
                    float nu = dot(viewdir, sundir);
                    float muS = dot(camera, sundir) / r;
            
                    float4 inScatter;
            
                    //if (r < _Azure_PrecomputedRg + 600.0)
                    //{
                    //    // avoids imprecision problems in aerial perspective near ground
                    //    float f = (_Azure_PrecomputedRg + 600.0) / r;
                    //    r = r * f;
                    //    rMu = rMu * f;
                    //    _point = _point * f;
                    //}
            
                    float r1 = length(_point);
                    float rMu1 = dot(_point, viewdir);
                    float mu1 = rMu1 / r1;
                    float muS1 = dot(_point, sundir) / r1;

                    if (mu > 0.0)
                    {
                        extinction = min(transmittance(r, mu) / transmittance(r1, mu1), 1.0);
                    }
                    else
                    {
                        extinction = min(transmittance(r1, -mu1) / transmittance(r, -mu), 1.0);
                    }
            
                    #ifdef HORIZON_HACK
                    const float EPS = 0.004;
                    float lim = -sqrt(1.0 - (_Azure_PrecomputedRg / r) * (_Azure_PrecomputedRg / r));
                    if (abs(mu - lim) < EPS)
                    {
                        float a = ((mu - lim) + EPS) / (2.0 * EPS);
            
                        mu = lim - EPS;
                        r1 = sqrt(r * r + d * d + 2.0 * r * d * mu);
                        mu1 = (r * mu + d) / r1;
                        float4 inScatter0 = texture4D(_Azure_InScatterTexture, r, mu, muS, nu);
                        float4 inScatter1 = texture4D(_Azure_InScatterTexture, r1, mu1, muS1, nu);
                        float4 inScatterA = max(inScatter0 - inScatter1 * extinction.rgbr, 0.0);
            
                        mu = lim + EPS;
                        r1 = sqrt(r * r + d * d + 2.0 * r * d * mu);
                        mu1 = (r * mu + d) / r1;
                        inScatter0 = texture4D(_Azure_InScatterTexture, r, mu, muS, nu);
                        inScatter1 = texture4D(_Azure_InScatterTexture, r1, mu1, muS1, nu);
                        float4 inScatterB = max(inScatter0 - inScatter1 * extinction.rgbr, 0.0);
            
                        inScatter = lerp(inScatterA, inScatterB, a);
                    }
                    else
                    {
                        float4 inScatter0 = texture4D(_Azure_InScatterTexture, r, mu, muS, nu);
                        float4 inScatter1 = texture4D(_Azure_InScatterTexture, r1, mu1, muS1, nu);
                        inScatter = max(inScatter0 - inScatter1 * extinction.rgbr, 0.0);
                    }
                    #else
                    float4 inScatter0 = texture4D(_Azure_InScatterTexture, r, mu, muS, nu);
                    float4 inScatter1 = texture4D(_Azure_InScatterTexture, r1, mu1, muS1, nu);
                    inScatter = max(inScatter0 - inScatter1 * extinction.rgbr, 0.0);
                    #endif
            
                    //cancels inscatter when sun hidden by mountains
                    //TODO smoothstep values depend on horizon angle in sun direction
                    //inScatter.w *= smoothstep(0.035, 0.07, muS);
            
                    // avoids imprecision problems in Mie scattering when sun is below horizon
                    inScatter.w *= smoothstep(0.00, 0.02, muS);
            
                    float3 inScatterM = getMie(inScatter);
                    float phase = phaseFunctionR(nu);
                    float phaseM = phaseFunctionM(nu);
                    result = inScatter.rgb * _Azure_RayleighColor * phase + inScatterM * _Azure_MieColor * phaseM;
                }
                else
                {
                    result = float3(0.0, 0.0, 0.0);
                    extinction = float3(1.0, 1.0, 1.0);
                }
            
                return result;
            }
            
            // single scattered sunlight between two points
            // camera=observer
            // viewdir=unit vector towards observed point
            // sundir=unit vector towards the sun
            // return scattered light and extinction coefficient
            float3 skyRadiance(float3 camera, float3 viewdir, float3 sundir, out float3 extinction)
            {
                float3 result;
                camera += _Azure_EarthPosition;
                float r = length(camera);
                float rMu = dot(camera, viewdir);
                float mu = rMu / r;
                float r0 = r;
                float mu0 = mu;
            
                float deltaSq = SQRT(rMu * rMu - r * r + _Azure_PrecomputedRt*_Azure_PrecomputedRt, 1e30);
                float din = max(-rMu - deltaSq, 0.0);
                if (din > 0.0)
                {
                    camera += din * viewdir;
                    rMu += din;
                    mu = rMu / _Azure_PrecomputedRt;
                    r = _Azure_PrecomputedRt;
                }
            
                if (r <= _Azure_PrecomputedRt)
                {
                    float nu = dot(viewdir, sundir);
                    float muS = dot(camera, sundir) / r;
            
                    float4 inScatter = texture4D(_Azure_InScatterTexture, r, rMu / r, muS, nu);
                    inScatter *= min(transmittance(r, -mu) / transmittance(r0, -mu0), 1.0).rgbr;
                    extinction = transmittance(r, mu);
            
                    float3 inScatterM = getMie(inScatter);
                    float phase = phaseFunctionR(nu);
                    float phaseM = phaseFunctionM(nu);
                    result = inScatter.rgb * _Azure_RayleighColor * phase + inScatterM * _Azure_MieColor * phaseM;
                } else
                {
                    result = float3(0.0, 0.0, 0.0);
                    extinction = float3(1.0, 1.0, 1.0);
                }
            
                return result;
            }

			float4 frag (v2f Input) : SV_Target
			{
			    // Original scene
				float3 screen = tex2D(_MainTex, UnityStereoTransformScreenSpaceTex(Input.uv)).rgb;
				float3 extinction = float3(0.0, 0.0, 0.0);
			    
			    // Reconstruct world space position and direction towards this screen pixel
			    float depth = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture,UnityStereoTransformScreenSpaceTex(Input.uv_depth))));
			    if(depth >= 1.0) return float4(screen, 1.0);
			    
			    float3 worldPos = Input.interpolatedRay.xyz * depth + _WorldSpaceCameraPos;
			    
			    //float3 viewDir = normalize(worldPos);
			    float3 viewDir = normalize(worldPos - _WorldSpaceCameraPos);
			    
			    //worldPos += _WorldSpaceCameraPos;
				//float d3 = distance(worldPos, _Azure_EarthPosition * -1.0);
                //if(dot(_WorldSpaceCameraPos, worldPos) < 0 && d3 >= _Azure_PrecomputedRt)
				//{
				    //viewDir = normalize(worldPos - _WorldSpaceCameraPos);
				//}
				
				//float height = distance(worldPos, _Azure_EarthPosition * -1.0);
                //float atmFactor = saturate(inverseLerp(_Azure_PrecomputedRg, _Azure_PrecomputedRt, height));
                float cameraHeight = distance(_WorldSpaceCameraPos, _Azure_EarthPosition * -1.0);
                float atmosphereHeight = _Azure_PrecomputedRt - _Azure_PrecomputedRg;
                float atmFactor = saturate(inverseLerp(_Azure_PrecomputedRg + (atmosphereHeight * 0.5), _Azure_PrecomputedRt, cameraHeight));
			    
			    // InScattering:
				float3 sunInScatter = inScattering(_WorldSpaceCameraPos, worldPos, _Azure_SunDirection, extinction) * _Azure_PrecomputedSunIntensity;
				float3 moonInScatter = inScattering(_WorldSpaceCameraPos, worldPos, _Azure_MoonDirection, extinction) * _Azure_PrecomputedMoonIntensity;
	
				// Sky Radiance:
				float3 sunRadiance = skyRadiance(_WorldSpaceCameraPos, viewDir, _Azure_SunDirection, extinction) * _Azure_PrecomputedSunIntensity;
				float3 moonRadiance = skyRadiance(_WorldSpaceCameraPos, viewDir, _Azure_MoonDirection, extinction) * _Azure_PrecomputedMoonIntensity;
				
                // Output
				//float3 OutputColor = screen.rgb * extinction + lerp(sunInScatter + moonInScatter, sunRadiance + moonRadiance, atmFactor);
				float3 OutputColor = screen.rgb * extinction + lerp(sunInScatter + moonInScatter, sunRadiance + moonRadiance, atmFactor);
				
				// Tonemapping
                OutputColor = saturate(1.0 - exp(-_Azure_Exposure * OutputColor));
				
				// Color correction
				//OutputColor = pow(OutputColor, 2.2);
			    #ifdef UNITY_COLORSPACE_GAMMA
			    OutputColor = pow(OutputColor, 0.4545);
				#else
				OutputColor = OutputColor;
    			#endif
			    
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