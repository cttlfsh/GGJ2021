#include "Assets/Azure[Sky] Dynamic Skybox/Shaders/Transparent/AzureComputeFogScattering.cginc"

// Apply fog scattering.
float4 ApplyAzureFog (float4 fragOutput, float3 worldPos)
{
    float3 fogScatteringColor = float3(0.0, 0.0, 0.0);
	#ifdef UNITY_PASS_FORWARDADD
		fogScatteringColor = float3(0.0, 0.0, 0.0);
	#else
	    if(_Azure_SkyModel == 0)
		    fogScatteringColor = AzureComputeStylizedFogScattering(worldPos);
	    else
	        fogScatteringColor = AzureComputePrecomputedFogScattering(worldPos, fragOutput);
	#endif

	// Calcule Standard Fog.
	float depth = distance(_WorldSpaceCameraPos, worldPos);
	float fog = smoothstep(-_Azure_GlobalFogSmooth, 1.25, depth / _Azure_GlobalFogDistance) * _Azure_GlobalFogDensity;
	float heightFogDistance = smoothstep(-_Azure_HeightFogSmooth, 1.25, depth / _Azure_HeightFogDistance);

	// Calcule Height Fog.
	float3 worldSpaceDirection = mul((float3x3)_Azure_UpDirectionMatrix, worldPos.xyz);
	float heightFog = saturate((worldSpaceDirection.y - _Azure_HeightFogStart) / (_Azure_HeightFogEnd + _Azure_HeightFogStart));
	heightFog = 1.0 - heightFog;
	heightFog *= heightFog;
	heightFog *= heightFogDistance;
	float fogFactor = saturate(fog + heightFog * _Azure_HeightFogDensity);

	// Apply Fog.
	#if defined(_ALPHAPREMULTIPLY_ON)
	fragOutput.a = lerp(fragOutput.a, 1.0, fogFactor);
	#endif
	fogScatteringColor = lerp(fragOutput.rgb, fogScatteringColor, fogFactor * lerp(fragOutput.a, 1.0, 2.0 - fogFactor));
	return float4(fogScatteringColor, fragOutput.a);
}

// Apply fog scattering to additive/multiply blend mode.
float4 ApplyAzureFog (float4 fragOutput, float3 worldPos, float4 fogColor)
{
    float3 fogScatteringColor = float3(0.0, 0.0, 0.0);
	#ifdef UNITY_PASS_FORWARDADD
		fogScatteringColor = float3(0.0, 0.0, 0.0);
	#else
		fogScatteringColor = fogColor;
	#endif

	// Calcule Standard Fog.
	float depth = distance(_WorldSpaceCameraPos, worldPos);
	float fog = smoothstep(-_Azure_GlobalFogSmooth, 1.25, depth / _Azure_GlobalFogDistance) * _Azure_GlobalFogDensity;
	float heightFogDistance = smoothstep(-_Azure_HeightFogSmooth, 1.25, depth / _Azure_HeightFogDistance);

	// Calcule Height Fog.
	float3 worldSpaceDirection = mul((float3x3)_Azure_UpDirectionMatrix, worldPos.xyz);
	float heightFog = saturate((worldSpaceDirection.y - _Azure_HeightFogStart) / (_Azure_HeightFogEnd + _Azure_HeightFogStart));
	heightFog = 1.0 - heightFog;
	heightFog *= heightFog;
	heightFog *= heightFogDistance;
	float fogFactor = saturate(fog + heightFog * _Azure_HeightFogDensity);

	// Apply Fog.
	fogScatteringColor = lerp(fragOutput.rgb, fogScatteringColor, fogFactor * lerp(fragOutput.a, 1.0, 2.0 - fogFactor));
	return float4(fogScatteringColor, fragOutput.a);
}

// DEPRECATED - backward compatibility (Actually, the projPos parameter is no longer needed.)
float4 ApplyAzureFog (float4 fragOutput, float4 projPos, float3 worldPos)
{
    float3 fogScatteringColor = float3(0.0, 0.0, 0.0);
	#ifdef UNITY_PASS_FORWARDADD
		fogScatteringColor = float3(0.0, 0.0, 0.0);
	#else
		if(_Azure_SkyModel == 0)
		    fogScatteringColor = AzureComputeStylizedFogScattering(worldPos);
	    else
	        fogScatteringColor = AzureComputePrecomputedFogScattering(worldPos, fragOutput);
	#endif

	// Calcule Standard Fog.
	float depth = distance(_WorldSpaceCameraPos, worldPos);
	float fog = smoothstep(-_Azure_GlobalFogSmooth, 1.25, depth / _Azure_GlobalFogDistance) * _Azure_GlobalFogDensity;
	float heightFogDistance = smoothstep(-_Azure_HeightFogSmooth, 1.25, depth / _Azure_HeightFogDistance);

	// Calcule Height Fog.
	float3 worldSpaceDirection = mul((float3x3)_Azure_UpDirectionMatrix, worldPos.xyz);
	float heightFog = saturate((worldSpaceDirection.y - _Azure_HeightFogStart) / (_Azure_HeightFogEnd + _Azure_HeightFogStart));
	heightFog = 1.0 - heightFog;
	heightFog *= heightFog;
	heightFog *= heightFogDistance;
	float fogFactor = saturate(fog + heightFog * _Azure_HeightFogDensity);

	// Apply Fog.
	fogScatteringColor = lerp(fragOutput.rgb, fogScatteringColor, fogFactor * lerp(fragOutput.a, 1.0, 2.0 - fogFactor));
	return float4(fogScatteringColor, fragOutput.a);
}