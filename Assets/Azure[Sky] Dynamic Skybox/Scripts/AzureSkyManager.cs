using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.AzureSky
{
	[ExecuteInEditMode]
	[AddComponentMenu("Azure[Sky]/Sky Manager")]
	public sealed class AzureSkyManager : MonoBehaviour
	{
		// Not included in build
		#if UNITY_EDITOR
		public AzureEditorSettings editorSettings;
		#endif
		
		private const float Pi = Mathf.PI;
		
		// Time Controller
		/// <summary>
		/// Instance of AzureTimeController used to set the time of day and compute sun and moon position.
		/// </summary>
		public AzureTimeController timeController = new AzureTimeController();
		
		private float m_timeProgression;
		private Vector3 m_sunLocalDirection, m_moonLocalDirection;
		private Vector3 m_sunRealisticRotation, m_moonRealisticRotation;
		private Quaternion m_sunSimpleRotation, m_moonSimpleRotation;
		private float m_sunElevation, m_moonElevation;
		private Vector3 m_starFieldOffset = Vector3.zero;
		private Matrix4x4 m_starFieldMatrix;
		private AzureSkyProfile m_nextDayProfile;
		private bool m_isTimelineTransitionInProgress = false;
		private float m_timelineSourceTransitionTime, m_timelineDestinationTransitionTime, m_startTimelineTransitionStep, m_timelineTransitionStep, m_timelineTransitionSpeed;
		
		// References
		public Transform sunTransform, moonTransform;
		public Light directionalLight;
		public Material skyMaterial, fogMaterial;
		public Texture2D sunTexture, moonTexture, starFieldTexture, dynamicCloudNoiseTexture, precomputedTransmittanceTexture;
		public Texture3D precomputedInScatterTexture;
		public Cubemap starNoiseTexture;
		public ReflectionProbe reflectionProbe;
		
		// Profile Controller
		/// <summary>
		/// Instance of AzureProfileController used to store day profiles and perform profile blend transitions.
		/// </summary>
		public AzureProfileController profileController = new AzureProfileController();
		/// <summary>
		/// Stores the current sky settings after performing all profile blend transitions.
		/// </summary>
		public AzureSkySettings settings = new AzureSkySettings();

		public int weatherIndex = -1;
		
		// Event System
		/// <summary>
		/// List used to store all Event Actions created by the user.
		/// </summary>
		public List<AzureEventAction> eventSystemList = new List<AzureEventAction>();
		public enum AzureEventScanMode
		{
			EveryMinute,
			Hourly
		}

		public AzureEventScanMode eventScanMode = AzureEventScanMode.EveryMinute;
		private Vector2 m_currentEventTime = Vector2.zero;
		private Vector2 m_previousEventTime = Vector2.zero;
		
		// Options
		/// <summary>
		/// Instance of AzureOptions used to store the sky system settings.
		/// </summary>
		public AzureOptions options = new AzureOptions();

		// Output System
		/// <summary>
		/// List used to store all outputs created by the user.
		/// </summary>
		public List<AzureOutput> outputSystemList = new List<AzureOutput>();
		
		/// <summary>
		/// The Sky Manager will create and perform the outputs only to the profiles using this same tag.
		/// </summary>
		public string outputTag = "Untagged";
		
		// Outer space utilities
		private float m_noiseRotationSpeed = 0.0f;
		private Quaternion m_noiseRotation;
		private Matrix4x4 m_noiseRotationMatrix;
		
		// Clouds utilities
		private float m_staticCloudLayer1Speed, m_staticCloudLayer2Speed;
		
		// Lighting utilities
		private float m_timeSinceLastProbeUpdate;

		private void Reset()
		{
			timeController.UpdateCalendar();
		}

		private void OnEnable()
		{
			RenderSettings.skybox = skyMaterial;
		}

		private void Start()
		{
			// Calculates the progression step to move the timeline
			m_timeProgression = timeController.GetTimeProgressionStep ();
			
			// Initialize the event time checker
			m_previousEventTime = timeController.GetTimeOfDay();
			m_currentEventTime = timeController.GetTimeOfDay();
			
			// Updates the calendar
			timeController.UpdateCalendar();
			
			// Always start the scene using the first profile from the default profiles list
			profileController.defaultProfile = profileController.defaultProfileList[0];
			profileController.currentProfile = profileController.defaultProfile;
			profileController.targetProfile = profileController.defaultProfile;
			
			// Gets the first sky settings after performing all the profiles blends
			EvaluateCurveAndGradientTime();
			settings = profileController.GetProfileSettings();
			
			// First sky shader update and material shader definition
			//profileController.UpdateStaticCloudTextures();
			UpdateShaderUniforms();
			SetMaterialsShader();
			
			// First reflection probe update
			if (options.reflectionProbeMode == AzureOptions.ReflectionProbeMode.On)
			{
				if (options.reflectionProbeUpdateAtFirstFrame || options.reflectionProbeRefreshMode == Rendering.ReflectionProbeRefreshMode.EveryFrame)
				{
					reflectionProbe.RenderProbe ();
				}
			}
		}

		private void Update()
		{
			// Only in gameplay
			if (Application.isPlaying)
			{
				// Moves the timeline
				timeController.timeline += m_timeProgression * Time.deltaTime;
				
				// Timeline transition
				if(m_isTimelineTransitionInProgress) DoTimelineTransition(m_timelineSourceTransitionTime, m_timelineDestinationTransitionTime);
				
				// Changes to the next day
				if (timeController.timeline > 24)
				{
					timeController.StartNextDay ();

					// Make the transition to the next day only if the profile is different from the current profile and there is no weather profile in use.
					m_nextDayProfile = profileController.defaultProfileList[Random.Range(0, profileController.defaultProfileList.Count)];
					profileController.defaultProfile = m_nextDayProfile;
					if (m_nextDayProfile != profileController.currentProfile && weatherIndex < 0)
					{
						if (options.doDayTransition)
						{
							SetNewDayProfile(m_nextDayProfile, options.dayTransitionTime);
						}
						else
						{
							SetNewDayProfile(m_nextDayProfile);
						}
					}
				}
				
				// Scan the event system list and perform the event actions if necessary
				if (TimeHasChanged() && eventSystemList.Count > 0)
				{
					ScanEventSystemList();
					m_previousEventTime = timeController.GetTimeOfDay();
				}
				
				// Rotates the static cloud layers
				m_staticCloudLayer1Speed += settings.StaticCloudLayer1Speed * Time.deltaTime;
				m_staticCloudLayer2Speed += settings.StaticCloudLayer2Speed * Time.deltaTime;
				if (m_staticCloudLayer1Speed >= 1.0f) { m_staticCloudLayer1Speed -= 1.0f; }
				if (m_staticCloudLayer2Speed >= 1.0f) { m_staticCloudLayer2Speed -= 1.0f; }
			}
			
			// Applies the rotation to the sun and moon transform always using the local space coordinates
			switch (timeController.timeMode)
			{
				case AzureTimeController.TimeMode.Simple:
					
					m_sunSimpleRotation = timeController.GetSunSimpleRotation();
					sunTransform.localRotation = m_sunSimpleRotation;
					m_moonSimpleRotation = m_sunSimpleRotation * Quaternion.Euler(0, -180, 0);
					moonTransform.localRotation = m_moonSimpleRotation;

					m_starFieldMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(options.starFieldPosition), Vector3.one).inverse * sunTransform.transform.worldToLocalMatrix;
					Shader.SetGlobalMatrix(AzureShaderUniforms.StarFieldMatrix, m_starFieldMatrix);
					break;
				
				case AzureTimeController.TimeMode.Realistic:
					
					m_sunRealisticRotation = timeController.GetSunRealisticRotation();
					sunTransform.forward = transform.TransformDirection(m_sunRealisticRotation);
					m_moonRealisticRotation = timeController.GetMoonRealisticRotation();
					moonTransform.forward = transform.TransformDirection(m_moonRealisticRotation);
					
					m_starFieldOffset.y = timeController.longitude;
					m_starFieldMatrix = Matrix4x4.TRS(Vector3.zero, timeController.GetCelestialRotation() * Quaternion.Euler(options.starFieldPosition - m_starFieldOffset), Vector3.one);
					Shader.SetGlobalMatrix(AzureShaderUniforms.StarFieldMatrix, m_starFieldMatrix.inverse);
					break;
			}
			
			// Gets the local direction of the sun and moon
			m_sunLocalDirection = transform.InverseTransformDirection(sunTransform.forward);
			m_moonLocalDirection = transform.InverseTransformDirection(moonTransform.forward);
			
			// Gets the sun and moon elevation and sets the directional light rotation
			m_sunElevation = Vector3.Dot(-m_sunLocalDirection, Vector3.up);
			m_moonElevation = Vector3.Dot(-m_moonLocalDirection, Vector3.up);
			directionalLight.transform.localRotation = Quaternion.LookRotation(m_sunElevation >= 0.0f ? m_sunLocalDirection : m_moonLocalDirection);
			
			// Gets the sky settings and output values after performing all the profiles blends
			// Computes curve and gradient times and send it to profile controller to do the evaluation process
			// Sends the output list and output tag to profile controller to do the profile blends and weather transitions process
			// The profile controller will perform the profile blends and return the resulting values to "settings"
			// The profile controller will send back the output list with resulting values after performing the profile blends and weather transitions process
			EvaluateCurveAndGradientTime();
			profileController.outputSystemList = outputSystemList;
			profileController.outputTag = outputTag;
			settings = profileController.GetProfileSettings();
			outputSystemList = profileController.outputSystemList;
			
			// Stars scintillation
			if (settings.RegularStarsScintillation > 0.0f)
			{
				m_noiseRotationSpeed += settings.RegularStarsScintillation * 10.0f * Time.deltaTime;
				m_noiseRotation = Quaternion.Euler(m_noiseRotationSpeed, m_noiseRotationSpeed, m_noiseRotationSpeed);
				m_noiseRotationMatrix = Matrix4x4.TRS(Vector3.zero, m_noiseRotation, new Vector3(1, 1, 1));
			}
			
			// Environment lighting
			directionalLight.intensity = settings.DirectionalLightIntensity;
			directionalLight.color = settings.DirectionalLightColor;
			RenderSettings.ambientIntensity = settings.AmbientIntensity;
			RenderSettings.ambientLight = settings.AmbientColor;
			RenderSettings.ambientSkyColor = settings.AmbientSkyColor;
			RenderSettings.ambientEquatorColor = settings.AmbientEquatorColor;
			RenderSettings.ambientGroundColor = settings.AmbientGroundColor;
			
			// Update shader uniforms
			UpdateShaderUniforms();

			// Update the sky reflection probe after performing all sky changes
			if (options.reflectionProbeMode == AzureOptions.ReflectionProbeMode.On && reflectionProbe.gameObject.activeSelf)
			{
				reflectionProbe.intensity = settings.SkyReflectionIntensity;
				m_timeSinceLastProbeUpdate += Time.deltaTime;
				if (options.reflectionProbeRefreshMode == Rendering.ReflectionProbeRefreshMode.ViaScripting)
				{
					if (m_timeSinceLastProbeUpdate >= options.reflectionProbeUpdateTime)
					{
						reflectionProbe.RenderProbe ();
						m_timeSinceLastProbeUpdate = 0;
					}
				}
			}
		}

		/// <summary>
		/// Changes the global weather with a smooth transition.
		/// </summary>
		/// Set the index to -1 if you want to reset the global weather back to the default day profile.
		/// <param name="index">The target profile number in the "global weather profiles" list.</param>
		public void SetNewWeatherProfile(int index)
		{
			switch (index)
			{
				// Back to the default day profile currently in use by sky manager
				case -1:
					if (profileController.defaultProfile)
					{
						profileController.targetProfile = profileController.defaultProfile;
						profileController.transitionTime = profileController.defaultProfileTransitionTime;
						weatherIndex = index;
					}
					break;
				
				// Changes the global weather to the corresponding profile index in the global weather list
				default:
					if (profileController.weatherProfileList[index].profile)
					{
						profileController.targetProfile = profileController.weatherProfileList[index].profile;
						profileController.transitionTime = profileController.weatherProfileList[index].transitionTime;
						weatherIndex = index;
					}
					break;
			}
			
			// Starts the global weather transition progress
			profileController.transitionProgress = 0.0f;
			profileController.startTransitionTime = Time.time;
			profileController.isWeatherTransition = true;
		}

		/// <summary>
		/// Changes the current day profile with transition.
		/// </summary>
		public void SetNewDayProfile(AzureSkyProfile profile, float transitionTime)
		{
			profileController.targetProfile = profile;
			profileController.transitionTime = transitionTime;
						
			// Starts the global weather transition progress
			profileController.transitionProgress = 0.0f;
			profileController.startTransitionTime = Time.time;
			profileController.isWeatherTransition = true;
		}
		
		/// <summary>
		/// Changes the current day profile without transition.
		/// </summary>
		public void SetNewDayProfile(AzureSkyProfile profile)
		{
			profileController.currentProfile = profile;
		}

		/// <summary>
		/// Total rayleigh computation.
		/// </summary>
		private Vector3 ComputeRayleigh()
		{
			Vector3 rayleigh = Vector3.one;
			Vector3 lambda = settings.Wavelength * 1e-9f;
			float n = 1.0003f; // Refractive index of air
			float pn = 0.035f; // Depolarization factor for standard air.
			float n2 = n * n;
			//float N = 2.545E25f;
			float N = settings.MolecularDensity;
			float temp = (8.0f * Pi*Pi*Pi * ((n2 - 1.0f) * (n2 - 1.0f))) / (3.0f * N * 1E25f) * ((6.0f + 3.0f * pn) / (6.0f - 7.0f * pn));
			
			rayleigh.x = temp / Mathf.Pow(lambda.x, 4.0f);
			rayleigh.y = temp / Mathf.Pow(lambda.y, 4.0f);
			rayleigh.z = temp / Mathf.Pow(lambda.z, 4.0f);

			return rayleigh;
		}

		/// <summary>
		/// Total mie computation.
		/// </summary>
		private Vector3 ComputeMie()
		{
			Vector3 mie;
			
			//float c = (0.6544f * Turbidity - 0.6510f) * 1e-16f;
			float c = (0.6544f * 5.0f - 0.6510f) * 10f * 1e-9f;
			Vector3 k = new Vector3(686.0f, 678.0f, 682.0f);
			
			mie.x = (434.0f * c * Pi * Mathf.Pow((4.0f * Pi) / settings.Wavelength.x, 2.0f) * k.x);
			mie.y = (434.0f * c * Pi * Mathf.Pow((4.0f * Pi) / settings.Wavelength.y, 2.0f) * k.y);
			mie.z = (434.0f * c * Pi * Mathf.Pow((4.0f * Pi) / settings.Wavelength.z, 2.0f) * k.z);
			
			//float c = (6544f * 5.0f - 6510f) * 10.0f * 1.0e-9f;
			
			//mie.x = (0.434f * c * Pi * Mathf.Pow((2.0f * Pi) / settings.Wavelength.x, 2.0f) * settings.K.x) / 3.0f;
			//mie.y = (0.434f * c * Pi * Mathf.Pow((2.0f * Pi) / settings.Wavelength.y, 2.0f) * settings.K.y) / 3.0f;
			//mie.z = (0.434f * c * Pi * Mathf.Pow((2.0f * Pi) / settings.Wavelength.z, 2.0f) * settings.K.z) / 3.0f;
			
			return mie;
		}

		/// <summary>
		/// Computes the 'time factors' to evaluate curves and gradients.
		/// </summary>
		private void EvaluateCurveAndGradientTime()
		{
			// Curves
			profileController.evaluateCurveTimeline = timeController.timeOfDay;
			profileController.evaluateCurveSunElevation = m_sunElevation;
			profileController.evaluateCurveMoonElevation = m_moonElevation;
			// Gradients
			profileController.evaluateGradientTimeline = timeController.timeOfDay / 24.0f;
			profileController.evaluateGradientSunElevation = Mathf.InverseLerp(-1.0f, 1.0f, m_sunElevation);
			profileController.evaluateGradientMoonElevation = Mathf.InverseLerp(-1.0f, 1.0f, m_moonElevation);
		}
		
		/// <summary>
		/// Checks if the time of day has changed.
		/// </summary>
		private bool TimeHasChanged()
		{
			m_currentEventTime = timeController.GetTimeOfDay();
			switch (eventScanMode)
			{
				case AzureEventScanMode.EveryMinute:
					if (Math.Abs(m_currentEventTime.x - m_previousEventTime.x) > 0 || Math.Abs(m_currentEventTime.y - m_previousEventTime.y) > 0)
					{
						return true;
					}
					break;
				case AzureEventScanMode.Hourly:
					if (Math.Abs(m_currentEventTime.x - m_previousEventTime.x) > 0)
					{
						return true;
					}
					break;
			}

			return false;
		}

		/// <summary>
		/// Scans the event system list and perform the event actions that match with the current date and time.
		/// </summary>
		private void ScanEventSystemList()
		{
			for (int i = 0; i < eventSystemList.Count; i++)
			{
				if(eventSystemList[i].year != timeController.year && eventSystemList[i].year != -1)
					continue;
				if(eventSystemList[i].month != timeController.month && eventSystemList[i].month != -1)
					continue;
				if(eventSystemList[i].day != timeController.day && eventSystemList[i].day != -1)
					continue;
				if(Math.Abs(eventSystemList[i].hour - m_currentEventTime.x) > 0 && eventSystemList[i].hour != -1)
					continue;
				if(Math.Abs(eventSystemList[i].minute - m_currentEventTime.y) > 0 && eventSystemList[i].minute != -1)
					continue;
				eventSystemList[i].eventAction.Invoke();
			}
		}

		public void SetTimelineTransitionTime(float transitionTime)
		{
			m_timelineTransitionSpeed = transitionTime;
		}

		public void SetTimelineSourceTransitionTime(float source)
		{
			m_timelineSourceTransitionTime = source;
		}
		
		public void SetTimelineDestinationTransitionTime(float destination)
		{
			m_timelineDestinationTransitionTime = destination;
		}
		
		/// <summary>
		/// Transition the timeline from source to destination in a period of time (in seconds).
		/// </summary>
		public void StartTimelineTransition(float source, float destination, float transitionTime)
		{
			m_timelineTransitionSpeed = transitionTime;
			m_startTimelineTransitionStep = Time.time;
			m_timelineSourceTransitionTime = source;
			m_timelineDestinationTransitionTime = destination;
			m_isTimelineTransitionInProgress = true;
		}
		
		/// <summary>
		/// Transition the timeline from current time of day to destination in a period of time (in seconds).
		/// </summary>
		public void StartTimelineTransition(float destination, float transitionTime)
		{
			m_timelineTransitionSpeed = transitionTime;
			m_startTimelineTransitionStep = Time.time;
			m_timelineSourceTransitionTime = timeController.timeline;
			m_timelineDestinationTransitionTime = destination;
			m_isTimelineTransitionInProgress = true;
		}
		
		/// <summary>
		/// Transition the timeline from current time of day to destination in a period of time (in seconds).
		/// </summary>
		public void StartTimelineTransition(float destination)
		{
			m_startTimelineTransitionStep = Time.time;
			m_timelineSourceTransitionTime = timeController.timeline;
			m_timelineDestinationTransitionTime = destination;
			m_isTimelineTransitionInProgress = true;
		}
		
		/// <summary>
		/// Transition the timeline from current time of day to destination in a period of time (in seconds).
		/// </summary>
		public void StartTimelineTransition()
		{
			m_startTimelineTransitionStep = Time.time;
			m_isTimelineTransitionInProgress = true;
		}
		
		private void DoTimelineTransition(float source, float destination)
		{
			// Computes the lerp t=time
			m_timelineTransitionStep = Mathf.Clamp01((Time.time - m_startTimelineTransitionStep) / m_timelineTransitionSpeed);
         
			// Apply the lerp transition to Azure[Sky] timeline
			timeController.timeline = Mathf.Lerp(source, destination, m_timelineTransitionStep);

			// Ends the lerp transition
			if (Mathf.Abs(m_timelineTransitionStep - 1.0f) <= 0.0f)
			{
				m_isTimelineTransitionInProgress = false;
			}
		}

		public void SetMaterialsShader()
		{
			switch (options.skyModel)
			{
				case AzureOptions.SkyModel.Stylized:
					fogMaterial.shader = Shader.Find("Azure[Sky]/Fog Scattering");
					switch (options.cloudMode)
					{
						case AzureOptions.CloudMode.Off:
							skyMaterial.shader = Shader.Find("Azure[Sky]/Empty Sky");
							break;
						case AzureOptions.CloudMode.Static:
							skyMaterial.shader = Shader.Find("Azure[Sky]/Static Clouds");
							break;
						case AzureOptions.CloudMode.StaticMultiLayer:
							skyMaterial.shader = Shader.Find("Azure[Sky]/Static Multi Layer Clouds");
							break;
						case AzureOptions.CloudMode.Dynamic:
							skyMaterial.shader = Shader.Find("Azure[Sky]/Dynamic Clouds");
							break;
						default:
							skyMaterial.shader = Shader.Find("Azure[Sky]/Empty Sky");
							break;
					}
					break;
				
				case AzureOptions.SkyModel.Precomputed:
					fogMaterial.shader = Shader.Find("Azure[Sky]/Precomputed Fog Scattering");
					skyMaterial.shader = Shader.Find("Azure[Sky]/Precomputed Sky");
					break;
			}
		}

		/// <summary>
		/// Updates the shader uniforms every frame.
		/// </summary>
		private void UpdateShaderUniforms()
		{
			// Textures
			Shader.SetGlobalTexture(AzureShaderUniforms.SunTexture, sunTexture);
			Shader.SetGlobalTexture(AzureShaderUniforms.MoonTexture, moonTexture);
			Shader.SetGlobalTexture(AzureShaderUniforms.DynamicCloudNoiseTexture, dynamicCloudNoiseTexture);
			Shader.SetGlobalTexture(AzureShaderUniforms.StarFieldTexture, starFieldTexture);
			Shader.SetGlobalTexture(AzureShaderUniforms.StarNoiseTexture, starNoiseTexture);
			Shader.SetGlobalTexture(AzureShaderUniforms.TransmittanceTexture, precomputedTransmittanceTexture);
			Shader.SetGlobalTexture(AzureShaderUniforms.InScatterTexture, precomputedInScatterTexture);
			
			// Directions
			Shader.SetGlobalVector (AzureShaderUniforms.SunDirection, -m_sunLocalDirection);
			Shader.SetGlobalVector (AzureShaderUniforms.MoonDirection, -m_moonLocalDirection);
			Shader.SetGlobalMatrix(AzureShaderUniforms.SunMatrix, sunTransform.worldToLocalMatrix);
			Shader.SetGlobalMatrix(AzureShaderUniforms.MoonMatrix, moonTransform.worldToLocalMatrix);
			Shader.SetGlobalMatrix(AzureShaderUniforms.UpDirectionMatrix, transform.worldToLocalMatrix);
			Shader.SetGlobalMatrix(AzureShaderUniforms.NoiseRotationMatrix, m_noiseRotationMatrix);
			
			// Scattering
			Shader.SetGlobalInt (AzureShaderUniforms.StylizedSunsetMode, (int)options.stylizedSunsetMode);
			Shader.SetGlobalInt(AzureShaderUniforms.SkyModel, options.skyModel == AzureOptions.SkyModel.Stylized ? 0 : 1);
			Shader.SetGlobalVector (AzureShaderUniforms.Rayleigh, ComputeRayleigh() * settings.RayleighMultiplier);
			Shader.SetGlobalVector (AzureShaderUniforms.Mie, ComputeMie() * settings.MieMultiplier);
			Shader.SetGlobalFloat (AzureShaderUniforms.ScatteringIntensity, settings.ScatteringIntensity * 60.0f);
			Shader.SetGlobalFloat (AzureShaderUniforms.SkyLuminance, settings.SkyLuminance);
			Shader.SetGlobalVector (AzureShaderUniforms.RayleighColor, settings.RayleighColor);
			Shader.SetGlobalVector (AzureShaderUniforms.MieColor, settings.MieColor);
			Shader.SetGlobalVector (AzureShaderUniforms.TransmittanceColor, settings.TransmittanceColor);
			Shader.SetGlobalFloat (AzureShaderUniforms.Exposure, settings.Exposure);
			Shader.SetGlobalFloat (AzureShaderUniforms.EarthSize, 6360.0f * options.earthScale);
			Shader.SetGlobalFloat (AzureShaderUniforms.AtmosphereSize, 6420.0f * options.earthScale);
			Shader.SetGlobalVector (AzureShaderUniforms.EarthPosition, options.earthPosition * options.earthScale);
			Shader.SetGlobalFloat (AzureShaderUniforms.PrecomputedSunIntensity, settings.PrecomputedSunIntensity);
			Shader.SetGlobalFloat (AzureShaderUniforms.PrecomputedMoonIntensity, settings.PrecomputedMoonIntensity);
			
			// Outer space
			Shader.SetGlobalFloat (AzureShaderUniforms.SunTextureSize, settings.SunTextureSize);
			Shader.SetGlobalFloat (AzureShaderUniforms.SunTextureIntensity, settings.SunTextureIntensity * 3.0f);
			Shader.SetGlobalVector (AzureShaderUniforms.SunTextureColor, settings.SunTextureColor);
			Shader.SetGlobalFloat (AzureShaderUniforms.MoonTextureSize, settings.MoonTextureSize);
			Shader.SetGlobalFloat (AzureShaderUniforms.MoonTextureIntensity, settings.MoonTextureIntensity * 3.0f);
			Shader.SetGlobalVector (AzureShaderUniforms.MoonTextureColor, settings.MoonTextureColor);
			Shader.SetGlobalFloat (AzureShaderUniforms.RegularStarsScintillation, settings.RegularStarsScintillation);
			Shader.SetGlobalFloat (AzureShaderUniforms.RegularStarsIntensity, settings.RegularStarsIntensity * 5.0f);
			Shader.SetGlobalFloat (AzureShaderUniforms.MilkyWayIntensity, settings.MilkyWayIntensity);
			Shader.SetGlobalVector (AzureShaderUniforms.StarFieldColorBalance, options.starFieldColor);
			
			// Fog scattering
			Shader.SetGlobalFloat (AzureShaderUniforms.FogScatteringScale, settings.FogScatteringScale);
			Shader.SetGlobalFloat (AzureShaderUniforms.GlobalFogSmooth, settings.GlobalFogSmooth);
			Shader.SetGlobalFloat (AzureShaderUniforms.GlobalFogDensity, settings.GlobalFogDensity);
			Shader.SetGlobalFloat (AzureShaderUniforms.GlobalFogDistance, settings.GlobalFogDistance);
			Shader.SetGlobalFloat (AzureShaderUniforms.HeightFogSmooth, settings.HeightFogSmooth);
			Shader.SetGlobalFloat (AzureShaderUniforms.HeightFogDensity, settings.HeightFogDensity);
			Shader.SetGlobalFloat (AzureShaderUniforms.HeightFogDistance, settings.HeightFogDistance);
			Shader.SetGlobalFloat (AzureShaderUniforms.HeightFogStart, settings.HeightFogStart);
			Shader.SetGlobalFloat (AzureShaderUniforms.HeightFogEnd, settings.HeightFogEnd);

			// Clouds
			Shader.SetGlobalFloat (AzureShaderUniforms.StaticCloudInterpolator, settings.StaticCloudInterpolator);
			Shader.SetGlobalTexture(AzureShaderUniforms.StaticCloudSourceTexture, profileController.staticCloudSource);
			Shader.SetGlobalTexture(AzureShaderUniforms.StaticCloudTargetTexture, profileController.staticCloudTarget);
			Shader.SetGlobalFloat (AzureShaderUniforms.StaticLowCloudSpeed, m_staticCloudLayer1Speed);
			Shader.SetGlobalFloat (AzureShaderUniforms.StaticHigCloudSpeed, m_staticCloudLayer2Speed);
			Shader.SetGlobalVector (AzureShaderUniforms.StaticCloudColor, settings.StaticCloudColor);
			Shader.SetGlobalFloat (AzureShaderUniforms.StaticCloudScattering, settings.StaticCloudScattering);
			Shader.SetGlobalFloat (AzureShaderUniforms.StaticCloudExtinction, settings.StaticCloudExtinction);
			Shader.SetGlobalFloat (AzureShaderUniforms.StaticCloudDensity, settings.StaticCloudSaturation);
			Shader.SetGlobalFloat (AzureShaderUniforms.StaticCloudOpacity, settings.StaticCloudOpacity);
			Shader.SetGlobalFloat (AzureShaderUniforms.DynamicCloudAltitude, settings.DynamicCloudAltitude);
			Shader.SetGlobalFloat (AzureShaderUniforms.DynamicCloudDirection, settings.DynamicCloudDirection);
			Shader.SetGlobalFloat (AzureShaderUniforms.DynamicCloudSpeed, settings.DynamicCloudSpeed);
			Shader.SetGlobalFloat (AzureShaderUniforms.DynamicCloudDensity, Mathf.Lerp (25.0f, 0.0f, settings.DynamicCloudDensity));
			Shader.SetGlobalColor (AzureShaderUniforms.DynamicCloudColor1, settings.DynamicCloudColor1);
			Shader.SetGlobalColor (AzureShaderUniforms.DynamicCloudColor2, settings.DynamicCloudColor2);
			
			// Weather
			Shader.SetGlobalFloat (AzureShaderUniforms.ThunderMultiplier, settings.ThunderMultiplier);
		}
	}
}