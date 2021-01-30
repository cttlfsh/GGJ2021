using System;

namespace UnityEngine.AzureSky
{
    public sealed class AzureSkySettings
    {
        // Scattering settings
        public float MolecularDensity = 2.545f;
        public Vector3 Wavelength = new Vector3(680.0f, 550.0f, 450.0f); // Visible wavelength 380 to 740
        public float RayleighMultiplier = 1.5f;
        public float MieMultiplier = 1.0f;
        public float ScatteringIntensity = 0.25f;
        public float SkyLuminance = 1.5f;
        public Color RayleighColor = Color.white;
        public Color MieColor = Color.white;
        public Color TransmittanceColor = Color.white;
        public float PrecomputedSunIntensity = 25.0f;
        public float PrecomputedMoonIntensity = 0.1f;
        public float Exposure = 2.0f;
        
        // Outer space
        public Color SunTextureColor = Color.white;
        public float SunTextureSize = 1.5f;
        public float SunTextureIntensity = 1.0f;
        public Color MoonTextureColor = Color.white;
        public float MoonTextureSize = 1.5f;
        public float MoonTextureIntensity = 1.0f;
        public float RegularStarsScintillation = 0.5f;
        public float RegularStarsIntensity = 0.5f;
        public float MilkyWayIntensity = 0.0f;
        
        // Fog scattering
        public float FogScatteringScale = 1.0f;
        public float GlobalFogSmooth = 0.25f;
        public float GlobalFogDensity = 1.0f;
        public float GlobalFogDistance = 1000.0f;
        public float HeightFogSmooth = 1.0f;
        public float HeightFogDensity = 0.0f;
        public float HeightFogDistance = 100.0f;
        public float HeightFogStart = 0.0f;
        public float HeightFogEnd = 100.0f;
        
        // Clouds
        public float StaticCloudInterpolator = 0.0f;
        public float StaticCloudLayer1Speed = 0.0f;
        public float StaticCloudLayer2Speed = 0.0f;
        public Color StaticCloudColor = Color.white;
        public float StaticCloudScattering = 1.0f;
        public float StaticCloudExtinction = 1.5f;
        public float StaticCloudSaturation = 2.5f;
        public float StaticCloudOpacity = 1.25f;
        public float DynamicCloudAltitude = 7.5f;
        public float DynamicCloudDirection = 1.0f;
        public float DynamicCloudSpeed = 0.1f;
        public float DynamicCloudDensity = 0.75f;
        public Color DynamicCloudColor1 = Color.white;
        public Color DynamicCloudColor2 = Color.white;
        
        // Lighting
        public float DirectionalLightIntensity = 1.0f;
        public Color DirectionalLightColor = Color.white;
        public float AmbientIntensity = 1.0f;
        public Color AmbientColor = Color.white;
        public Color AmbientSkyColor = Color.white;
        public Color AmbientEquatorColor = Color.white;
        public Color AmbientGroundColor = Color.white;
        public float SkyReflectionIntensity = 1.0f;
        
        // Weather
        public float RainLightIntensity = 0.0f;
        public float RainMediumIntensity = 0.0f;
        public float RainHeavyIntensity = 0.0f;
        public float SnowIntensity = 0.0f;
        public Color RainColor = Color.white;
        public Color SnowColor = Color.white;
        public float RainLightVolume = 0.0f;
        public float RainMediumVolume = 0.0f;
        public float RainHeavyVolume = 0.0f;
        public float WindLightVolume = 0.0f;
        public float WindMediumVolume = 0.0f;
        public float WindHeavyVolume = 0.0f;
        public float WindSpeed = 0.0f;
        public float WindDirection = 0.0f;
        public float ThunderMultiplier = 0.0f;
    }
}