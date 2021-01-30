using System;

namespace UnityEngine.AzureSky
{
    [Serializable]
    public class AzureCloudsComponent
    {
        public Texture2D staticCloudTexture;
        
        // Static cloud layer1 rotation speed
        public AzureFloatProperty staticCloudLayer1Speed = new AzureFloatProperty
        (
            0.0f,
            AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
            AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
            AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
        );
        
        // Static cloud layer2 rotation speed
        public AzureFloatProperty staticCloudLayer2Speed = new AzureFloatProperty
        (
            0.0f,
            AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
            AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
            AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
        );
        
        // Static cloud color
        public AzureColorProperty staticCloudColor = new AzureColorProperty
        (
            Color.white,
            new Gradient(),
            new Gradient(),
            new Gradient()
        );
        
        // Static cloud scattering
        public AzureFloatProperty staticCloudScattering = new AzureFloatProperty
        (
            1.0f,
            AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
            AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
            AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
        );
        
        // Static cloud extinction
        public AzureFloatProperty staticCloudExtinction = new AzureFloatProperty
        (
            1.5f,
            AnimationCurve.Linear (0.0f, 1.5f, 24.0f, 1.5f),
            AnimationCurve.Linear (-1.0f, 1.5f, 1.0f, 1.5f),
            AnimationCurve.Linear (-1.0f, 1.5f, 1.0f, 1.5f)
        );
        
        // Static cloud density
        public AzureFloatProperty staticCloudSaturation = new AzureFloatProperty
        (
            2.5f,
            AnimationCurve.Linear (0.0f, 2.5f, 24.0f, 2.5f),
            AnimationCurve.Linear (-1.0f, 2.5f, 1.0f, 2.5f),
            AnimationCurve.Linear (-1.0f, 2.5f, 1.0f, 2.5f)
        );
        
        // Static cloud opacity
        public AzureFloatProperty staticCloudOpacity = new AzureFloatProperty
        (
            1.25f,
            AnimationCurve.Linear (0.0f, 1.25f, 24.0f, 1.25f),
            AnimationCurve.Linear (-1.0f, 1.25f, 1.0f, 1.25f),
            AnimationCurve.Linear (-1.0f, 1.25f, 1.0f, 1.25f)
        );
        
        // Dynamic cloud altitude
        public AzureFloatProperty dynamicCloudAltitude = new AzureFloatProperty
        (
            7.5f,
            AnimationCurve.Linear (0.0f, 7.5f, 24.0f, 7.5f),
            AnimationCurve.Linear (-1.0f, 7.5f, 1.0f, 7.5f),
            AnimationCurve.Linear (-1.0f, 7.5f, 1.0f, 7.5f)
        );
        
        // Dynamic cloud direction
        public AzureFloatProperty dynamicCloudDirection = new AzureFloatProperty
        (
            1.0f,
            AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
            AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
            AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
        );
        
        // Dynamic cloud speed
        public AzureFloatProperty dynamicCloudSpeed = new AzureFloatProperty
        (
            0.1f,
            AnimationCurve.Linear (0.0f, 0.1f, 24.0f, 0.1f),
            AnimationCurve.Linear (-1.0f, 0.1f, 1.0f, 0.1f),
            AnimationCurve.Linear (-1.0f, 0.1f, 1.0f, 0.1f)
        );
        
        // Dynamic cloud density
        public AzureFloatProperty dynamicCloudDensity = new AzureFloatProperty
        (
            0.75f,
            AnimationCurve.Linear (0.0f, 0.75f, 24.0f, 0.75f),
            AnimationCurve.Linear (-1.0f, 0.75f, 1.0f, 0.75f),
            AnimationCurve.Linear (-1.0f, 0.75f, 1.0f, 0.75f)
        );
        
        // Dynamic cloud color 1
        public AzureColorProperty dynamicCloudColor1 = new AzureColorProperty
        (
            Color.white,
            new Gradient(),
            new Gradient(),
            new Gradient()
        );
        
        // Dynamic cloud color 2
        public AzureColorProperty dynamicCloudColor2 = new AzureColorProperty
        (
            Color.white,
            new Gradient(),
            new Gradient(),
            new Gradient()
        );
    }
}