using System;

namespace UnityEngine.AzureSky
{
    [Serializable]
    public class AzureFogScatteringComponent
    {
        // Fog scattering scale
        public AzureFloatProperty fogScatteringScale = new AzureFloatProperty
        (
            1.0f,
            AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
            AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
            AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
        );
        
        // Global fog smooth
        public AzureFloatProperty globalFogSmooth = new AzureFloatProperty
        (
            0.25f,
            AnimationCurve.Linear (0.0f, 0.25f, 24.0f, 0.25f),
            AnimationCurve.Linear (-1.0f, 0.25f, 1.0f, 0.25f),
            AnimationCurve.Linear (-1.0f, 0.25f, 1.0f, 0.25f)
        );
        
        // Global fog density
        public AzureFloatProperty globalFogDensity = new AzureFloatProperty
        (
            1.0f,
            AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
            AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
            AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
        );
        
        // Global fog distance
        public AzureFloatProperty globalFogDistance = new AzureFloatProperty
        (
            1000.0f,
            AnimationCurve.Linear (0.0f, 1000.0f, 24.0f, 1000.0f),
            AnimationCurve.Linear (-1.0f, 1000.0f, 1.0f, 1000.0f),
            AnimationCurve.Linear (-1.0f, 1000.0f, 1.0f, 1000.0f)
        );
        
        // Height fog smooth
        public AzureFloatProperty heightFogSmooth = new AzureFloatProperty
        (
            1.0f,
            AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
            AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
            AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
        );
        
        // Height fog density
        public AzureFloatProperty heightFogDensity = new AzureFloatProperty
        (
            0.0f,
            AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
            AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
            AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
        );
        
        // Height fog distance
        public AzureFloatProperty heightFogDistance = new AzureFloatProperty
        (
            100.0f,
            AnimationCurve.Linear (0.0f, 100.0f, 24.0f, 100.0f),
            AnimationCurve.Linear (-1.0f, 100.0f, 1.0f, 100.0f),
            AnimationCurve.Linear (-1.0f, 100.0f, 1.0f, 100.0f)
        );
        
        // Height fog start
        public AzureFloatProperty heightFogStart = new AzureFloatProperty
        (
            0.0f,
            AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
            AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
            AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
        );
        
        // Height fog end
        public AzureFloatProperty heightFogEnd = new AzureFloatProperty
        (
            100.0f,
            AnimationCurve.Linear (0.0f, 100.0f, 24.0f, 100.0f),
            AnimationCurve.Linear (-1.0f, 100.0f, 1.0f, 100.0f),
            AnimationCurve.Linear (-1.0f, 100.0f, 1.0f, 100.0f)
        );
    }
}