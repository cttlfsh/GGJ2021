using System;

namespace UnityEngine.AzureSky
{
    public enum AzureOutputType
    {
        Slider,
        Vector2,
        Vector3,
        CurveTimeline,
        CurveSunElevation,
        CurveMoonElevation,
        Color,
        GradientTimeline,
        GradientSunElevation,
        GradientMoonElevation
    }
    
    [Serializable]
    public class AzureOutput
    {
        // Not included in build
        #if UNITY_EDITOR
        public string description;
        #endif
        
        public AzureOutputType azureOutputType = AzureOutputType.Slider;
        public float floatOutput;
        public Color colorOutput;
        public Vector2 vector2Output;
        public Vector3 vector3Output;
    }
}