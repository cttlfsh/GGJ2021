using System;
using System.Collections.Generic;

namespace UnityEngine.AzureSky
{
    [Serializable]
    public struct WeatherProfile
    {
        public AzureSkyProfile profile;
        public float transitionTime;
    }
    
    [Serializable]
    public sealed class AzureProfileController
    {
        // Local weather zones
        public Transform weatherZoneTrigger;
        private Vector3 m_weatherZoneTriggerPosition;
        private Vector3 m_weatherZoneClosestPoint;
        private float m_weatherZoneClosestDistanceSqr;
        private float m_weatherZoneDistance;
        private float m_weatherZoneBlendDistanceSqr;
        private float m_weatherZoneInterpolationFactor;
        private Collider m_weatherZoneCollider;
        
        // Profiles
        public AzureSkyProfile defaultProfile;
        public AzureSkyProfile currentProfile;
        public AzureSkyProfile targetProfile;
        public float defaultProfileTransitionTime = 10.0f;
        
        // Lists
        public List<WeatherProfile> weatherProfileList = new List<WeatherProfile>();
        public List<AzureSkyProfile> defaultProfileList = new List<AzureSkyProfile>();
        public List<AzureSkyWeatherZone> weatherZoneList = new List<AzureSkyWeatherZone>();
        
        // Output computation
        public List<AzureOutput> outputSystemList = new List<AzureOutput>();
        public string outputTag = "Untagged";

        // Global weather transition
        public bool isWeatherTransition = false;
        public float transitionTime = 0.0f;
        public float startTransitionTime = 0.0f;
        public float transitionProgress = 0.0f;
        public Texture2D staticCloudSource;
        public Texture2D staticCloudTarget;
        
        // Sky settings
        private AzureSkySettings m_settings = new AzureSkySettings();
        public float evaluateCurveTimeline;
        public float evaluateCurveSunElevation;
        public float evaluateCurveMoonElevation;
        public float evaluateGradientTimeline;
        public float evaluateGradientSunElevation;
        public float evaluateGradientMoonElevation;

        /// <summary>
        /// Gets the current sky setting if there is no global weather transition or local weather zone influence.
        /// </summary>
        /// Computes the global weather transition.
        /// <summary>
        /// Computes local weather zones influence.
        /// </summary>
        /// Returns the final sky setting resulting from the profile blending process.
        public AzureSkySettings GetProfileSettings()
        {
            if (!isWeatherTransition)
            {
                // Gets the current sky setting when there is no global weather transition or local weather zone influence
                ComputeDefaultSettings();
            }
            else
            {
                // Runs the global weather transition
                transitionProgress = Mathf.Clamp01((Time.time - startTransitionTime) / transitionTime);

                // Blends the profiles
                ComputeWeatherTransition(currentProfile, targetProfile, transitionProgress);
                
                // Ends the global weather transition
                if (Math.Abs(transitionProgress - 1.0f) <= 0.0f)
                {
                    isWeatherTransition = false;
                    transitionProgress = 0.0f;
                    startTransitionTime = 0.0f;
                    currentProfile = targetProfile;
                }
            }

            // Computes weather zones influence
            // Based on Unity's Post Processing v2
            if (!weatherZoneTrigger)
                return m_settings;
            
            m_weatherZoneTriggerPosition = weatherZoneTrigger.position;
                
            // Traverse all weather zones in the weather zone list
            foreach (var weatherZone in weatherZoneList)
            {
                // Skip if the list index is null
                if (weatherZone == null)
                    continue;
                    
                // If weather zone has no collider, skip it as it's useless
                m_weatherZoneCollider = weatherZone.GetComponent<Collider>();
                if (!m_weatherZoneCollider)
                    continue;
                    
                if (!m_weatherZoneCollider.enabled)
                    continue;
                    
                // Find closest distance to weather zone, 0 means it's inside it
                m_weatherZoneClosestDistanceSqr = float.PositiveInfinity;

                m_weatherZoneClosestPoint = m_weatherZoneCollider.ClosestPoint(m_weatherZoneTriggerPosition); // 5.6-only API
                m_weatherZoneDistance = ((m_weatherZoneClosestPoint - m_weatherZoneTriggerPosition) / 2f).sqrMagnitude;

                if (m_weatherZoneDistance < m_weatherZoneClosestDistanceSqr)
                    m_weatherZoneClosestDistanceSqr = m_weatherZoneDistance;

                m_weatherZoneCollider = null;
                m_weatherZoneBlendDistanceSqr = weatherZone.blendDistance * weatherZone.blendDistance;
                    
                // Weather zone has no influence, ignore it
                // Note: Weather zone doesn't do anything when `closestDistanceSqr = blendDistSqr` but
                //       we can't use a >= comparison as blendDistSqr could be set to 0 in which
                //       case weather zone would have total influence
                if (m_weatherZoneClosestDistanceSqr > m_weatherZoneBlendDistanceSqr)
                    continue;
                    
                // Weather zone has influence
                m_weatherZoneInterpolationFactor = 1f;

                if (m_weatherZoneBlendDistanceSqr > 0f)
                    m_weatherZoneInterpolationFactor = 1f - (m_weatherZoneClosestDistanceSqr / m_weatherZoneBlendDistanceSqr);

                // No need to clamp01 the interpolation factor as it'll always be in [0;1[ range
                BlendWeatherZones(weatherZone.profile, m_weatherZoneInterpolationFactor);
            }

            return m_settings;
        }

