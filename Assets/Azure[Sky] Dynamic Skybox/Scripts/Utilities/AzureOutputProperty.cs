using System;

namespace UnityEngine.AzureSky
{
    [Serializable]
    public sealed class AzureOutputProperty
    {
        // Not included in build
        #if UNITY_EDITOR
        public string description = "";
        #endif

        public AzureOutputType azureOutputType;
        public float slider;
        public Vector2 vector2;
        public Vector3 vector3;
        public AnimationCurve timelineCurve;
        public AnimationCurve sunElevationCurve;
        public AnimationCurve moonElevationCurve;
        public Color color;
        public Gradient timelineGradient;
        public Gradient sunElevationGradient;
        public Gradient moonElevationGradient;
        
        // Constructor
        public AzureOutputProperty
        (
            AzureOutputType azureOutputType,
            float slider,
            Vector2 vector2,
            Vector3 vector3,
            AnimationCurve timelineCurve,
            AnimationCurve sunElevationCurve,
            AnimationCurve moonElevationCurve,
            Color color,
            Gradient timelineGradient,
            Gradient sunElevationGradient,
            Gradient moonElevationGradient
        )
        {
            this.azureOutputType = azureOutputType;
            this.slider = slider;
            this.vector2 = vector2;
            this.vector3 = vector3;
            this.timelineCurve = timelineCurve;
            this.sunElevationCurve = sunElevationCurve;
            this.moonElevationCurve = moonElevationCurve;
            this.color = color;
            this.timelineGradient = timelineGradient;
            this.sunElevationGradient = sunElevationGradient;
            this.moonElevationGradient = moonElevationGradient;
        }
    }
}