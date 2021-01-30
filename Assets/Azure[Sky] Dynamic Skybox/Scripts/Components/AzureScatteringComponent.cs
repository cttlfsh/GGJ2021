using System;

namespace UnityEngine.AzureSky
{
    [Serializable]
    public sealed class AzureScatteringComponent
    {
        // Molecular density
        public AzureFloatProperty molecularDensity = new AzureFloatProperty
        (
            2.545f,
            AnimationCurve.Linear (0.0f, 2.545f, 24.0f, 2.545f),
            AnimationCurve.Linear (-1.0f, 2.545f, 1.0f, 2.545f),
            AnimationCurve.Linear (-1.0f, 2.545f, 1.0f, 2.545f)
        );
        
        // Visible wavelength
        public AzureFloatProperty wavelengthR = new AzureFloatProperty
        (
            680.0f,
            AnimationCurve.Linear (0.0f, 680.0f, 24.0f, 680.0f),
            AnimationCurve.Linear (-1.0f, 680.0f, 1.0f, 680.0f),
            AnimationCurve.Linear (-1.0f, 680.0f, 1.0f, 680.0f)
        );
        
        public AzureFloatProperty wavelengthG = new AzureFloatProperty
        (
            550.0f,
            AnimationCurve.Linear (0.0f, 550.0f, 24.0f, 550.0f),
            AnimationCurve.Linear (-1.0f, 550.0f, 1.0f, 550.0f),
            AnimationCurve.Linear (-1.0f, 550.0f, 1.0f, 550.0f)
        );
        
        public AzureFloatProperty wavelengthB = new AzureFloatProperty
        (
            450.0f,
            AnimationCurve.Linear (0.0f, 450.0f, 24.0f, 450.0f),
            AnimationCurve.Linear (-1.0f, 450.0f, 1.0f, 450.0f),
            AnimationCurve.Linear (-1.0f, 450.0f, 1.0f, 450.0f)
        );
        
        // Rayleigh multiplier
        public AzureFloatProperty rayleighMultiplier = new AzureFloatProperty
        (
            1.5f,
            AnimationCurve.Linear (0.0f, 1.5f, 24.0f, 1.5f),
            AnimationCurve.Linear (-1.0f, 1.5f, 1.0f, 1.5f),
            AnimationCurve.Linear (-1.0f, 1.5f, 1.0f, 1.5f)
        );
        
        // Mie multiplier
        public AzureFloatProperty mieMultiplier = new AzureFloatProperty
        (
            1.0f,
            AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
            AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
            AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
        );
        
        // Scattering intensity
        public AzureFloatProperty scatteringIntensity = new AzureFloatProperty
        (
            0.25f,
            AnimationCurve.Linear (0.0f, 0.25f, 24.0f, 0.25f),
            AnimationCurve.Linear (-1.0f, 0.25f, 1.0f, 0.25f),
            AnimationCurve.Linear (-1.0f, 0.25f, 1.0f, 0.25f)
        );
        
        // Sky luminance
        public AzureFloatProperty skyLuminance = new AzureFloatProperty
        (
            1.5f,
            AnimationCurve.Linear (0.0f, 1.5f, 24.0f, 1.5f),
            AnimationCurve.Linear (-1.0f, 1.5f, 1.0f, 1.5f),
            AnimationCurve.Linear (-1.0f, 1.5f, 1.0f, 1.5f)
        );
        
        // Rayleigh color
        public AzureColorProperty rayleighColor = new AzureColorProperty
        (
            Color.white,
            new Gradient(),
            new Gradient(),
            new Gradient()
        );
        
        // Mie color
        public AzureColorProperty mieColor = new AzureColorProperty
        (
            Color.white,
            new Gradient(),
            new Gradient(),
            new Gradient()
        );
        
        // Transmittance color
        public AzureColorProperty sunsetColor = new AzureColorProperty
        (
            Color.white,
            new Gradient(),
            new Gradient(),
            new Gradient()
        );
        
        // Precomputed sun intensity
        public AzureFloatProperty precomputedSunIntensity = new AzureFloatProperty
        (
            25.0f,
            AnimationCurve.Linear (0.0f, 25.0f, 24.0f, 25.0f),
            AnimationCurve.Linear (-1.0f, 25.0f, 1.0f, 25.0f),
            AnimationCurve.Linear (-1.0f, 25.0f, 1.0f, 25.0f)
        );
        
        // Precomputed moon intensity
        public AzureFloatProperty precomputedMoonIntensity = new AzureFloatProperty
        (
            0.1f,
            AnimationCurve.Linear (0.0f, 0.1f, 24.0f, 0.1f),
            AnimationCurve.Linear (-1.0f, 0.1f, 1.0f, 0.1f),
            AnimationCurve.Linear (-1.0f, 0.1f, 1.0f, 0.1f)
        );
        
        // Exposure
        public AzureFloatProperty exposure = new AzureFloatProperty
        (
            2.0f,
            AnimationCurve.Linear (0.0f, 2.0f, 24.0f, 2.0f),
            AnimationCurve.Linear (-1.0f, 2.0f, 1.0f, 2.0f),
            AnimationCurve.Linear (-1.0f, 2.0f, 1.0f, 2.0f)
        );
    }
}