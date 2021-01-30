using System;

namespace UnityEngine.AzureSky
{
	[Serializable]
	public sealed class AzureColorProperty
	{
		public enum PropertyType
		{
			Color,
			GradientTimeline,
			GradientSunElevation,
			GradientMoonElevation
		}

		public PropertyType propertyType = PropertyType.Color;
		public Color color;
		public Gradient timelineGradient;
		public Gradient sunElevationGradient;
		public Gradient moonElevationGradient;

		public AzureColorProperty(Color color, Gradient timelineGradient, Gradient sunElevationGradient, Gradient moonElevationGradient)
		{
			this.color = color;
			this.timelineGradient = timelineGradient;
			this.sunElevationGradient = sunElevationGradient;
			this.moonElevationGradient = moonElevationGradient;
		}
	}
}