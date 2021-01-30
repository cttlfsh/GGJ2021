using System;
using System.Collections.Generic;

namespace UnityEngine.AzureSky
{
	/// <summary>
	/// Thunder settings container.
	/// </summary>
	[Serializable]
	public struct AzureThunderSettings
	{
		public Transform thunderPrefab;
		public AudioClip audioClip;
		public AnimationCurve lightFrequency;
		public float audioDelay;
		public Vector3 position;
	}

	[ExecuteInEditMode]
	public class AzureSkyEffects : MonoBehaviour
	{
		/// <summary>
		/// Reference to AzureSkyManager.
		/// </summary>
		public AzureSkyManager azureSky;

		public WindZone windZone;
		public float windMultiplier = 1.0f;
		private Vector3 m_windDirection = Vector3.up;
		
		/// <summary>
		/// Audio source to perform the rain sounds effects.
		/// </summary>
		public AudioSource rainLight;
		public AudioSource rainMedium;
		public AudioSource rainHeavy;
		public AudioSource windLight;
		public AudioSource windMedium;
		public AudioSource windHeavy;

		/// <summary>
		/// Store a particle group and a target transform.
		/// </summary>
		[Serializable]
		public struct AzureParticleDriver
		{
			public Transform particleTransform;
			public Transform followTargetTransform;
		}
		/// <summary>
		/// List of the particles group and target transform.
		/// </summary>
		public List<AzureParticleDriver> particlesDriverList = new List<AzureParticleDriver>();
		
		/// <summary>
		/// Particle material.
		/// </summary>
		public Material particleRainMaterial, particleHeavyRainMaterial, particleSnowMaterial, particleRippleMaterial;

		/// <summary>
		/// Particle System.
		/// </summary>
		public ParticleSystem rainLightParticle, rainMediumParticle, rainHeavyParticle, snowParticle;

		/// <summary>
		/// List containing each thunder settings. When the instance is created, it gets one of the settings from this list.
		/// </summary>
		public List<AzureThunderSettings> thunderSettingsList = new List<AzureThunderSettings>();

		private void Start()
		{
			UpdateParticlesMaterials();
			UpdateParticlesPosition();
		}
		
		private void Update()
		{
			UpdateParticlesMaterials();
			UpdateParticlesPosition();
			if (Application.isPlaying)
			{
				SoundEffectController(azureSky.settings.RainLightVolume, rainLight);
				SoundEffectController(azureSky.settings.RainMediumVolume, rainMedium);
				SoundEffectController(azureSky.settings.RainHeavyVolume, rainHeavy);
				SoundEffectController(azureSky.settings.WindLightVolume, windLight);
				SoundEffectController(azureSky.settings.WindMediumVolume, windMedium);
				SoundEffectController(azureSky.settings.WindHeavyVolume, windHeavy);

				ParticleEffectController(azureSky.settings.RainLightIntensity * 4000.0f, rainLightParticle);
				ParticleEffectController(azureSky.settings.RainMediumIntensity * 4000.0f, rainMediumParticle);
				ParticleEffectController(azureSky.settings.RainHeavyIntensity * 2000.0f, rainHeavyParticle);
				ParticleEffectController(azureSky.settings.SnowIntensity * 2000.0f, snowParticle);

				windZone.windMain = azureSky.settings.WindSpeed * windMultiplier;
				m_windDirection = new Vector3(0.0f, Mathf.Lerp(-180.0f, 180.0f, azureSky.settings.WindDirection), 0.0f);
				windZone.transform.rotation = Quaternion.Euler(m_windDirection);
			}
		}
		
		/// <summary>
		/// Start and stop the sounds effect.
		/// </summary>
		private void SoundEffectController(float volume, AudioSource sound)
		{
			sound.volume = volume;
			if (volume > 0)
			{
				if (!sound.isPlaying) sound.Play ();
			}
			else if (sound.isPlaying) sound.Stop ();
		}
		
		/// <summary>
		/// Start and stop the sounds effect.
		/// </summary>
		private void ParticleEffectController(float intensity, ParticleSystem particle)
		{
			var emission = particle.emission;
			emission.rateOverTimeMultiplier = intensity;
			if (intensity > 0)
			{
				if (!particle.isPlaying) particle.Play ();
			}
			else if (particle.isPlaying) particle.Stop ();
		}

		/// <summary>
		/// Updates each particle group from the 'particlesDriverList' to their respective target position.
		/// </summary>
		private void UpdateParticlesPosition()
		{
			if (particlesDriverList.Count <= 0) return;
			for (int i = 0; i < particlesDriverList.Count; i++)
			{
				particlesDriverList[i].particleTransform.position = particlesDriverList[i].followTargetTransform.position;
			}
		}

		/// <summary>
		/// Updates the particles color.
		/// </summary>
		private void UpdateParticlesMaterials()
		{
			particleRainMaterial.SetColor("_TintColor", azureSky.settings.RainColor);
			particleHeavyRainMaterial.SetColor("_TintColor", azureSky.settings.RainColor);
			particleSnowMaterial.SetColor("_TintColor", azureSky.settings.SnowColor);
			particleRippleMaterial.SetColor("_TintColor", azureSky.settings.RainColor);
		}

		/// <summary>
		/// Create a thunder effect in the scene. When the thunder sound is over, the instance is automatically deleted.
		/// </summary>
		public void InstantiateThunderEffect(int index)
		{
			Transform thunder = Instantiate(thunderSettingsList[index].thunderPrefab, thunderSettingsList[index].position, thunderSettingsList[index].thunderPrefab.rotation);
			AzureThunderEffect thunderEffect = thunder.GetComponent<AzureThunderEffect>();
			thunderEffect.audioClip = thunderSettingsList[index].audioClip;
			thunderEffect.audioDelay = thunderSettingsList[index].audioDelay;
			thunderEffect.lightFrequency = thunderSettingsList[index].lightFrequency;
		}
		
		/// <summary>
		/// Create a thunder effect in the scene. When the thunder sound is over, the instance is automatically deleted.
		/// </summary>
		public void InstantiateThunderEffect(int index, Vector3 worldPos)
		{
			Transform thunder = Instantiate(thunderSettingsList[index].thunderPrefab, worldPos, thunderSettingsList[index].thunderPrefab.rotation);
			AzureThunderEffect thunderEffect = thunder.GetComponent<AzureThunderEffect>();
			thunderEffect.audioClip = thunderSettingsList[index].audioClip;
			thunderEffect.audioDelay = thunderSettingsList[index].audioDelay;
			thunderEffect.lightFrequency = thunderSettingsList[index].lightFrequency;
		}
	}
}