namespace UnityEngine.AzureSky
{
    internal static class AzureShaderUniforms
    {
        // Textures
        internal static readonly int SunTexture = Shader.PropertyToID("_Azure_SunTexture");
        internal static readonly int MoonTexture = Shader.PropertyToID("_Azure_MoonTexture");
        internal static readonly int StarFieldTexture = Shader.PropertyToID("_Azure_StarFieldTexture");
        internal static readonly int StarNoiseTexture = Shader.PropertyToID("_Azure_StarNoiseTexture");
        internal static readonly int DynamicCloudNoiseTexture = Shader.PropertyToID("_Azure_DynamicCloudNoiseTexture");
        internal static readonly int StaticCloudSourceTexture = Shader.PropertyToID("_Azure_StaticCloudSourceTexture");
        internal static readonly int StaticCloudTargetTexture = Shader.PropertyToID("_Azure_StaticCloudTargetTexture");
        internal static readonly int TransmittanceTexture = Shader.PropertyToID("_Azure_TransmittanceTexture");
        internal static readonly int InScatterTexture = Shader.PropertyToID("_Azure_InScatterTexture");
        
        // Directions
        internal static readonly int SunDirection = Shader.PropertyToID("_Azure_SunDirection");
        internal static readonly int MoonDirection = Shader.PropertyToID("_Azure_MoonDirection");
        internal static readonly int SunMatrix = Shader.PropertyToID("_Azure_SunMatrix");
        internal static readonly int MoonMatrix = Shader.PropertyToID("_Azure_MoonMatrix");
        internal static readonly int UpDirectionMatrix = Shader.PropertyToID("_Azure_UpDirectionMatrix");
        internal static readonly int StarFieldMatrix = Shader.PropertyToID("_Azure_StarFieldMatrix");
        internal static readonly int NoiseRotationMatrix = Shader.PropertyToID("_Azure_NoiseRotationMatrix");
        
        // Scattering
        internal static readonly int StylizedSunsetMode = Shader.PropertyToID("_Azure_StylizedTransmittanceMode");
        internal static readonly int SkyModel = Shader.PropertyToID("_Azure_SkyModel");
        internal static readonly int Rayleigh = Shader.PropertyToID("_Azure_Br");
        internal static readonly int Mie = Shader.PropertyToID("_Azure_Bm");
        internal static readonly int ScatteringIntensity = Shader.PropertyToID("_Azure_ScatteringIntensity");
        internal static readonly int SkyLuminance = Shader.PropertyToID("_Azure_SkyLuminance");
        internal static readonly int RayleighColor = Shader.PropertyToID("_Azure_RayleighColor");
        internal static readonly int MieColor = Shader.PropertyToID("_Azure_MieColor");
        internal static readonly int TransmittanceColor = Shader.PropertyToID("_Azure_TransmittanceColor");
        internal static readonly int Exposure = Shader.PropertyToID("_Azure_Exposure");
        internal static readonly int EarthSize = Shader.PropertyToID("_Azure_PrecomputedRg");
        internal static readonly int AtmosphereSize = Shader.PropertyToID("_Azure_PrecomputedRt");
        internal static readonly int EarthPosition = Shader.PropertyToID("_Azure_EarthPosition");
        internal static readonly int PrecomputedSunIntensity = Shader.PropertyToID("_Azure_PrecomputedSunIntensity");
        internal static readonly int PrecomputedMoonIntensity = Shader.PropertyToID("_Azure_PrecomputedMoonIntensity");
        
        // Outer space
        internal static readonly int SunTextureColor = Shader.PropertyToID("_Azure_SunTextureColor");
        internal static readonly int SunTextureSize = Shader.PropertyToID("_Azure_SunTextureSize");
        internal static readonly int SunTextureIntensity = Shader.PropertyToID("_Azure_SunTextureIntensity");
        internal static readonly int MoonTextureColor = Shader.PropertyToID("_Azure_MoonTextureColor");
        internal static readonly int MoonTextureSize = Shader.PropertyToID("_Azure_MoonTextureSize");
        internal static readonly int MoonTextureIntensity = Shader.PropertyToID("_Azure_MoonTextureIntensity");
        internal static readonly int RegularStarsScintillation = Shader.PropertyToID("_Azure_RegularStarsScintillation");
        internal static readonly int RegularStarsIntensity = Shader.PropertyToID("_Azure_RegularStarsIntensity");
        internal static readonly int MilkyWayIntensity = Shader.PropertyToID("_Azure_MilkyWayIntensity");
        internal static readonly int StarFieldColorBalance = Shader.PropertyToID("_Azure_StarFieldColorBalance");
        
        // Fog scattering
        internal static readonly int FogScatteringScale = Shader.PropertyToID("_Azure_FogScatteringScale");
        internal static readonly int GlobalFogSmooth = Shader.PropertyToID("_Azure_GlobalFogSmooth");
        internal static readonly int GlobalFogDensity = Shader.PropertyToID("_Azure_GlobalFogDensity");
        internal static readonly int GlobalFogDistance = Shader.PropertyToID("_Azure_GlobalFogDistance");
        internal static readonly int HeightFogSmooth = Shader.PropertyToID("_Azure_HeightFogSmooth");
        internal static readonly int HeightFogDensity = Shader.PropertyToID("_Azure_HeightFogDensity");
        internal static readonly int HeightFogDistance = Shader.PropertyToID("_Azure_HeightFogDistance");
        internal static readonly int HeightFogStart = Shader.PropertyToID("_Azure_HeightFogStart");
        internal static readonly int HeightFogEnd = Shader.PropertyToID("_Azure_HeightFogEnd");
        
        // Clouds
        internal static readonly int StaticCloudInterpolator = Shader.PropertyToID("_Azure_StaticCloudInterpolator");
        internal static readonly int StaticLowCloudSpeed = Shader.PropertyToID("_Azure_StaticLowCloudSpeed");
        internal static readonly int StaticHigCloudSpeed = Shader.PropertyToID("_Azure_StaticHigCloudSpeed");
        internal static readonly int StaticCloudColor = Shader.PropertyToID("_Azure_StaticCloudColor");
        internal static readonly int StaticCloudScattering = Shader.PropertyToID("_Azure_StaticCloudScattering");
        internal static readonly int StaticCloudExtinction = Shader.PropertyToID("_Azure_StaticCloudExtinction");
        internal static readonly int StaticCloudDensity = Shader.PropertyToID("_Azure_StaticCloudDensity");
        internal static readonly int StaticCloudOpacity = Shader.PropertyToID("_Azure_StaticCloudOpacity");
        internal static readonly int DynamicCloudAltitude = Shader.PropertyToID("_Azure_DynamicCloudAltitude");
        internal static readonly int DynamicCloudDirection = Shader.PropertyToID("_Azure_DynamicCloudDirection");
        internal static readonly int DynamicCloudSpeed = Shader.PropertyToID("_Azure_DynamicCloudSpeed");
        internal static readonly int DynamicCloudDensity = Shader.PropertyToID("_Azure_DynamicCloudDensity");
        internal static readonly int DynamicCloudColor1 = Shader.PropertyToID("_Azure_DynamicCloudColor1");
        internal static readonly int DynamicCloudColor2 = Shader.PropertyToID("_Azure_DynamicCloudColor2");
        internal static readonly int ThunderLightning = Shader.PropertyToID("_Azure_ThunderLightning");
        internal static readonly int ThunderMultiplier = Shader.PropertyToID("_Azure_ThunderMultiplier");
    }
}