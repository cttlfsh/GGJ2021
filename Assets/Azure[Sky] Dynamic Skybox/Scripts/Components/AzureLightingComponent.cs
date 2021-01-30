using System;

namespace UnityEngine.AzureSky
{
	[Serializable]
	public class AzureLightingComponent
	{
		// Directional light intensity
		public AzureFloatProperty directionalLightIntensity = new AzureFloatProperty
		(
			1.0f,
			AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
			AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
			AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
		);
		
		// Directional light color
		public AzureColorProperty directionalLightColor = new AzureColorProperty
		(
			Color.white,
			new Gradient(),
			new Gradient(),
			new Gradient()
		);
		
		// Ambient intensity
		public AzureFloatProperty ambientIntensity = new AzureFloatProperty
		(
			1.0f,
			AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
			AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
			AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
		);
		
		// Ambient color
		public AzureColorProperty ambientColor = new AzureColorProperty
		(
			Color.white,
			new Gradient(),
			new Gradient(),
			new Gradient()
		);
		
		// Ambient sky color
		public AzureColorProperty ambientSkyColor = new AzureColorProperty
		(
			Color.white,
			new Gradient(),
			new Gradient(),
			new Gradient()
		);
		
		// Ambient equator color
		public AzureColorProperty ambientEquatorColor = new AzureColorProperty
		(
			Color.white,
			new Gradient(),
			new Gradient(),
			new Gradient()
		);
		
		// Ambient ground color
		public AzureColorProperty ambientGroundColor = new AzureColorProperty
		(
			Color.white,
			new Gradient(),
			new Gradient(),
			new Gradient()
		);
		
		// Sky reflection intensity
		public AzureFloatProperty skyReflectionIntensity = new AzureFloatProperty
		(
			1.0f,
			AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
			AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
			AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
		);
	}
}