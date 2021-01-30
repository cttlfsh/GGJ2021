using System;

namespace UnityEngine.AzureSky
{
	[Serializable]
	public sealed class AzureOuterSpaceComponent
	{
		// Sun texture size
		public AzureFloatProperty sunTextureSize = new AzureFloatProperty
		(
			2.5f,
			AnimationCurve.Linear (0.0f, 2.5f, 24.0f, 2.5f),
			AnimationCurve.Linear (-1.0f, 2.5f, 1.0f, 2.5f),
			AnimationCurve.Linear (-1.0f, 2.5f, 1.0f, 2.5f)
		);
        
		// Sun texture intensity
		public AzureFloatProperty sunTextureIntensity = new AzureFloatProperty
		(
			3.0f,
			AnimationCurve.Linear (0.0f, 3.0f, 24.0f, 3.0f),
			AnimationCurve.Linear (-1.0f, 3.0f, 1.0f, 3.0f),
			AnimationCurve.Linear (-1.0f, 3.0f, 1.0f, 3.0f)
		);
		
		// Sun texture color
		public AzureColorProperty sunTextureColor = new AzureColorProperty
		(
			Color.white,
			new Gradient(),
			new Gradient(),
			new Gradient()
		);
		
		// Moon texture size
		public AzureFloatProperty moonTextureSize = new AzureFloatProperty
		(
			10.0f,
			AnimationCurve.Linear (0.0f, 10.0f, 24.0f, 10.0f),
			AnimationCurve.Linear (-1.0f, 10.0f, 1.0f, 10.0f),
			AnimationCurve.Linear (-1.0f, 10.0f, 1.0f, 10.0f)
		);
		
		// Moon texture intensity
		public AzureFloatProperty moonTextureIntensity = new AzureFloatProperty
		(
			1.0f,
			AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
			AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
			AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
		);
		
		// Moon texture color
		public AzureColorProperty moonTextureColor = new AzureColorProperty
		(
			Color.white,
			new Gradient(),
			new Gradient(),
			new Gradient()
		);
		
		// Regular stars scintillation
		public AzureFloatProperty regularStarsScintillation = new AzureFloatProperty
		(
			0.5f,
			AnimationCurve.Linear (0.0f, 0.5f, 24.0f, 0.5f),
			AnimationCurve.Linear (-1.0f, 0.5f, 1.0f, 0.5f),
			AnimationCurve.Linear (-1.0f, 0.5f, 1.0f, 0.5f)
		);
		
		// Regular stars intensity
		public AzureFloatProperty regularStarsIntensity = new AzureFloatProperty
		(
			0.5f,
			AnimationCurve.Linear (0.0f, 0.5f, 24.0f, 0.5f),
			AnimationCurve.Linear (-1.0f, 0.5f, 1.0f, 0.5f),
			AnimationCurve.Linear (-1.0f, 0.5f, 1.0f, 0.5f)
		);
		
		// Milky Way intensity
		public AzureFloatProperty milkyWayIntensity = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
	}
}