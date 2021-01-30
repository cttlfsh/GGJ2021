using System;

namespace UnityEngine.AzureSky
{
	[Serializable]
	public class AzureWeatherComponent
	{
		// Rain light intensity
		public AzureFloatProperty rainLightIntensity = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
		
		// Rain medium intensity
		public AzureFloatProperty rainMediumIntensity = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
		
		// Rain heavy intensity
		public AzureFloatProperty rainHeavyIntensity = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
		
		// Snow intensity
		public AzureFloatProperty snowIntensity = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
		
		// Rain color
		public AzureColorProperty rainColor = new AzureColorProperty
		(
			Color.white,
			new Gradient(),
			new Gradient(),
			new Gradient()
		);
		
		// Snow color
		public AzureColorProperty snowColor = new AzureColorProperty
		(
			Color.white,
			new Gradient(),
			new Gradient(),
			new Gradient()
		);
		
		// Rain light volume
		public AzureFloatProperty rainLightVolume = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
		
		// Rain medium volume
		public AzureFloatProperty rainMediumVolume = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
		
		// Rain heavy volume
		public AzureFloatProperty rainHeavyVolume = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
		
		// Wind light volume
		public AzureFloatProperty windLightVolume = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
		
		// Wind medium volume
		public AzureFloatProperty windMediumVolume = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
		
		// Wind heavy volume
		public AzureFloatProperty windHeavyVolume = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
		
		// Wind speed
		public AzureFloatProperty windSpeed = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
		
		// Wind direction
		public AzureFloatProperty windDirection = new AzureFloatProperty
		(
			0.5f,
			AnimationCurve.Linear (0.0f, 0.5f, 24.0f, 0.5f),
			AnimationCurve.Linear (-1.0f, 0.5f, 1.0f, 0.5f),
			AnimationCurve.Linear (-1.0f, 0.5f, 1.0f, 0.5f)
		);
		
		// Thunder multiplier
		public AzureFloatProperty thunderMultiplier = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
	}
}