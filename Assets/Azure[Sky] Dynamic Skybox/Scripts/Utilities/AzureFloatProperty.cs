using System;

namespace UnityEngine.AzureSky
{
    [Serializable]
    public sealed class AzureFloatProperty
    {
        public enum PropertyType
        {
            Slider,
            CurveTimeline,
            CurveSunElevation,
            CurveMoonElevation
        }

        public PropertyType propertyType = PropertyType.Slider;
        public float slider;
        public AnimationCurve timelineCurve;
        public AnimationCurve sunElevationCurve;
        public AnimationCurve moonElevationCurve;

        public AzureFloatProperty(float slider, AnimationCurve timelineCurve, AnimationCurve sunElevationCurve, AnimationCurve moonElevationCurve)
        {
            this.slider = slider;
            this.timelineCurve = timelineCurve;
            this.sunElevationCurve = sunElevationCurve;
            this.moonElevationCurve = moonElevationCurve;
        }
    }
}