        /// <summary>
        /// Gets the current sky setting when there is no global weather transition or local weather zone influence.
        /// </summary>
        private void ComputeDefaultSettings()
        {
            // Scattering
            m_settings.MolecularDensity = GetFloatPropertyValue(currentProfile.scattering.molecularDensity);
            m_settings.Wavelength.x = GetFloatPropertyValue(currentProfile.scattering.wavelengthR);
            m_settings.Wavelength.y = GetFloatPropertyValue(currentProfile.scattering.wavelengthG);
            m_settings.Wavelength.z = GetFloatPropertyValue(currentProfile.scattering.wavelengthB);
            m_settings.RayleighMultiplier = GetFloatPropertyValue(currentProfile.scattering.rayleighMultiplier);
            m_settings.MieMultiplier = GetFloatPropertyValue(currentProfile.scattering.mieMultiplier);
            m_settings.ScatteringIntensity = GetFloatPropertyValue(currentProfile.scattering.scatteringIntensity);
            m_settings.SkyLuminance = GetFloatPropertyValue(currentProfile.scattering.skyLuminance);
            m_settings.RayleighColor = GetColorPropertyValue(currentProfile.scattering.rayleighColor);
            m_settings.MieColor = GetColorPropertyValue(currentProfile.scattering.mieColor);
            m_settings.TransmittanceColor = GetColorPropertyValue(currentProfile.scattering.sunsetColor);
            m_settings.PrecomputedSunIntensity = GetFloatPropertyValue(currentProfile.scattering.precomputedSunIntensity);
            m_settings.PrecomputedMoonIntensity = GetFloatPropertyValue(currentProfile.scattering.precomputedMoonIntensity);
            m_settings.Exposure = GetFloatPropertyValue(currentProfile.scattering.exposure);
            
            // Outer space
            m_settings.SunTextureSize = GetFloatPropertyValue(currentProfile.outerSpace.sunTextureSize);
            m_settings.SunTextureIntensity = GetFloatPropertyValue(currentProfile.outerSpace.sunTextureIntensity);
            m_settings.SunTextureColor = GetColorPropertyValue(currentProfile.outerSpace.sunTextureColor);
            m_settings.MoonTextureSize = GetFloatPropertyValue(currentProfile.outerSpace.moonTextureSize);
            m_settings.MoonTextureIntensity = GetFloatPropertyValue(currentProfile.outerSpace.moonTextureIntensity);
            m_settings.MoonTextureColor = GetColorPropertyValue(currentProfile.outerSpace.moonTextureColor);
            m_settings.RegularStarsScintillation = GetFloatPropertyValue(currentProfile.outerSpace.regularStarsScintillation);
            m_settings.RegularStarsIntensity = GetFloatPropertyValue(currentProfile.outerSpace.regularStarsIntensity);
            m_settings.MilkyWayIntensity = GetFloatPropertyValue(currentProfile.outerSpace.milkyWayIntensity);
            
            // Fog scattering
            m_settings.FogScatteringScale = GetFloatPropertyValue(currentProfile.fogScattering.fogScatteringScale);
            m_settings.GlobalFogSmooth = GetFloatPropertyValue(currentProfile.fogScattering.globalFogSmooth);
            m_settings.GlobalFogDensity = GetFloatPropertyValue(currentProfile.fogScattering.globalFogDensity);
            m_settings.GlobalFogDistance = GetFloatPropertyValue(currentProfile.fogScattering.globalFogDistance);
            m_settings.HeightFogSmooth = GetFloatPropertyValue(currentProfile.fogScattering.heightFogSmooth);
            m_settings.HeightFogDensity = GetFloatPropertyValue(currentProfile.fogScattering.heightFogDensity);
            m_settings.HeightFogDistance = GetFloatPropertyValue(currentProfile.fogScattering.heightFogDistance);
            m_settings.HeightFogStart = GetFloatPropertyValue(currentProfile.fogScattering.heightFogStart);
            m_settings.HeightFogEnd = GetFloatPropertyValue(currentProfile.fogScattering.heightFogEnd);

            // Clouds
            m_settings.StaticCloudInterpolator = 0.0f;
            staticCloudSource = currentProfile.clouds.staticCloudTexture;
            staticCloudTarget = currentProfile.clouds.staticCloudTexture;
            m_settings.StaticCloudLayer1Speed = GetFloatPropertyValue(currentProfile.clouds.staticCloudLayer1Speed);
            m_settings.StaticCloudLayer2Speed = GetFloatPropertyValue(currentProfile.clouds.staticCloudLayer2Speed);
            m_settings.StaticCloudColor = GetColorPropertyValue(currentProfile.clouds.staticCloudColor);
            m_settings.StaticCloudScattering = GetFloatPropertyValue(currentProfile.clouds.staticCloudScattering);
            m_settings.StaticCloudExtinction = GetFloatPropertyValue(currentProfile.clouds.staticCloudExtinction);
            m_settings.StaticCloudSaturation = GetFloatPropertyValue(currentProfile.clouds.staticCloudSaturation);
            m_settings.StaticCloudOpacity = GetFloatPropertyValue(currentProfile.clouds.staticCloudOpacity);
            m_settings.DynamicCloudAltitude = GetFloatPropertyValue(currentProfile.clouds.dynamicCloudAltitude);
            m_settings.DynamicCloudDirection = GetFloatPropertyValue(currentProfile.clouds.dynamicCloudDirection);
            m_settings.DynamicCloudSpeed = GetFloatPropertyValue(currentProfile.clouds.dynamicCloudSpeed);
            m_settings.DynamicCloudDensity = GetFloatPropertyValue(currentProfile.clouds.dynamicCloudDensity);
            m_settings.DynamicCloudColor1 = GetColorPropertyValue(currentProfile.clouds.dynamicCloudColor1);
            m_settings.DynamicCloudColor2 = GetColorPropertyValue(currentProfile.clouds.dynamicCloudColor2);

            // Lighting
            m_settings.DirectionalLightIntensity = GetFloatPropertyValue(currentProfile.lighting.directionalLightIntensity);
            m_settings.DirectionalLightColor = GetColorPropertyValue(currentProfile.lighting.directionalLightColor);
            m_settings.AmbientIntensity = GetFloatPropertyValue(currentProfile.lighting.ambientIntensity);
            m_settings.AmbientColor = GetColorPropertyValue(currentProfile.lighting.ambientColor);
            m_settings.AmbientSkyColor = GetColorPropertyValue(currentProfile.lighting.ambientSkyColor);
            m_settings.AmbientEquatorColor = GetColorPropertyValue(currentProfile.lighting.ambientEquatorColor);
            m_settings.AmbientGroundColor = GetColorPropertyValue(currentProfile.lighting.ambientGroundColor);
            m_settings.SkyReflectionIntensity = GetFloatPropertyValue(currentProfile.lighting.skyReflectionIntensity);
            
            // Weather
            m_settings.RainLightIntensity = GetFloatPropertyValue(currentProfile.weather.rainLightIntensity);
            m_settings.RainMediumIntensity = GetFloatPropertyValue(currentProfile.weather.rainMediumIntensity);
            m_settings.RainHeavyIntensity = GetFloatPropertyValue(currentProfile.weather.rainHeavyIntensity);
            m_settings.SnowIntensity = GetFloatPropertyValue(currentProfile.weather.snowIntensity);
            m_settings.RainLightVolume = GetFloatPropertyValue(currentProfile.weather.rainLightVolume);
            m_settings.RainMediumVolume = GetFloatPropertyValue(currentProfile.weather.rainMediumVolume);
            m_settings.RainHeavyVolume = GetFloatPropertyValue(currentProfile.weather.rainHeavyVolume);
            m_settings.WindLightVolume = GetFloatPropertyValue(currentProfile.weather.windLightVolume);
            m_settings.WindMediumVolume = GetFloatPropertyValue(currentProfile.weather.windMediumVolume);
            m_settings.WindHeavyVolume = GetFloatPropertyValue(currentProfile.weather.windHeavyVolume);
            m_settings.RainColor = GetColorPropertyValue(currentProfile.weather.rainColor);
            m_settings.SnowColor = GetColorPropertyValue(currentProfile.weather.snowColor);
            m_settings.WindSpeed = GetFloatPropertyValue(currentProfile.weather.windSpeed);
            m_settings.WindDirection = GetFloatPropertyValue(currentProfile.weather.windDirection);
            m_settings.ThunderMultiplier = GetFloatPropertyValue(currentProfile.weather.thunderMultiplier);
            
            // Outputs
            if (outputSystemList.Count > 0 && outputTag == currentProfile.outputTag && currentProfile.outputsList.Count == outputSystemList.Count)
            {
                for (int i = 0; i < outputSystemList.Count; i++)
                {
                    switch (outputSystemList[i].azureOutputType)
                    {
                        case AzureOutputType.Slider:
                            outputSystemList[i].floatOutput = currentProfile.outputsList[i].slider;
                            break;
                        case AzureOutputType.Vector2:
                            outputSystemList[i].vector2Output = currentProfile.outputsList[i].vector2;
                            break;
                        case AzureOutputType.Vector3:
                            outputSystemList[i].vector3Output = currentProfile.outputsList[i].vector3;
                            break;
                        case AzureOutputType.CurveTimeline:
                            outputSystemList[i].floatOutput = currentProfile.outputsList[i].timelineCurve.Evaluate(evaluateCurveTimeline);
                            break;
                        case AzureOutputType.CurveSunElevation:
                            outputSystemList[i].floatOutput = currentProfile.outputsList[i].sunElevationCurve.Evaluate(evaluateCurveSunElevation);
                            break;
                        case AzureOutputType.CurveMoonElevation:
                            outputSystemList[i].floatOutput = currentProfile.outputsList[i].moonElevationCurve.Evaluate(evaluateCurveMoonElevation);
                            break;
                        case AzureOutputType.Color:
                            outputSystemList[i].colorOutput = currentProfile.outputsList[i].color;
                            break;
                        case AzureOutputType.GradientTimeline:
                            outputSystemList[i].colorOutput = currentProfile.outputsList[i].timelineGradient.Evaluate(evaluateGradientTimeline);
                            break;
                        case AzureOutputType.GradientSunElevation:
                            outputSystemList[i].colorOutput = currentProfile.outputsList[i].sunElevationGradient.Evaluate(evaluateGradientSunElevation);
                            break;
                        case AzureOutputType.GradientMoonElevation:
                            outputSystemList[i].colorOutput = currentProfile.outputsList[i].moonElevationGradient.Evaluate(evaluateGradientMoonElevation);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Blends the profiles when there is a weather transition.
        /// </summary>
        private void ComputeWeatherTransition(AzureSkyProfile from, AzureSkyProfile to, float t)
        {
            // Scattering
            m_settings.MolecularDensity = FloatInterpolation(GetFloatPropertyValue(from.scattering.molecularDensity), GetFloatPropertyValue(to.scattering.molecularDensity), t);
            m_settings.Wavelength.x = FloatInterpolation(GetFloatPropertyValue(from.scattering.wavelengthR), GetFloatPropertyValue(to.scattering.wavelengthR), t);
            m_settings.Wavelength.y = FloatInterpolation(GetFloatPropertyValue(from.scattering.wavelengthG), GetFloatPropertyValue(to.scattering.wavelengthG), t);
            m_settings.Wavelength.z = FloatInterpolation(GetFloatPropertyValue(from.scattering.wavelengthB), GetFloatPropertyValue(to.scattering.wavelengthB), t);
            m_settings.RayleighMultiplier = FloatInterpolation(GetFloatPropertyValue(from.scattering.rayleighMultiplier), GetFloatPropertyValue(to.scattering.rayleighMultiplier), t);
            m_settings.MieMultiplier = FloatInterpolation(GetFloatPropertyValue(from.scattering.mieMultiplier), GetFloatPropertyValue(to.scattering.mieMultiplier), t);
            m_settings.ScatteringIntensity = FloatInterpolation(GetFloatPropertyValue(from.scattering.scatteringIntensity), GetFloatPropertyValue(to.scattering.scatteringIntensity), t);
            m_settings.SkyLuminance = FloatInterpolation(GetFloatPropertyValue(from.scattering.skyLuminance), GetFloatPropertyValue(to.scattering.skyLuminance), t);
            m_settings.RayleighColor = ColorInterpolation(GetColorPropertyValue(from.scattering.rayleighColor), GetColorPropertyValue(to.scattering.rayleighColor), t);
            m_settings.MieColor = ColorInterpolation(GetColorPropertyValue(from.scattering.mieColor), GetColorPropertyValue(to.scattering.mieColor), t);
            m_settings.TransmittanceColor = ColorInterpolation(GetColorPropertyValue(from.scattering.sunsetColor), GetColorPropertyValue(to.scattering.sunsetColor), t);
            m_settings.PrecomputedSunIntensity = FloatInterpolation(GetFloatPropertyValue(from.scattering.precomputedSunIntensity), GetFloatPropertyValue(to.scattering.precomputedSunIntensity), t);
            m_settings.PrecomputedMoonIntensity = FloatInterpolation(GetFloatPropertyValue(from.scattering.precomputedMoonIntensity), GetFloatPropertyValue(to.scattering.precomputedMoonIntensity), t);
            m_settings.Exposure = FloatInterpolation(GetFloatPropertyValue(from.scattering.exposure), GetFloatPropertyValue(to.scattering.exposure), t);
            
            // Outer space
            m_settings.SunTextureSize = FloatInterpolation(GetFloatPropertyValue(from.outerSpace.sunTextureSize), GetFloatPropertyValue(to.outerSpace.sunTextureSize), t);
            m_settings.SunTextureIntensity = FloatInterpolation(GetFloatPropertyValue(from.outerSpace.sunTextureIntensity), GetFloatPropertyValue(to.outerSpace.sunTextureIntensity), t);
            m_settings.SunTextureColor = ColorInterpolation(GetColorPropertyValue(from.outerSpace.sunTextureColor), GetColorPropertyValue(to.outerSpace.sunTextureColor), t);
            m_settings.MoonTextureSize = FloatInterpolation(GetFloatPropertyValue(from.outerSpace.moonTextureSize), GetFloatPropertyValue(to.outerSpace.moonTextureSize), t);
            m_settings.MoonTextureIntensity = FloatInterpolation(GetFloatPropertyValue(from.outerSpace.moonTextureIntensity), GetFloatPropertyValue(to.outerSpace.moonTextureIntensity), t);
            m_settings.MoonTextureColor = ColorInterpolation(GetColorPropertyValue(from.outerSpace.moonTextureColor), GetColorPropertyValue(to.outerSpace.moonTextureColor), t);
            m_settings.RegularStarsScintillation = FloatInterpolation(GetFloatPropertyValue(from.outerSpace.regularStarsScintillation), GetFloatPropertyValue(to.outerSpace.regularStarsScintillation), t);
            m_settings.RegularStarsIntensity = FloatInterpolation(GetFloatPropertyValue(from.outerSpace.regularStarsIntensity), GetFloatPropertyValue(to.outerSpace.regularStarsIntensity), t);
            m_settings.MilkyWayIntensity = FloatInterpolation(GetFloatPropertyValue(from.outerSpace.milkyWayIntensity), GetFloatPropertyValue(to.outerSpace.milkyWayIntensity), t);
            
            // Fog scattering
            m_settings.FogScatteringScale = FloatInterpolation(GetFloatPropertyValue(from.fogScattering.fogScatteringScale), GetFloatPropertyValue(to.fogScattering.fogScatteringScale), t);
            m_settings.GlobalFogSmooth = FloatInterpolation(GetFloatPropertyValue(from.fogScattering.globalFogSmooth), GetFloatPropertyValue(to.fogScattering.globalFogSmooth), t);
            m_settings.GlobalFogDensity = FloatInterpolation(GetFloatPropertyValue(from.fogScattering.globalFogDensity), GetFloatPropertyValue(to.fogScattering.globalFogDensity), t);
            m_settings.GlobalFogDistance = FloatInterpolation(GetFloatPropertyValue(from.fogScattering.globalFogDistance), GetFloatPropertyValue(to.fogScattering.globalFogDistance), t);
            m_settings.HeightFogSmooth = FloatInterpolation(GetFloatPropertyValue(from.fogScattering.heightFogSmooth), GetFloatPropertyValue(to.fogScattering.heightFogSmooth), t);
            m_settings.HeightFogDensity = FloatInterpolation(GetFloatPropertyValue(from.fogScattering.heightFogDensity), GetFloatPropertyValue(to.fogScattering.heightFogDensity), t);
            m_settings.HeightFogDistance = FloatInterpolation(GetFloatPropertyValue(from.fogScattering.heightFogDistance), GetFloatPropertyValue(to.fogScattering.heightFogDistance), t);
            m_settings.HeightFogStart = FloatInterpolation(GetFloatPropertyValue(from.fogScattering.heightFogStart), GetFloatPropertyValue(to.fogScattering.heightFogStart), t);
            m_settings.HeightFogEnd = FloatInterpolation(GetFloatPropertyValue(from.fogScattering.heightFogEnd), GetFloatPropertyValue(to.fogScattering.heightFogEnd), t);

            // Clouds
            m_settings.StaticCloudInterpolator = transitionProgress;
            staticCloudSource = currentProfile.clouds.staticCloudTexture;
            staticCloudTarget = targetProfile.clouds.staticCloudTexture;
            m_settings.StaticCloudLayer1Speed = FloatInterpolation(GetFloatPropertyValue(from.clouds.staticCloudLayer1Speed), GetFloatPropertyValue(to.clouds.staticCloudLayer1Speed), t);
            m_settings.StaticCloudLayer2Speed = FloatInterpolation(GetFloatPropertyValue(from.clouds.staticCloudLayer2Speed), GetFloatPropertyValue(to.clouds.staticCloudLayer2Speed), t);
            m_settings.StaticCloudColor = ColorInterpolation(GetColorPropertyValue(from.clouds.staticCloudColor), GetColorPropertyValue(to.clouds.staticCloudColor), t);
            m_settings.StaticCloudScattering = FloatInterpolation(GetFloatPropertyValue(from.clouds.staticCloudScattering), GetFloatPropertyValue(to.clouds.staticCloudScattering), t);
            m_settings.StaticCloudExtinction = FloatInterpolation(GetFloatPropertyValue(from.clouds.staticCloudExtinction), GetFloatPropertyValue(to.clouds.staticCloudExtinction), t);
            m_settings.StaticCloudSaturation = FloatInterpolation(GetFloatPropertyValue(from.clouds.staticCloudSaturation), GetFloatPropertyValue(to.clouds.staticCloudSaturation), t);
            m_settings.StaticCloudOpacity = FloatInterpolation(GetFloatPropertyValue(from.clouds.staticCloudOpacity), GetFloatPropertyValue(to.clouds.staticCloudOpacity), t);
            m_settings.DynamicCloudAltitude = FloatInterpolation(GetFloatPropertyValue(from.clouds.dynamicCloudAltitude), GetFloatPropertyValue(to.clouds.dynamicCloudAltitude), t);
            m_settings.DynamicCloudDirection = FloatInterpolation(GetFloatPropertyValue(from.clouds.dynamicCloudDirection), GetFloatPropertyValue(to.clouds.dynamicCloudDirection), t);
            m_settings.DynamicCloudSpeed = FloatInterpolation(GetFloatPropertyValue(from.clouds.dynamicCloudSpeed), GetFloatPropertyValue(to.clouds.dynamicCloudSpeed), t);
            m_settings.DynamicCloudDensity = FloatInterpolation(GetFloatPropertyValue(from.clouds.dynamicCloudDensity), GetFloatPropertyValue(to.clouds.dynamicCloudDensity), t);
            m_settings.DynamicCloudColor1 = ColorInterpolation(GetColorPropertyValue(from.clouds.dynamicCloudColor1), GetColorPropertyValue(to.clouds.dynamicCloudColor1), t);
            m_settings.DynamicCloudColor2 = ColorInterpolation(GetColorPropertyValue(from.clouds.dynamicCloudColor2), GetColorPropertyValue(to.clouds.dynamicCloudColor2), t);

            // Lighting
            m_settings.DirectionalLightIntensity = FloatInterpolation(GetFloatPropertyValue(from.lighting.directionalLightIntensity), GetFloatPropertyValue(to.lighting.directionalLightIntensity), t);
            m_settings.DirectionalLightColor = ColorInterpolation(GetColorPropertyValue(from.lighting.directionalLightColor), GetColorPropertyValue(to.lighting.directionalLightColor), t);
            m_settings.AmbientIntensity = FloatInterpolation(GetFloatPropertyValue(from.lighting.ambientIntensity), GetFloatPropertyValue(to.lighting.ambientIntensity), t);
            m_settings.AmbientColor = ColorInterpolation(GetColorPropertyValue(from.lighting.ambientColor), GetColorPropertyValue(to.lighting.ambientColor), t);
            m_settings.AmbientSkyColor = ColorInterpolation(GetColorPropertyValue(from.lighting.ambientSkyColor), GetColorPropertyValue(to.lighting.ambientSkyColor), t);
            m_settings.AmbientEquatorColor = ColorInterpolation(GetColorPropertyValue(from.lighting.ambientEquatorColor), GetColorPropertyValue(to.lighting.ambientEquatorColor), t);
            m_settings.AmbientGroundColor = ColorInterpolation(GetColorPropertyValue(from.lighting.ambientGroundColor), GetColorPropertyValue(to.lighting.ambientGroundColor), t);
            m_settings.SkyReflectionIntensity = FloatInterpolation(GetFloatPropertyValue(from.lighting.skyReflectionIntensity), GetFloatPropertyValue(to.lighting.skyReflectionIntensity), t);
            
            // Weather
            m_settings.RainLightIntensity = FloatInterpolation(GetFloatPropertyValue(from.weather.rainLightIntensity), GetFloatPropertyValue(to.weather.rainLightIntensity), t);
            m_settings.RainMediumIntensity = FloatInterpolation(GetFloatPropertyValue(from.weather.rainMediumIntensity), GetFloatPropertyValue(to.weather.rainMediumIntensity), t);
            m_settings.RainHeavyIntensity = FloatInterpolation(GetFloatPropertyValue(from.weather.rainHeavyIntensity), GetFloatPropertyValue(to.weather.rainHeavyIntensity), t);
            m_settings.SnowIntensity = FloatInterpolation(GetFloatPropertyValue(from.weather.snowIntensity), GetFloatPropertyValue(to.weather.snowIntensity), t);
            m_settings.RainLightVolume = FloatInterpolation(GetFloatPropertyValue(from.weather.rainLightVolume), GetFloatPropertyValue(to.weather.rainLightVolume), t);
            m_settings.RainMediumVolume = FloatInterpolation(GetFloatPropertyValue(from.weather.rainMediumVolume), GetFloatPropertyValue(to.weather.rainMediumVolume), t);
            m_settings.RainHeavyVolume = FloatInterpolation(GetFloatPropertyValue(from.weather.rainHeavyVolume), GetFloatPropertyValue(to.weather.rainHeavyVolume), t);
            m_settings.WindLightVolume = FloatInterpolation(GetFloatPropertyValue(from.weather.windLightVolume), GetFloatPropertyValue(to.weather.windLightVolume), t);
            m_settings.WindMediumVolume = FloatInterpolation(GetFloatPropertyValue(from.weather.windMediumVolume), GetFloatPropertyValue(to.weather.windMediumVolume), t);
            m_settings.WindHeavyVolume = FloatInterpolation(GetFloatPropertyValue(from.weather.windHeavyVolume), GetFloatPropertyValue(to.weather.windHeavyVolume), t);
            m_settings.RainColor = ColorInterpolation(GetColorPropertyValue(from.weather.rainColor), GetColorPropertyValue(to.weather.rainColor), t);
            m_settings.SnowColor = ColorInterpolation(GetColorPropertyValue(from.weather.snowColor), GetColorPropertyValue(to.weather.snowColor), t);
            m_settings.WindSpeed = FloatInterpolation(GetFloatPropertyValue(from.weather.windSpeed), GetFloatPropertyValue(to.weather.windSpeed), t);
            m_settings.WindDirection = FloatInterpolation(GetFloatPropertyValue(from.weather.windDirection), GetFloatPropertyValue(to.weather.windDirection), t);
            m_settings.ThunderMultiplier = FloatInterpolation(GetFloatPropertyValue(from.weather.thunderMultiplier), GetFloatPropertyValue(to.weather.thunderMultiplier), t);
            
            // Outputs
            if (outputSystemList.Count > 0 && from.outputTag == to.outputTag && from.outputsList.Count == to.outputsList.Count && outputSystemList.Count > from.outputsList.Count && outputSystemList.Count > to.outputsList.Count)
            {
                for (int i = 0; i < outputSystemList.Count; i++)
                {
                    switch (outputSystemList[i].azureOutputType)
                    {
                        case AzureOutputType.Slider:
                            outputSystemList[i].floatOutput = FloatInterpolation(from.outputsList[i].slider, to.outputsList[i].slider, t);
                            break;
                        case AzureOutputType.Vector2:
                            outputSystemList[i].vector2Output = Vector2Interpolation(from.outputsList[i].vector2, to.outputsList[i].vector2, t);
                            break;
                        case AzureOutputType.Vector3:
                            outputSystemList[i].vector3Output = Vector3Interpolation(from.outputsList[i].vector3, to.outputsList[i].vector3, t);
                            break;
                        case AzureOutputType.CurveTimeline:
                            outputSystemList[i].floatOutput = FloatInterpolation(from.outputsList[i].timelineCurve.Evaluate(evaluateCurveTimeline), to.outputsList[i].timelineCurve.Evaluate(evaluateCurveTimeline), t);
                            break;
                        case AzureOutputType.CurveSunElevation:
                            outputSystemList[i].floatOutput = FloatInterpolation(from.outputsList[i].sunElevationCurve.Evaluate(evaluateCurveSunElevation), to.outputsList[i].sunElevationCurve.Evaluate(evaluateCurveSunElevation), t);
                            break;
                        case AzureOutputType.CurveMoonElevation:
                            outputSystemList[i].floatOutput = FloatInterpolation(from.outputsList[i].moonElevationCurve.Evaluate(evaluateCurveMoonElevation), to.outputsList[i].moonElevationCurve.Evaluate(evaluateCurveMoonElevation), t);
                            break;
                        case AzureOutputType.Color:
                            outputSystemList[i].colorOutput = ColorInterpolation(from.outputsList[i].color, to.outputsList[i].color, t);
                            break;
                        case AzureOutputType.GradientTimeline:
                            outputSystemList[i].colorOutput = ColorInterpolation(from.outputsList[i].timelineGradient.Evaluate(evaluateGradientTimeline), to.outputsList[i].timelineGradient.Evaluate(evaluateGradientTimeline), t);
                            break;
                        case AzureOutputType.GradientSunElevation:
                            outputSystemList[i].colorOutput = ColorInterpolation(from.outputsList[i].sunElevationGradient.Evaluate(evaluateGradientSunElevation), to.outputsList[i].sunElevationGradient.Evaluate(evaluateGradientSunElevation), t);
                            break;
                        case AzureOutputType.GradientMoonElevation:
                            outputSystemList[i].colorOutput = ColorInterpolation(from.outputsList[i].moonElevationGradient.Evaluate(evaluateGradientMoonElevation), to.outputsList[i].moonElevationGradient.Evaluate(evaluateGradientMoonElevation), t);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Computes local weather zones influence.
        /// </summary>
        private void BlendWeatherZones(AzureSkyProfile climateZoneProfile, float t)
        {
            // Scattering
            m_settings.MolecularDensity = FloatInterpolation(m_settings.MolecularDensity, GetFloatPropertyValue(climateZoneProfile.scattering.molecularDensity), t);
            m_settings.Wavelength.x = FloatInterpolation(m_settings.Wavelength.x, GetFloatPropertyValue(climateZoneProfile.scattering.wavelengthR), t);
            m_settings.Wavelength.y = FloatInterpolation(m_settings.Wavelength.y, GetFloatPropertyValue(climateZoneProfile.scattering.wavelengthG), t);
            m_settings.Wavelength.z = FloatInterpolation(m_settings.Wavelength.z, GetFloatPropertyValue(climateZoneProfile.scattering.wavelengthB), t);
            m_settings.RayleighMultiplier = FloatInterpolation(m_settings.RayleighMultiplier, GetFloatPropertyValue(climateZoneProfile.scattering.rayleighMultiplier), t);
            m_settings.MieMultiplier = FloatInterpolation(m_settings.MieMultiplier, GetFloatPropertyValue(climateZoneProfile.scattering.mieMultiplier), t);
            m_settings.ScatteringIntensity = FloatInterpolation(m_settings.ScatteringIntensity, GetFloatPropertyValue(climateZoneProfile.scattering.scatteringIntensity), t);
            m_settings.SkyLuminance = FloatInterpolation(m_settings.SkyLuminance, GetFloatPropertyValue(climateZoneProfile.scattering.skyLuminance), t);
            m_settings.RayleighColor = ColorInterpolation(m_settings.RayleighColor, GetColorPropertyValue(climateZoneProfile.scattering.rayleighColor), t);
            m_settings.MieColor = ColorInterpolation(m_settings.MieColor, GetColorPropertyValue(climateZoneProfile.scattering.mieColor), t);
            m_settings.TransmittanceColor = ColorInterpolation(m_settings.TransmittanceColor, GetColorPropertyValue(climateZoneProfile.scattering.sunsetColor), t);
            m_settings.PrecomputedSunIntensity = FloatInterpolation(m_settings.PrecomputedSunIntensity, GetFloatPropertyValue(climateZoneProfile.scattering.precomputedSunIntensity), t);
            m_settings.PrecomputedMoonIntensity = FloatInterpolation(m_settings.PrecomputedMoonIntensity, GetFloatPropertyValue(climateZoneProfile.scattering.precomputedMoonIntensity), t);
            m_settings.Exposure = FloatInterpolation(m_settings.Exposure, GetFloatPropertyValue(climateZoneProfile.scattering.exposure), t);
            
            // Outer space
            m_settings.SunTextureSize = FloatInterpolation(m_settings.SunTextureSize, GetFloatPropertyValue(climateZoneProfile.outerSpace.sunTextureSize), t);
            m_settings.SunTextureIntensity = FloatInterpolation(m_settings.SunTextureIntensity, GetFloatPropertyValue(climateZoneProfile.outerSpace.sunTextureIntensity), t);
            m_settings.SunTextureColor = ColorInterpolation(m_settings.SunTextureColor, GetColorPropertyValue(climateZoneProfile.outerSpace.sunTextureColor), t);
            m_settings.MoonTextureSize = FloatInterpolation(m_settings.MoonTextureSize, GetFloatPropertyValue(climateZoneProfile.outerSpace.moonTextureSize), t);
            m_settings.MoonTextureIntensity = FloatInterpolation(m_settings.MoonTextureIntensity, GetFloatPropertyValue(climateZoneProfile.outerSpace.moonTextureIntensity), t);
            m_settings.MoonTextureColor = ColorInterpolation(m_settings.MoonTextureColor, GetColorPropertyValue(climateZoneProfile.outerSpace.moonTextureColor), t);
            m_settings.RegularStarsScintillation = FloatInterpolation(m_settings.RegularStarsScintillation, GetFloatPropertyValue(climateZoneProfile.outerSpace.regularStarsScintillation), t);
            m_settings.RegularStarsIntensity = FloatInterpolation(m_settings.RegularStarsIntensity, GetFloatPropertyValue(climateZoneProfile.outerSpace.regularStarsIntensity), t);
            m_settings.MilkyWayIntensity = FloatInterpolation(m_settings.MilkyWayIntensity, GetFloatPropertyValue(climateZoneProfile.outerSpace.milkyWayIntensity), t);
            
            // Fog scattering
            m_settings.FogScatteringScale = FloatInterpolation(m_settings.FogScatteringScale, GetFloatPropertyValue(climateZoneProfile.fogScattering.fogScatteringScale), t);
            m_settings.GlobalFogSmooth = FloatInterpolation(m_settings.GlobalFogSmooth, GetFloatPropertyValue(climateZoneProfile.fogScattering.globalFogSmooth), t);
            m_settings.GlobalFogDensity = FloatInterpolation(m_settings.GlobalFogDensity, GetFloatPropertyValue(climateZoneProfile.fogScattering.globalFogDensity), t);
            m_settings.GlobalFogDistance = FloatInterpolation(m_settings.GlobalFogDistance, GetFloatPropertyValue(climateZoneProfile.fogScattering.globalFogDistance), t);
            m_settings.HeightFogSmooth = FloatInterpolation(m_settings.HeightFogSmooth, GetFloatPropertyValue(climateZoneProfile.fogScattering.heightFogSmooth), t);
            m_settings.HeightFogDensity = FloatInterpolation(m_settings.HeightFogDensity, GetFloatPropertyValue(climateZoneProfile.fogScattering.heightFogDensity), t);
            m_settings.HeightFogDistance = FloatInterpolation(m_settings.HeightFogDistance, GetFloatPropertyValue(climateZoneProfile.fogScattering.heightFogDistance), t);
            m_settings.HeightFogStart = FloatInterpolation(m_settings.HeightFogStart, GetFloatPropertyValue(climateZoneProfile.fogScattering.heightFogStart), t);
            m_settings.HeightFogEnd = FloatInterpolation(m_settings.HeightFogEnd, GetFloatPropertyValue(climateZoneProfile.fogScattering.heightFogEnd), t);

            // Clouds
            m_settings.StaticCloudInterpolator = t;
            staticCloudSource = currentProfile.clouds.staticCloudTexture;
            staticCloudTarget = climateZoneProfile.clouds.staticCloudTexture;
            m_settings.StaticCloudLayer1Speed = FloatInterpolation(m_settings.StaticCloudLayer1Speed, GetFloatPropertyValue(climateZoneProfile.clouds.staticCloudLayer1Speed), t);
            m_settings.StaticCloudLayer2Speed = FloatInterpolation(m_settings.StaticCloudLayer2Speed, GetFloatPropertyValue(climateZoneProfile.clouds.staticCloudLayer2Speed), t);
            m_settings.StaticCloudColor = ColorInterpolation(m_settings.StaticCloudColor, GetColorPropertyValue(climateZoneProfile.clouds.staticCloudColor), t);
            m_settings.StaticCloudScattering = FloatInterpolation(m_settings.StaticCloudScattering, GetFloatPropertyValue(climateZoneProfile.clouds.staticCloudScattering), t);
            m_settings.StaticCloudExtinction = FloatInterpolation(m_settings.StaticCloudExtinction, GetFloatPropertyValue(climateZoneProfile.clouds.staticCloudExtinction), t);
            m_settings.StaticCloudSaturation = FloatInterpolation(m_settings.StaticCloudSaturation, GetFloatPropertyValue(climateZoneProfile.clouds.staticCloudSaturation), t);
            m_settings.StaticCloudOpacity = FloatInterpolation(m_settings.StaticCloudOpacity, GetFloatPropertyValue(climateZoneProfile.clouds.staticCloudOpacity), t);
            m_settings.DynamicCloudAltitude = FloatInterpolation(m_settings.DynamicCloudAltitude, GetFloatPropertyValue(climateZoneProfile.clouds.dynamicCloudAltitude), t);
            m_settings.DynamicCloudDirection = FloatInterpolation(m_settings.DynamicCloudDirection, GetFloatPropertyValue(climateZoneProfile.clouds.dynamicCloudDirection), t);
            m_settings.DynamicCloudSpeed = FloatInterpolation(m_settings.DynamicCloudSpeed, GetFloatPropertyValue(climateZoneProfile.clouds.dynamicCloudSpeed), t);
            m_settings.DynamicCloudDensity = FloatInterpolation(m_settings.DynamicCloudDensity, GetFloatPropertyValue(climateZoneProfile.clouds.dynamicCloudDensity), t);
            m_settings.DynamicCloudColor1 = ColorInterpolation(m_settings.DynamicCloudColor1, GetColorPropertyValue(climateZoneProfile.clouds.dynamicCloudColor1), t);
            m_settings.DynamicCloudColor2 = ColorInterpolation(m_settings.DynamicCloudColor2, GetColorPropertyValue(climateZoneProfile.clouds.dynamicCloudColor2), t);

            // Lighting
            m_settings.DirectionalLightIntensity = FloatInterpolation(m_settings.DirectionalLightIntensity, GetFloatPropertyValue(climateZoneProfile.lighting.directionalLightIntensity), t);
            m_settings.DirectionalLightColor = ColorInterpolation(m_settings.DirectionalLightColor, GetColorPropertyValue(climateZoneProfile.lighting.directionalLightColor), t);
            m_settings.AmbientIntensity = FloatInterpolation(m_settings.AmbientIntensity, GetFloatPropertyValue(climateZoneProfile.lighting.ambientIntensity), t);
            m_settings.AmbientColor = ColorInterpolation(m_settings.AmbientColor, GetColorPropertyValue(climateZoneProfile.lighting.ambientColor), t);
            m_settings.AmbientSkyColor = ColorInterpolation(m_settings.AmbientSkyColor, GetColorPropertyValue(climateZoneProfile.lighting.ambientSkyColor), t);
            m_settings.AmbientEquatorColor = ColorInterpolation(m_settings.AmbientEquatorColor, GetColorPropertyValue(climateZoneProfile.lighting.ambientEquatorColor), t);
            m_settings.AmbientGroundColor = ColorInterpolation(m_settings.AmbientGroundColor, GetColorPropertyValue(climateZoneProfile.lighting.ambientGroundColor), t);
            m_settings.SkyReflectionIntensity = FloatInterpolation(m_settings.SkyReflectionIntensity, GetFloatPropertyValue(climateZoneProfile.lighting.skyReflectionIntensity), t);
            
            // Weather
            m_settings.RainLightIntensity = FloatInterpolation(m_settings.RainLightIntensity, GetFloatPropertyValue(climateZoneProfile.weather.rainLightIntensity), t);
            m_settings.RainMediumIntensity = FloatInterpolation(m_settings.RainMediumIntensity, GetFloatPropertyValue(climateZoneProfile.weather.rainMediumIntensity), t);
            m_settings.RainHeavyIntensity = FloatInterpolation(m_settings.RainHeavyIntensity, GetFloatPropertyValue(climateZoneProfile.weather.rainHeavyIntensity), t);
            m_settings.SnowIntensity = FloatInterpolation(m_settings.SnowIntensity, GetFloatPropertyValue(climateZoneProfile.weather.snowIntensity), t);
            m_settings.RainLightVolume = FloatInterpolation(m_settings.RainLightVolume, GetFloatPropertyValue(climateZoneProfile.weather.rainLightVolume), t);
            m_settings.RainMediumVolume = FloatInterpolation(m_settings.RainMediumVolume, GetFloatPropertyValue(climateZoneProfile.weather.rainMediumVolume), t);
            m_settings.RainHeavyVolume = FloatInterpolation(m_settings.RainHeavyVolume, GetFloatPropertyValue(climateZoneProfile.weather.rainHeavyVolume), t);
            m_settings.WindLightVolume = FloatInterpolation(m_settings.WindLightVolume, GetFloatPropertyValue(climateZoneProfile.weather.windLightVolume), t);
            m_settings.WindMediumVolume = FloatInterpolation(m_settings.WindMediumVolume, GetFloatPropertyValue(climateZoneProfile.weather.windMediumVolume), t);
            m_settings.WindHeavyVolume = FloatInterpolation(m_settings.WindHeavyVolume, GetFloatPropertyValue(climateZoneProfile.weather.windHeavyVolume), t);
            m_settings.RainColor = ColorInterpolation(m_settings.RainColor, GetColorPropertyValue(climateZoneProfile.weather.rainColor), t);
            m_settings.SnowColor = ColorInterpolation(m_settings.SnowColor, GetColorPropertyValue(climateZoneProfile.weather.snowColor), t);
            m_settings.WindSpeed = FloatInterpolation(m_settings.WindSpeed, GetFloatPropertyValue(climateZoneProfile.weather.windSpeed), t);
            m_settings.WindDirection = FloatInterpolation(m_settings.WindDirection, GetFloatPropertyValue(climateZoneProfile.weather.windDirection), t);
            m_settings.ThunderMultiplier = FloatInterpolation(m_settings.ThunderMultiplier, GetFloatPropertyValue(climateZoneProfile.weather.thunderMultiplier), t);

            // Outputs
            if (outputSystemList.Count > 0 && climateZoneProfile.outputTag == outputTag && climateZoneProfile.outputsList.Count == outputSystemList.Count)
            {
                for (int i = 0; i < outputSystemList.Count; i++)
                {
                    switch (outputSystemList[i].azureOutputType)
                    {
                        case AzureOutputType.Slider:
                            outputSystemList[i].floatOutput = FloatInterpolation(outputSystemList[i].floatOutput, climateZoneProfile.outputsList[i].slider, t);
                            break;
                        case AzureOutputType.Vector2:
                            outputSystemList[i].vector2Output = Vector2Interpolation(outputSystemList[i].vector2Output, climateZoneProfile.outputsList[i].vector2, t);
                            break;
                        case AzureOutputType.Vector3:
                            outputSystemList[i].vector3Output = Vector3Interpolation(outputSystemList[i].vector3Output, climateZoneProfile.outputsList[i].vector3, t);
                            break;
                        case AzureOutputType.CurveTimeline:
                            outputSystemList[i].floatOutput = FloatInterpolation(outputSystemList[i].floatOutput, climateZoneProfile.outputsList[i].timelineCurve.Evaluate(evaluateCurveTimeline), t);
                            break;
                        case AzureOutputType.CurveSunElevation:
                            outputSystemList[i].floatOutput = FloatInterpolation(outputSystemList[i].floatOutput, climateZoneProfile.outputsList[i].sunElevationCurve.Evaluate(evaluateCurveSunElevation), t);
                            break;
                        case AzureOutputType.CurveMoonElevation:
                            outputSystemList[i].floatOutput = FloatInterpolation(outputSystemList[i].floatOutput, climateZoneProfile.outputsList[i].moonElevationCurve.Evaluate(evaluateCurveMoonElevation), t);
                            break;
                        case AzureOutputType.Color:
                            outputSystemList[i].colorOutput = ColorInterpolation(outputSystemList[i].colorOutput, climateZoneProfile.outputsList[i].color, t);
                            break;
                        case AzureOutputType.GradientTimeline:
                            outputSystemList[i].colorOutput = ColorInterpolation(outputSystemList[i].colorOutput, climateZoneProfile.outputsList[i].timelineGradient.Evaluate(evaluateGradientTimeline), t);
                            break;
                        case AzureOutputType.GradientSunElevation:
                            outputSystemList[i].colorOutput = ColorInterpolation(outputSystemList[i].colorOutput, climateZoneProfile.outputsList[i].sunElevationGradient.Evaluate(evaluateGradientSunElevation), t);
                            break;
                        case AzureOutputType.GradientMoonElevation:
                            outputSystemList[i].colorOutput = ColorInterpolation(outputSystemList[i].colorOutput, climateZoneProfile.outputsList[i].moonElevationGradient.Evaluate(evaluateGradientMoonElevation), t);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the float value of the custom Azure Float Property used in the profiles.
        /// </summary>
        private float GetFloatPropertyValue(AzureFloatProperty property)
        {
            float ret;
            switch (property.propertyType)
            {
                case AzureFloatProperty.PropertyType.Slider:
                    ret = property.slider;
                    break;
                case AzureFloatProperty.PropertyType.CurveTimeline:
                    ret = property.timelineCurve.Evaluate(evaluateCurveTimeline);
                    break;
                case AzureFloatProperty.PropertyType.CurveSunElevation:
                    ret = property.sunElevationCurve.Evaluate(evaluateCurveSunElevation);
                    break;
                case AzureFloatProperty.PropertyType.CurveMoonElevation:
                    ret = property.moonElevationCurve.Evaluate(evaluateCurveMoonElevation);
                    break;
                default:
                    ret = 1.0f;
                    break;
            }

            return ret;
        }
        
        /// <summary>
        /// Gets the color value of the custom Azure Color Property used in the profiles.
        /// </summary>
        private Color GetColorPropertyValue(AzureColorProperty property)
        {
            Color ret;
            switch (property.propertyType)
            {
                case AzureColorProperty.PropertyType.Color:
                    ret = property.color;
                    break;
                case AzureColorProperty.PropertyType.GradientTimeline:
                    ret = property.timelineGradient.Evaluate(evaluateGradientTimeline);
                    break;
                case AzureColorProperty.PropertyType.GradientSunElevation:
                    ret = property.sunElevationGradient.Evaluate(evaluateGradientSunElevation);
                    break;
                case AzureColorProperty.PropertyType.GradientMoonElevation:
                    ret = property.moonElevationGradient.Evaluate(evaluateGradientMoonElevation);
                    break;
                default:
                    ret = Color.white;
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Interpolates between two values given an interpolation factor.
        /// </summary>
        private float FloatInterpolation(float from, float to, float t)
        {
            return from + (to - from) * t;
        }
        
        /// <summary>
        /// Interpolates between two vectors given an interpolation factor.
        /// </summary>
        private Vector2 Vector2Interpolation(Vector2 from, Vector2 to, float t)
        {
            Vector2 ret;
            ret.x = from.x + (to.x - from.x) * t;
            ret.y = from.y + (to.y - from.y) * t;
            return ret;
        }

        /// <summary>
        /// Interpolates between two vectors given an interpolation factor.
        /// </summary>
        private Vector3 Vector3Interpolation(Vector3 from, Vector3 to, float t)
        {
            Vector3 ret;
            ret.x = from.x + (to.x - from.x) * t;
            ret.y = from.y + (to.y - from.y) * t;
            ret.z = from.z + (to.z - from.z) * t;
            return ret;
        }
        
        /// <summary>
        /// Interpolates between two colors given an interpolation factor.
        /// </summary>
        private Color ColorInterpolation(Color from, Color to, float t)
        {
            Color ret;
            ret.r = from.r + (to.r - from.r) * t;
            ret.g = from.g + (to.g - from.g) * t;
            ret.b = from.b + (to.b - from.b) * t;
            ret.a = from.a + (to.a - from.a) * t;
            return ret;
        }
    }
}