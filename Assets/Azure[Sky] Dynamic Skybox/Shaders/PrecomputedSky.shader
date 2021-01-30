/**
 * Precomputed Atmospheric Scattering
 * Copyright (c) 2008 INRIA
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * 3. Neither the name of the copyright holders nor the names of its
 *    contributors may be used to endorse or promote products derived from
 *    this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
 * THE POSSIBILITY OF SUCH DAMAGE.
 */

/**
 * Author: Eric Bruneton
 */

Shader "Azure[Sky]/Precomputed Sky"
{
	SubShader
	{
		Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" "IgnoreProjector"="True" }
	    Cull Back     // Render side
		Fog{Mode Off} // Don't use fog
    	ZWrite Off    // Don't draw to depth buffer

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			// Constants
			#define PI 3.1415926535
            static const float3 betaR = float3(5.8e-3, 1.35e-2, 3.31e-2);
            static const float mieG = 0.8;
            static const int RES_R = 32;
            static const int RES_MU = 128;
            static const int RES_MU_S = 32;
            static const int RES_NU = 8;
			
			// Inputs
			uniform sampler2D   _Azure_SunTexture, _Azure_MoonTexture, _Azure_TransmittanceTexture, _Azure_StarFieldTexture;
			uniform sampler3D   _Azure_InScatterTexture;
			uniform samplerCUBE _Azure_StarNoiseTexture;
			
			uniform float3    _Azure_SunDirection, _Azure_MoonDirection, _Azure_EarthPosition, _Azure_RayleighColor, _Azure_MieColor;
			uniform float     _Azure_PrecomputedSunIntensity, _Azure_PrecomputedMoonIntensity, _Azure_PrecomputedRg, _Azure_PrecomputedRt, _Azure_Exposure;
			uniform float     _Azure_SunTextureSize, _Azure_SunTextureIntensity, _Azure_MoonTextureSize, _Azure_MoonTextureIntensity;
			uniform float3    _Azure_SunTextureColor, _Azure_MoonTextureColor, _Azure_StarFieldColorBalance;
			uniform float     _Azure_RegularStarsScintillation, _Azure_RegularStarsIntensity, _Azure_MilkyWayIntensity;
			
			uniform float4x4  _Azure_SunMatrix, _Azure_MoonMatrix, _Azure_UpDirectionMatrix, _Azure_StarFieldMatrix, _Azure_NoiseRotationMatrix;

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 Position : SV_POSITION;
				float3 WorldPos : TEXCOORD0;
				float3 SunPos   : TEXCOORD1;
				float3 MoonPos  : TEXCOORD2;
				float3 StarPos  : TEXCOORD3;
				float3 NoiseRot : TEXCOORD4;
			};
			
			v2f vert (appdata v)
			{
				v2f Output;
				UNITY_INITIALIZE_OUTPUT(v2f, Output);

				Output.Position = UnityObjectToClipPos(v.vertex);
				Output.WorldPos = normalize(mul((float3x3)unity_WorldToObject, v.vertex.xyz));
				Output.WorldPos = normalize(mul((float3x3)_Azure_UpDirectionMatrix, Output.WorldPos));
				
				Output.SunPos = mul((float3x3)_Azure_SunMatrix, v.vertex.xyz) * _Azure_SunTextureSize;
				Output.StarPos  = mul((float3x3)_Azure_StarFieldMatrix, Output.WorldPos);
				Output.NoiseRot = mul((float3x3)_Azure_NoiseRotationMatrix, v.vertex.xyz);
				Output.MoonPos = mul((float3x3)_Azure_MoonMatrix, v.vertex.xyz) * 0.75 * _Azure_MoonTextureSize;
				Output.MoonPos.x *= -1.0;
				
				return Output;
			}
			
			bool iSphere(in float3 origin, in float3 direction, in float3 position, in float radius, out float3 normalDirection)
			{
				float3 rc = origin - position;
				float c = dot(rc, rc) - (radius * radius);
				float b = dot(direction, rc);
				float d = b * b - c;
				float t = -b - sqrt(abs(d));
				float st = step(0.0, min(t, d));
				normalDirection = normalize(-position + (origin + direction * t));

				if (st > 0.0) { return true; }
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
            
            //float3 outerSunRadiance(float3 sundir)
            //{
            //    float3 data = sundir.z > 0.0 ? tex2D(_Azure_SunTexture, float2(0.5, 0.5) + sundir.xy * 4.0).rgb : float3(0.0, 0.0, 0.0);
            //    return pow(data, float3(2.2, 2.2, 2.2)) * _Azure_PrecomputedSunIntensity;
            //}
			
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
			    // Initializations
			    float3 transmittance = float3(1.0, 1.0, 1.0);
			    
			    // Directions
				float3 viewDir = normalize(Input.WorldPos);
				float  sunCosTheta = dot(viewDir, _Azure_SunDirection);
				float  moonCosTheta = dot(viewDir, _Azure_MoonDirection);

	            // Extinction
                float3 extinction = float3(1.0, 1.0, 1.0);
                
                // Sun inScattering
                float3 inScatter = skyRadiance(_WorldSpaceCameraPos, viewDir, _Azure_SunDirection, extinction) * _Azure_PrecomputedSunIntensity;

                float moonExtinction = saturate(extinction.b * 0.5);
                float nightTransition = saturate(1.0 - inScatter.r);
                
                // Moon inScattering
                float3 moonInScatter = skyRadiance(_WorldSpaceCameraPos, viewDir, _Azure_MoonDirection, extinction) * _Azure_PrecomputedMoonIntensity * nightTransition;
                
                float cameraHeight = distance(_WorldSpaceCameraPos, _Azure_EarthPosition * -1.0);
                float atmosphereHeight = _Azure_PrecomputedRt - _Azure_PrecomputedRg;
                float starsFactor = saturate(inverseLerp(_Azure_PrecomputedRg + (atmosphereHeight * 0.05), _Azure_PrecomputedRt, cameraHeight));
                _Azure_RegularStarsIntensity = lerp(_Azure_RegularStarsIntensity, 1.0, starsFactor) * saturate(extinction.b * 0.5);
                _Azure_MilkyWayIntensity = lerp(_Azure_MilkyWayIntensity, 1.0, starsFactor) * saturate(extinction.b * 0.5);
                
                // Sun texture
                float3 sunTexture = tex2D( _Azure_SunTexture, Input.SunPos + 0.5).rgb * _Azure_SunTextureColor * _Azure_SunTextureIntensity;
					   sunTexture = pow(sunTexture, 2.0);
					   sunTexture *= extinction.b * saturate(sunCosTheta);
			    
			    // Moon sphere
				float3 rayOrigin = float3(0.0, 0.0, 0.0);//_WorldSpaceCameraPos;
				float3 rayDirection = viewDir;
				float3 moonPosition = _Azure_MoonDirection * 38400.0 * _Azure_MoonTextureSize;
				float3 normalDirection = float3(0.0, 0.0, 0.0);
				float3 moonColor = float3(0.0, 0.0, 0.0);
				float4 moonTexture = saturate(tex2D( _Azure_MoonTexture, Input.MoonPos.xy + 0.5) * moonCosTheta);
				float moonMask = 1.0 - moonTexture.a;
				if(iSphere(rayOrigin, rayDirection, moonPosition, 17370.0, normalDirection))
				{
					float moonSphere = max(dot(normalDirection, _Azure_SunDirection), 0.0) * moonTexture.a * 2.0;
					moonColor = moonTexture.rgb * moonSphere * _Azure_MoonTextureColor * _Azure_MoonTextureIntensity * moonExtinction;
				}
				
				// Starfield
				float2 stars_uv = float2(-atan2(Input.StarPos.z, Input.StarPos.x), -acos(Input.StarPos.y)) / float2(2.0 * PI, PI);
				float scintillation = texCUBE(_Azure_StarNoiseTexture, Input.NoiseRot).r * 1.5;
				float4 starTexture   = tex2D(_Azure_StarFieldTexture, stars_uv);
				float3 stars     = starTexture.rgb * pow(starTexture.a, 2.0) * _Azure_RegularStarsIntensity * scintillation;
				float3 milkyWay  = (pow(starTexture.rgb, 1.5)) * _Azure_MilkyWayIntensity;
				float3 starfield = (stars + milkyWay) * _Azure_StarFieldColorBalance * moonMask;
                
                // Output
				float3 OutputColor = inScatter + moonInScatter + sunTexture + moonColor + starfield;
				
				// Tonemapping
                OutputColor = saturate(1.0 - exp(-_Azure_Exposure * OutputColor));
				
				// Color correction
				//OutputColor = pow(OutputColor, 2.2);
			    #ifdef UNITY_COLORSPACE_GAMMA
			    OutputColor = pow(OutputColor, 0.4545);
				#else
				OutputColor = OutputColor;
    			#endif
				
				return float4(OutputColor, 1.0);
			}
			ENDCG
		}
	}
}