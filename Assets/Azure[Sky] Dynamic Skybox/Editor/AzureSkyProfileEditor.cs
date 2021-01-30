using UnityEngine;
using UnityEngine.AzureSky;
using UnityEditorInternal;

namespace UnityEditor.AzureSky
{
	[CustomEditor(typeof(AzureSkyProfile))]
	public class AzureSkyProfileEditor : Editor
	{
		private AzureSkyProfile m_target;
		
		// Header bar foldout strings
		private string m_scatteringFoldout;
		private string m_outerSpaceFoldout;
		private string m_fogScatteringFoldout;
		private string m_cloudsFoldout;
		private string m_lightingFoldout;
		private string m_weatherFoldout;
		private string m_outputsFoldout;
		
		// Custom InspectorGUI
		private Rect m_controlRect;
		private Texture2D m_logoTexture;
		private GUIStyle m_headerStyle;
		private readonly int m_labelWidth = 88;
		private readonly int m_propertyTypeWidth = 50;
		private readonly Color m_customGreenColor = new Color(0.75f, 1.0f, 0.75f);
		private readonly Color m_customRedColor = new Color(1.0f, 0.5f, 0.5f);
		
		// Serialized Properties
		private SerializedProperty m_MolecularDensity;
		private SerializedProperty m_WavelengthR;
		private SerializedProperty m_WavelengthG;
		private SerializedProperty m_WavelengthB;
		private SerializedProperty m_RayleighMultiplier;
		private SerializedProperty m_MieMultiplier;
		private SerializedProperty m_ScatteringIntensity;
		private SerializedProperty m_SkyLuminance;
		private SerializedProperty m_RayleighColor;
		private SerializedProperty m_MieColor;
		private SerializedProperty m_SunsetColor;
		private SerializedProperty m_PrecomputedSunIntensity;
		private SerializedProperty m_PrecomputedMoonIntensity;
		private SerializedProperty m_Exposure;
		private SerializedProperty m_SunTextureSize;
		private SerializedProperty m_SunTextureIntensity;
		private SerializedProperty m_SunTextureColor;
		private SerializedProperty m_MoonTextureSize;
		private SerializedProperty m_MoonTextureIntensity;
		private SerializedProperty m_MoonTextureColor;
		private SerializedProperty m_RegularStarsScintillation;
		private SerializedProperty m_RegularStarsIntensity;
		private SerializedProperty m_MilkyWayIntensity;
		private SerializedProperty m_GlobalFogSmooth;
		private SerializedProperty m_FogScatteringScale;
		private SerializedProperty m_GlobalFogDensity;
		private SerializedProperty m_GlobalFogDistance;
		private SerializedProperty m_HeightFogSmooth;
		private SerializedProperty m_HeightFogDensity;
		private SerializedProperty m_HeightFogDistance;
		private SerializedProperty m_HeightFogStart;
		private SerializedProperty m_HeightFogEnd;
		private SerializedProperty m_StaticCloudLayer1Speed;
		private SerializedProperty m_StaticCloudLayer2Speed;
		private SerializedProperty m_StaticCloudColor;
		private SerializedProperty m_StaticCloudScattering;
		private SerializedProperty m_StaticCloudExtinction;
		private SerializedProperty m_StaticCloudSaturation;
		private SerializedProperty m_StaticCloudOpacity;
		private SerializedProperty m_DynamicCloudAltitude;
		private SerializedProperty m_DynamicCloudDirection;
		private SerializedProperty m_DynamicCloudSpeed;
		private SerializedProperty m_DynamicCloudDensity;
		private SerializedProperty m_DynamicCloudColor1;
		private SerializedProperty m_DynamicCloudColor2;
		private SerializedProperty m_DirectionalLightIntensity;
		private SerializedProperty m_DirectionalLightColor;
		private SerializedProperty m_AmbientIntensity;
		private SerializedProperty m_AmbientColor;
		private SerializedProperty m_AmbientSkyColor;
		private SerializedProperty m_AmbientEquatorColor;
		private SerializedProperty m_AmbientGroundColor;
		private SerializedProperty m_SkyReflectionIntensity;
		private SerializedProperty m_RainLightIntensity;
		private SerializedProperty m_RainMediumIntensity;
		private SerializedProperty m_RainHeavyIntensity;
		private SerializedProperty m_SnowIntensity;
		private SerializedProperty m_RainLightVolume;
		private SerializedProperty m_RainMediumVolume;
		private SerializedProperty m_RainHeavyVolume;
		private SerializedProperty m_WindLightVolume;
		private SerializedProperty m_WindMediumVolume;
		private SerializedProperty m_WindHeavyVolume;
		private SerializedProperty m_RainColor;
		private SerializedProperty m_SnowColor;
		private SerializedProperty m_WindSpeed;
		private SerializedProperty m_WindDirection;
		private SerializedProperty m_ThunderMultiplier;
		
		// Outputs list
		private SerializedProperty m_OutputsList;
		private ReorderableList m_reorderableOutputsList;

		private void OnEnable()
		{
			// Get Target
			m_target = (AzureSkyProfile) target;
			
			// Initialize serialized properties
			m_MolecularDensity = serializedObject.FindProperty("scattering.molecularDensity");
			m_WavelengthR = serializedObject.FindProperty("scattering.wavelengthR");
			m_WavelengthG = serializedObject.FindProperty("scattering.wavelengthG");
			m_WavelengthB = serializedObject.FindProperty("scattering.wavelengthB");
			m_RayleighMultiplier = serializedObject.FindProperty("scattering.rayleighMultiplier");
			m_MieMultiplier = serializedObject.FindProperty("scattering.mieMultiplier");
			m_ScatteringIntensity = serializedObject.FindProperty("scattering.scatteringIntensity");
			m_SkyLuminance = serializedObject.FindProperty("scattering.skyLuminance");
			m_RayleighColor = serializedObject.FindProperty("scattering.rayleighColor");
			m_MieColor = serializedObject.FindProperty("scattering.mieColor");
			m_SunsetColor = serializedObject.FindProperty("scattering.sunsetColor");
			m_PrecomputedSunIntensity = serializedObject.FindProperty("scattering.precomputedSunIntensity");
			m_PrecomputedMoonIntensity = serializedObject.FindProperty("scattering.precomputedMoonIntensity");
			m_Exposure = serializedObject.FindProperty("scattering.exposure");
			m_SunTextureSize = serializedObject.FindProperty("outerSpace.sunTextureSize");
			m_SunTextureIntensity = serializedObject.FindProperty("outerSpace.sunTextureIntensity");
			m_SunTextureColor = serializedObject.FindProperty("outerSpace.sunTextureColor");
			m_MoonTextureSize = serializedObject.FindProperty("outerSpace.moonTextureSize");
			m_MoonTextureIntensity = serializedObject.FindProperty("outerSpace.moonTextureIntensity");
			m_MoonTextureColor = serializedObject.FindProperty("outerSpace.moonTextureColor");
			m_RegularStarsScintillation = serializedObject.FindProperty("outerSpace.regularStarsScintillation");
			m_RegularStarsIntensity = serializedObject.FindProperty("outerSpace.regularStarsIntensity");
			m_MilkyWayIntensity = serializedObject.FindProperty("outerSpace.milkyWayIntensity");
			m_GlobalFogSmooth = serializedObject.FindProperty("fogScattering.globalFogSmooth");
			m_FogScatteringScale = serializedObject.FindProperty("fogScattering.fogScatteringScale");
			m_GlobalFogDensity = serializedObject.FindProperty("fogScattering.globalFogDensity");
			m_GlobalFogDistance = serializedObject.FindProperty("fogScattering.globalFogDistance");
			m_HeightFogSmooth = serializedObject.FindProperty("fogScattering.heightFogSmooth");
			m_HeightFogDensity = serializedObject.FindProperty("fogScattering.heightFogDensity");
			m_HeightFogDistance = serializedObject.FindProperty("fogScattering.heightFogDistance");
			m_HeightFogStart = serializedObject.FindProperty("fogScattering.heightFogStart");
			m_HeightFogEnd = serializedObject.FindProperty("fogScattering.heightFogEnd");
			m_StaticCloudLayer1Speed = serializedObject.FindProperty("clouds.staticCloudLayer1Speed");
			m_StaticCloudLayer2Speed = serializedObject.FindProperty("clouds.staticCloudLayer2Speed");
			m_StaticCloudColor = serializedObject.FindProperty("clouds.staticCloudColor");
			m_StaticCloudScattering = serializedObject.FindProperty("clouds.staticCloudScattering");
			m_StaticCloudExtinction = serializedObject.FindProperty("clouds.staticCloudExtinction");
			m_StaticCloudSaturation = serializedObject.FindProperty("clouds.staticCloudSaturation");
			m_StaticCloudOpacity = serializedObject.FindProperty("clouds.staticCloudOpacity");
			m_DynamicCloudAltitude = serializedObject.FindProperty("clouds.dynamicCloudAltitude");
			m_DynamicCloudDirection = serializedObject.FindProperty("clouds.dynamicCloudDirection");
			m_DynamicCloudSpeed = serializedObject.FindProperty("clouds.dynamicCloudSpeed");
			m_DynamicCloudDensity = serializedObject.FindProperty("clouds.dynamicCloudDensity");
			m_DynamicCloudColor1 = serializedObject.FindProperty("clouds.dynamicCloudColor1");
			m_DynamicCloudColor2 = serializedObject.FindProperty("clouds.dynamicCloudColor2");
			m_DirectionalLightIntensity = serializedObject.FindProperty("lighting.directionalLightIntensity");
			m_DirectionalLightColor = serializedObject.FindProperty("lighting.directionalLightColor");
			m_AmbientIntensity = serializedObject.FindProperty("lighting.ambientIntensity");
			m_AmbientColor = serializedObject.FindProperty("lighting.ambientColor");
			m_AmbientSkyColor = serializedObject.FindProperty("lighting.ambientSkyColor");
			m_AmbientEquatorColor = serializedObject.FindProperty("lighting.ambientEquatorColor");
			m_AmbientGroundColor = serializedObject.FindProperty("lighting.ambientGroundColor");
			m_SkyReflectionIntensity = serializedObject.FindProperty("lighting.skyReflectionIntensity");
			m_RainLightIntensity = serializedObject.FindProperty("weather.rainLightIntensity");
			m_RainMediumIntensity = serializedObject.FindProperty("weather.rainMediumIntensity");
			m_RainHeavyIntensity = serializedObject.FindProperty("weather.rainHeavyIntensity");
			m_SnowIntensity = serializedObject.FindProperty("weather.snowIntensity");
			m_RainLightVolume = serializedObject.FindProperty("weather.rainLightVolume");
			m_RainMediumVolume = serializedObject.FindProperty("weather.rainMediumVolume");
			m_RainHeavyVolume = serializedObject.FindProperty("weather.rainHeavyVolume");
			m_WindLightVolume = serializedObject.FindProperty("weather.windLightVolume");
			m_WindMediumVolume = serializedObject.FindProperty("weather.windMediumVolume");
			m_WindHeavyVolume = serializedObject.FindProperty("weather.windHeavyVolume");
			m_RainColor = serializedObject.FindProperty("weather.rainColor");
			m_SnowColor = serializedObject.FindProperty("weather.snowColor");
			m_WindSpeed = serializedObject.FindProperty("weather.windSpeed");
			m_WindDirection = serializedObject.FindProperty("weather.windDirection");
			m_ThunderMultiplier = serializedObject.FindProperty("weather.thunderMultiplier");
			
			// Create output list
			m_OutputsList = serializedObject.FindProperty("outputsList");
			m_reorderableOutputsList = new ReorderableList(serializedObject, m_OutputsList, false, true, false, false)
			{
				drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
				{
					var element = m_reorderableOutputsList.serializedProperty.GetArrayElementAtIndex(index);
					rect.y += 2;

					// Header title
					EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Output Item " + index.ToString(), m_headerStyle);
					
					rect.y += EditorGUIUtility.singleLineHeight +1;

					// Property
					switch (m_target.outputsList[index].azureOutputType)
					{
						case AzureOutputType.Slider:
							m_target.outputsList[index].slider = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),m_target.outputsList[index].slider,0.0f, 1.0f);
							break;
						case AzureOutputType.Vector2:
							EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("vector2"), GUIContent.none);
							break;
						case AzureOutputType.Vector3:
							EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("vector3"), GUIContent.none);
							break;
						case AzureOutputType.CurveTimeline:
							EditorGUI.CurveField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0, 0.0f, 24, 1.0f), GUIContent.none);
							break;
						case AzureOutputType.CurveSunElevation:
							EditorGUI.CurveField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(0, 0.0f, 1, 1.0f), GUIContent.none);
							break;
						case AzureOutputType.CurveMoonElevation:
							EditorGUI.CurveField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("moonElevationCurve"), Color.cyan, new Rect(0, 0.0f, 1, 1.0f), GUIContent.none);
							break;
						case AzureOutputType.Color:
							EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("color"), GUIContent.none);
							break;
						case AzureOutputType.GradientTimeline:
							EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("timelineGradient"), GUIContent.none);
							break;
						case AzureOutputType.GradientSunElevation:
							EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
							break;
						case AzureOutputType.GradientMoonElevation:
							EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("moonElevationGradient"), GUIContent.none);
							break;
					}
					
					// Description
					rect.y += EditorGUIUtility.singleLineHeight +1;
					EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 3.0f),
						"Property Type: " + m_target.outputsList[index].azureOutputType + "\nDescription: " +element.FindPropertyRelative("description").stringValue,
						MessageType.Info);
				},
				
				elementHeightCallback = (int index) => EditorGUIUtility.singleLineHeight * 5.5f,

				drawHeaderCallback = (Rect rect) =>
				{
					EditorGUI.LabelField(rect,new GUIContent("List", ""), EditorStyles.boldLabel);
				},

				drawElementBackgroundCallback = (rect, index, active, focused) =>
				{
					if (active)
						GUI.Box(new Rect(rect.x +2, rect.y -1, rect.width -4, rect.height +1), "","selectionRect");
				}
			};
		}

		public override void OnInspectorGUI()
		{
			// Start custom Inspector
			Undo.RecordObject(m_target, "Undo Azure Sky Profile");
			serializedObject.Update();
			
			// Setting header bar foldout strings
			m_scatteringFoldout = m_target.editorSettings.showScatteringTab ? "| Hide" : "| Show";
			m_outerSpaceFoldout = m_target.editorSettings.showOuterSpaceTab ? "| Hide" : "| Show";
			m_fogScatteringFoldout = m_target.editorSettings.showFogScatteringTab ? "| Hide" : "| Show";
			m_cloudsFoldout = m_target.editorSettings.showCloudsTab ? "| Hide" : "| Show";
			m_lightingFoldout = m_target.editorSettings.showLightingTab ? "| Hide" : "| Show";
			m_weatherFoldout = m_target.editorSettings.showWeatherTab ? "| Hide" : "| Show";
			m_outputsFoldout = m_target.editorSettings.showOutputsTab ? "| Hide" : "| Show";
			
			// Initializing the custom styles
			m_headerStyle = new GUIStyle("selectionRect")
			{
				fontStyle = FontStyle.Bold,
				contentOffset = new Vector2(0f, -2f)
			};
			
			// Profile logo
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical("selectionRect");
			m_controlRect = EditorGUILayout.GetControlRect();
			m_logoTexture = AssetDatabase.LoadAssetAtPath("Assets/Azure[Sky] Dynamic Skybox/Editor/InspectorGui/AzureSkyProfileBar.png",typeof(Texture2D)) as Texture2D;
			GUI.DrawTexture(new Rect(m_controlRect.x, m_controlRect.y, 256, 55), m_logoTexture);
			GUILayout.Space(38);
			EditorGUILayout.EndVertical();
			
			// Scattering tab
			m_controlRect = EditorGUILayout.GetControlRect();
			GUI.Label(m_controlRect, "", m_headerStyle);
			if (GUI.Button(m_controlRect, "SCATTERING", m_headerStyle)) m_target.editorSettings.showScatteringTab = !m_target.editorSettings.showScatteringTab;
			GUI.Label(new Rect(m_controlRect.width - 40, m_controlRect.y, m_controlRect.width, 15), m_scatteringFoldout);
			if (m_target.editorSettings.showScatteringTab)
			{
				GUILayout.Label("Stylized Sky:");
				
				// Molecular density
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Air Density", "The molecular density."), GUILayout.Width(m_labelWidth));
				switch (m_target.scattering.molecularDensity.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.scattering.molecularDensity.slider = EditorGUILayout.Slider(m_target.scattering.molecularDensity.slider,0.1f, 3.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_MolecularDensity.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0, 0.1f, 24, 3.1f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_MolecularDensity.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.1f, 2.0f, 3.1f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_MolecularDensity.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.1f, 2.0f, 3.1f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_MolecularDensity.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.scattering.molecularDensity = new AzureFloatProperty
					(
						2.545f,
						AnimationCurve.Linear (0.0f, 2.545f, 24.0f, 2.545f),
						AnimationCurve.Linear (-1.0f, 2.545f, 1.0f, 2.545f),
						AnimationCurve.Linear (-1.0f, 2.545f, 1.0f, 2.545f)
					);
				}
				EditorGUILayout.EndHorizontal();

				// Wavelength R
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Wavelength R", "The red wavelength from the visible spectrum."), GUILayout.Width(m_labelWidth));
				switch (m_target.scattering.wavelengthR.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.scattering.wavelengthR.slider = EditorGUILayout.Slider(m_target.scattering.wavelengthR.slider,380, 740);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_WavelengthR.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0, 380, 24, 360), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_WavelengthR.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 380, 2.0f, 360), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_WavelengthR.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 380, 2.0f, 360), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_WavelengthR.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.scattering.wavelengthR = new AzureFloatProperty
					(
						680.0f,
						AnimationCurve.Linear (0.0f, 680.0f, 24.0f, 680.0f),
						AnimationCurve.Linear (-1.0f, 680.0f, 1.0f, 680.0f),
						AnimationCurve.Linear (-1.0f, 680.0f, 1.0f, 680.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Wavelength G
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Wavelength G", "The green wavelength from the visible spectrum."), GUILayout.Width(m_labelWidth));
				switch (m_target.scattering.wavelengthG.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.scattering.wavelengthG.slider = EditorGUILayout.Slider(m_target.scattering.wavelengthG.slider,380, 740);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_WavelengthG.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0, 380, 24, 360), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_WavelengthG.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 380, 2.0f, 360), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_WavelengthG.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 380, 2.0f, 360), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_WavelengthG.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.scattering.wavelengthG = new AzureFloatProperty
					(
						550.0f,
						AnimationCurve.Linear (0.0f, 550.0f, 24.0f, 550.0f),
						AnimationCurve.Linear (-1.0f, 550.0f, 1.0f, 550.0f),
						AnimationCurve.Linear (-1.0f, 550.0f, 1.0f, 550.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Wavelength B
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Wavelength B", "The blue wavelength from the visible spectrum."), GUILayout.Width(m_labelWidth));
				switch (m_target.scattering.wavelengthB.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.scattering.wavelengthB.slider = EditorGUILayout.Slider(m_target.scattering.wavelengthB.slider,380, 740);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_WavelengthB.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0, 380, 24, 360), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_WavelengthB.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 380, 2.0f, 360), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_WavelengthB.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 380, 2.0f, 360), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_WavelengthB.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.scattering.wavelengthB = new AzureFloatProperty
					(
						450.0f,
						AnimationCurve.Linear (0.0f, 450.0f, 24.0f, 450.0f),
						AnimationCurve.Linear (-1.0f, 450.0f, 1.0f, 450.0f),
						AnimationCurve.Linear (-1.0f, 450.0f, 1.0f, 450.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.Space();
				
				// Rayleigh multiplier
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Rayleigh", "The rayleigh multiplier coefficient."), GUILayout.Width(m_labelWidth));
				switch (m_target.scattering.rayleighMultiplier.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.scattering.rayleighMultiplier.slider = EditorGUILayout.Slider(m_target.scattering.rayleighMultiplier.slider,0, 10);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_RayleighMultiplier.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0, 0, 24, 10), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_RayleighMultiplier.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0, 2.0f, 10), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_RayleighMultiplier.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0, 2.0f, 10), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_RayleighMultiplier.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.scattering.rayleighMultiplier = new AzureFloatProperty
					(
						1.5f,
						AnimationCurve.Linear (0.0f, 1.5f, 24.0f, 1.5f),
						AnimationCurve.Linear (-1.0f, 1.5f, 1.0f, 1.5f),
						AnimationCurve.Linear (-1.0f, 1.5f, 1.0f, 1.5f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Mie multiplier
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Mie", "The mie multiplier coefficient."), GUILayout.Width(m_labelWidth));
				switch (m_target.scattering.mieMultiplier.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.scattering.mieMultiplier.slider = EditorGUILayout.Slider(m_target.scattering.mieMultiplier.slider,0, 10);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_MieMultiplier.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0, 0, 24, 10), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_MieMultiplier.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0, 2.0f, 10), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_MieMultiplier.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0, 2.0f, 10), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_MieMultiplier.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.scattering.mieMultiplier = new AzureFloatProperty
					(
						1.0f,
						AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
						AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
						AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Scattering intensity
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Scattering", "The light scattering multiplier coefficient."), GUILayout.Width(m_labelWidth));
				switch (m_target.scattering.scatteringIntensity.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.scattering.scatteringIntensity.slider = EditorGUILayout.Slider(m_target.scattering.scatteringIntensity.slider,0, 1);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_ScatteringIntensity.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0, 0, 24, 1), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_ScatteringIntensity.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0, 2.0f, 1), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_ScatteringIntensity.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0, 2.0f, 1), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_ScatteringIntensity.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.scattering.scatteringIntensity = new AzureFloatProperty
					(
						0.25f,
						AnimationCurve.Linear (0.0f, 0.25f, 24.0f, 0.25f),
						AnimationCurve.Linear (-1.0f, 0.25f, 1.0f, 0.25f),
						AnimationCurve.Linear (-1.0f, 0.25f, 1.0f, 0.25f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Sky luminance
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Luminance", "Controls the luminance of the sky, it is very useful to set the intensity of the night when there is no moon in the sky."), GUILayout.Width(m_labelWidth));
				switch (m_target.scattering.skyLuminance.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.scattering.skyLuminance.slider = EditorGUILayout.Slider(m_target.scattering.skyLuminance.slider,0, 5);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_SkyLuminance.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0, 0, 24, 5), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_SkyLuminance.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0, 2.0f, 5), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_SkyLuminance.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0, 2.0f, 5), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_SkyLuminance.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.scattering.skyLuminance = new AzureFloatProperty
					(
						1.5f,
						AnimationCurve.Linear (0.0f, 1.5f, 24.0f, 1.5f),
						AnimationCurve.Linear (-1.0f, 1.5f, 1.0f, 1.5f),
						AnimationCurve.Linear (-1.0f, 1.5f, 1.0f, 1.5f)
					);
				}
				EditorGUILayout.EndHorizontal();

				// Transmittance color
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Sunset Color", "Custom sunset color. Effect only if 'Sunset Mode' is set to 'Custom Color'."), GUILayout.Width(m_labelWidth));
				switch (m_target.scattering.sunsetColor.propertyType)
				{
					case AzureColorProperty.PropertyType.Color:
						m_target.scattering.sunsetColor.color = EditorGUILayout.ColorField(m_target.scattering.sunsetColor.color);
						break;
					case AzureColorProperty.PropertyType.GradientTimeline:
						EditorGUILayout.PropertyField(m_SunsetColor.FindPropertyRelative("timelineGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientSunElevation:
						EditorGUILayout.PropertyField(m_SunsetColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientMoonElevation:
						EditorGUILayout.PropertyField(m_SunsetColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_SunsetColor.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.scattering.sunsetColor = new AzureColorProperty
					(
						Color.white,
						new Gradient(),
						new Gradient(),
						new Gradient()
					);
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.Space();
				m_controlRect = EditorGUILayout.GetControlRect();
				GUI.Label(m_controlRect, "", "IN Title");
				GUILayout.Label("Precomputed Sky:");
				
				// Precomputed sun intensity
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Sun Intensity", "The intensity of sun light scattering of the precomputed sky model."), GUILayout.Width(m_labelWidth));
				switch (m_target.scattering.precomputedSunIntensity.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.scattering.precomputedSunIntensity.slider = EditorGUILayout.Slider(m_target.scattering.precomputedSunIntensity.slider,0.0f, 100.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_PrecomputedSunIntensity.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0, 0.0f, 24, 100.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_PrecomputedSunIntensity.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 100.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_PrecomputedSunIntensity.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 100.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_PrecomputedSunIntensity.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.scattering.precomputedSunIntensity = new AzureFloatProperty
					(
						25.0f,
						AnimationCurve.Linear (0.0f, 25.0f, 24.0f, 25.0f),
						AnimationCurve.Linear (-1.0f, 25.0f, 1.0f, 25.0f),
						AnimationCurve.Linear (-1.0f, 25.0f, 1.0f, 25.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Precomputed moon intensity
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Moon Intensity", "The intensity of moon light scattering of the precomputed sky model."), GUILayout.Width(m_labelWidth));
				switch (m_target.scattering.precomputedMoonIntensity.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.scattering.precomputedMoonIntensity.slider = EditorGUILayout.Slider(m_target.scattering.precomputedMoonIntensity.slider,0.0f, 10.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_PrecomputedMoonIntensity.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0, 0.0f, 24, 10.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_PrecomputedMoonIntensity.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 10.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_PrecomputedMoonIntensity.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 10.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_PrecomputedMoonIntensity.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.scattering.precomputedMoonIntensity = new AzureFloatProperty
					(
						0.1f,
						AnimationCurve.Linear (0.0f, 0.1f, 24.0f, 0.1f),
						AnimationCurve.Linear (-1.0f, 0.1f, 1.0f, 0.1f),
						AnimationCurve.Linear (-1.0f, 0.1f, 1.0f, 0.1f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				
				EditorGUILayout.Space();
				m_controlRect = EditorGUILayout.GetControlRect();
				GUI.Label(m_controlRect, "", "IN Title");
				GUILayout.Label("Stylized Sky / Precomputed Sky:");
				
				// Rayleigh color
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Rayleigh Color", "Rayleigh color multiplier."), GUILayout.Width(m_labelWidth));
				switch (m_target.scattering.rayleighColor.propertyType)
				{
					case AzureColorProperty.PropertyType.Color:
						m_target.scattering.rayleighColor.color = EditorGUILayout.ColorField(m_target.scattering.rayleighColor.color);
						break;
					case AzureColorProperty.PropertyType.GradientTimeline:
						EditorGUILayout.PropertyField(m_RayleighColor.FindPropertyRelative("timelineGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientSunElevation:
						EditorGUILayout.PropertyField(m_RayleighColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientMoonElevation:
						EditorGUILayout.PropertyField(m_RayleighColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_RayleighColor.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.scattering.rayleighColor = new AzureColorProperty
					(
						Color.white,
						new Gradient(),
						new Gradient(),
						new Gradient()
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Mie color
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Mie Color", "Mie color multiplier."), GUILayout.Width(m_labelWidth));
				switch (m_target.scattering.mieColor.propertyType)
				{
					case AzureColorProperty.PropertyType.Color:
						m_target.scattering.mieColor.color = EditorGUILayout.ColorField(m_target.scattering.mieColor.color);
						break;
					case AzureColorProperty.PropertyType.GradientTimeline:
						EditorGUILayout.PropertyField(m_MieColor.FindPropertyRelative("timelineGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientSunElevation:
						EditorGUILayout.PropertyField(m_MieColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientMoonElevation:
						EditorGUILayout.PropertyField(m_MieColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_MieColor.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.scattering.mieColor = new AzureColorProperty
					(
						Color.white,
						new Gradient(),
						new Gradient(),
						new Gradient()
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Exposure
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Exposure", "The color exposure of the tonemapping effect."), GUILayout.Width(m_labelWidth));
				switch (m_target.scattering.exposure.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.scattering.exposure.slider = EditorGUILayout.Slider(m_target.scattering.exposure.slider,0, 10);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_Exposure.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0, 0, 24, 10), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_Exposure.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0, 2.0f, 10), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_Exposure.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0, 2.0f, 10), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_Exposure.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.scattering.exposure = new AzureFloatProperty
					(
						2.0f,
						AnimationCurve.Linear (0.0f, 2.0f, 24.0f, 2.0f),
						AnimationCurve.Linear (-1.0f, 2.0f, 1.0f, 2.0f),
						AnimationCurve.Linear (-1.0f, 2.0f, 1.0f, 2.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
			}
			
			// Outer space tab
			m_controlRect = EditorGUILayout.GetControlRect();
			GUI.Label(m_controlRect, "", m_headerStyle);
			if (GUI.Button(m_controlRect, "OUTER SPACE", m_headerStyle)) m_target.editorSettings.showOuterSpaceTab = !m_target.editorSettings.showOuterSpaceTab;
			GUI.Label(new Rect(m_controlRect.width - 40, m_controlRect.y, m_controlRect.width, 15), m_outerSpaceFoldout);
			if (m_target.editorSettings.showOuterSpaceTab)
			{
				GUILayout.Label("Sun Texture:");
				
				// Size
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Size", "The size of the sun texture."), GUILayout.Width(m_labelWidth));
				switch (m_target.outerSpace.sunTextureSize.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.outerSpace.sunTextureSize.slider = EditorGUILayout.Slider(m_target.outerSpace.sunTextureSize.slider,0.5f, 10);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_SunTextureSize.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0, 0.5f, 24, 9.5f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_SunTextureSize.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.5f, 2.0f, 9.5f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_SunTextureSize.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.5f, 2.0f, 9.5f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_SunTextureSize.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.outerSpace.sunTextureSize = new AzureFloatProperty
					(
						2.5f,
						AnimationCurve.Linear (0.0f, 2.5f, 24.0f, 2.5f),
						AnimationCurve.Linear (-1.0f, 2.5f, 1.0f, 2.5f),
						AnimationCurve.Linear (-1.0f, 2.5f, 1.0f, 2.5f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Intensity
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Intensity", "The intensity of the sun texture."), GUILayout.Width(m_labelWidth));
				switch (m_target.outerSpace.sunTextureIntensity.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.outerSpace.sunTextureIntensity.slider = EditorGUILayout.Slider(m_target.outerSpace.sunTextureIntensity.slider,0.0f, 2.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_SunTextureIntensity.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24, 2.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_SunTextureIntensity.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 2.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_SunTextureIntensity.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 2.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_SunTextureIntensity.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.outerSpace.sunTextureIntensity = new AzureFloatProperty
					(
						1.0f,
						AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
						AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
						AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Color
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Color", "The color of the sun texture."), GUILayout.Width(m_labelWidth));
				switch (m_target.outerSpace.sunTextureColor.propertyType)
				{
					case AzureColorProperty.PropertyType.Color:
						m_target.outerSpace.sunTextureColor.color = EditorGUILayout.ColorField(m_target.outerSpace.sunTextureColor.color);
						break;
					case AzureColorProperty.PropertyType.GradientTimeline:
						EditorGUILayout.PropertyField(m_SunTextureColor.FindPropertyRelative("timelineGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientSunElevation:
						EditorGUILayout.PropertyField(m_SunTextureColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientMoonElevation:
						EditorGUILayout.PropertyField(m_SunTextureColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_SunTextureColor.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.outerSpace.sunTextureColor = new AzureColorProperty
					(
						Color.white,
						new Gradient(),
						new Gradient(),
						new Gradient()
					);
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.Space();
				m_controlRect = EditorGUILayout.GetControlRect();
				GUI.Label(m_controlRect, "", "IN Title");
				GUILayout.Label("Moon Texture:");
				
				// Size
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Size", "The size of the moon texture."), GUILayout.Width(m_labelWidth));
				switch (m_target.outerSpace.moonTextureSize.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.outerSpace.moonTextureSize.slider = EditorGUILayout.Slider(m_target.outerSpace.moonTextureSize.slider,1.0f, 20.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_MoonTextureSize.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0, 1.0f, 24, 20.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_MoonTextureSize.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 1.0f, 2.0f, 20.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_MoonTextureSize.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 1.0f, 2.0f, 20.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_MoonTextureSize.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.outerSpace.moonTextureSize = new AzureFloatProperty
					(
						10.0f,
						AnimationCurve.Linear (0.0f, 10.0f, 24.0f, 10.0f),
						AnimationCurve.Linear (-1.0f, 10.0f, 1.0f, 10.0f),
						AnimationCurve.Linear (-1.0f, 10.0f, 1.0f, 10.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Intensity
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Intensity", "The intensity of the moon texture."), GUILayout.Width(m_labelWidth));
				switch (m_target.outerSpace.moonTextureIntensity.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.outerSpace.moonTextureIntensity.slider = EditorGUILayout.Slider(m_target.outerSpace.moonTextureIntensity.slider,0.0f, 2.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_MoonTextureIntensity.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 2.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_MoonTextureIntensity.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 2.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_MoonTextureIntensity.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 2.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_MoonTextureIntensity.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.outerSpace.moonTextureIntensity = new AzureFloatProperty
					(
						1.0f,
						AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
						AnimationCurve.Linear (-1.0f, 31.0f, 1.0f, 1.0f),
						AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Color
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Color", "The color of the moon texture."), GUILayout.Width(m_labelWidth));
				switch (m_target.outerSpace.moonTextureColor.propertyType)
				{
					case AzureColorProperty.PropertyType.Color:
						m_target.outerSpace.moonTextureColor.color = EditorGUILayout.ColorField(m_target.outerSpace.moonTextureColor.color);
						break;
					case AzureColorProperty.PropertyType.GradientTimeline:
						EditorGUILayout.PropertyField(m_MoonTextureColor.FindPropertyRelative("timelineGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientSunElevation:
						EditorGUILayout.PropertyField(m_MoonTextureColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientMoonElevation:
						EditorGUILayout.PropertyField(m_MoonTextureColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_MoonTextureColor.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.outerSpace.moonTextureColor = new AzureColorProperty
					(
						Color.white,
						new Gradient(),
						new Gradient(),
						new Gradient()
					);
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.Space();
				m_controlRect = EditorGUILayout.GetControlRect();
				GUI.Label(m_controlRect, "", "IN Title");
				GUILayout.Label("Starfield Texture:");
				
				// Regular stars scintillation
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Scintillation", "The scintillation speed of the regular stars."), GUILayout.Width(m_labelWidth));
				switch (m_target.outerSpace.regularStarsScintillation.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.outerSpace.regularStarsScintillation.slider = EditorGUILayout.Slider(m_target.outerSpace.regularStarsScintillation.slider,0.0f, 1.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_RegularStarsScintillation.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_RegularStarsScintillation.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_RegularStarsScintillation.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_RegularStarsScintillation.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.outerSpace.regularStarsScintillation = new AzureFloatProperty
					(
						0.5f,
						AnimationCurve.Linear (0.0f, 0.5f, 24.0f, 0.5f),
						AnimationCurve.Linear (-1.0f, 0.5f, 1.0f, 0.5f),
						AnimationCurve.Linear (-1.0f, 0.5f, 1.0f, 0.5f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Regular stars intensity
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Regular Stars", "The intensity of the regular stars."), GUILayout.Width(m_labelWidth));
				switch (m_target.outerSpace.regularStarsIntensity.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.outerSpace.regularStarsIntensity.slider = EditorGUILayout.Slider(m_target.outerSpace.regularStarsIntensity.slider,0.0f, 1.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_RegularStarsIntensity.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_RegularStarsIntensity.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_RegularStarsIntensity.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_RegularStarsIntensity.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.outerSpace.regularStarsIntensity = new AzureFloatProperty
					(
						0.5f,
						AnimationCurve.Linear (0.0f, 0.5f, 24.0f, 0.5f),
						AnimationCurve.Linear (-1.0f, 0.5f, 1.0f, 0.5f),
						AnimationCurve.Linear (-1.0f, 0.5f, 1.0f, 0.5f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Milky Way intensity
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Milky Way", "The intensity of the Milky Way."), GUILayout.Width(m_labelWidth));
				switch (m_target.outerSpace.milkyWayIntensity.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.outerSpace.milkyWayIntensity.slider = EditorGUILayout.Slider(m_target.outerSpace.milkyWayIntensity.slider,0.0f, 1.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_MilkyWayIntensity.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_MilkyWayIntensity.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_MilkyWayIntensity.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_MilkyWayIntensity.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.outerSpace.milkyWayIntensity = new AzureFloatProperty
					(
						0.0f,
						AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
			}
			
			// Fog scattering tab
			m_controlRect = EditorGUILayout.GetControlRect();
			GUI.Label(m_controlRect, "", m_headerStyle);
			if (GUI.Button(m_controlRect, "FOG SCATTERING", m_headerStyle)) m_target.editorSettings.showFogScatteringTab = !m_target.editorSettings.showFogScatteringTab;
			GUI.Label(new Rect(m_controlRect.width - 40, m_controlRect.y, m_controlRect.width, 15), m_fogScatteringFoldout);
			if (m_target.editorSettings.showFogScatteringTab)
			{
				// Fog scattering scale
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Scale", "The fog scattering scale. Use this to make the fog of stylized sky model more bright or more blue."), GUILayout.Width(m_labelWidth));
				switch (m_target.fogScattering.fogScatteringScale.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.fogScattering.fogScatteringScale.slider = EditorGUILayout.Slider(m_target.fogScattering.fogScatteringScale.slider,0.0f, 1.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_FogScatteringScale.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_FogScatteringScale.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_FogScatteringScale.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_FogScatteringScale.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.fogScattering.fogScatteringScale = new AzureFloatProperty
					(
						1.0f,
						AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
						AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
						AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.Space();
				GUILayout.Label("Global Fog:");
				
				// Global fog smooth
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Smooth", "Smooths the fog transition between where the global fog starts and where it is completely covered by the global fog."), GUILayout.Width(m_labelWidth));
				switch (m_target.fogScattering.globalFogSmooth.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.fogScattering.globalFogSmooth.slider = EditorGUILayout.Slider(m_target.fogScattering.globalFogSmooth.slider,-1.0f, 2.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_GlobalFogSmooth.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, -1.0f, 24.0f, 3.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_GlobalFogSmooth.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, -1.0f, 2.0f, 3.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_GlobalFogSmooth.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, -1.0f, 2.0f, 3.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_GlobalFogSmooth.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.fogScattering.globalFogSmooth = new AzureFloatProperty
					(
						0.25f,
						AnimationCurve.Linear (0.0f, 0.25f, 24.0f, 0.25f),
						AnimationCurve.Linear (-1.0f, 0.25f, 1.0f, 0.25f),
						AnimationCurve.Linear (-1.0f, 0.25f, 1.0f, 0.25f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Global fog density
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Density", "The density of the global fog. Use this to make the entire fog disappear at the same time regardless of distance."), GUILayout.Width(m_labelWidth));
				switch (m_target.fogScattering.globalFogDensity.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.fogScattering.globalFogDensity.slider = EditorGUILayout.Slider(m_target.fogScattering.globalFogDensity.slider,0.0f, 1.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_GlobalFogDensity.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_GlobalFogDensity.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_GlobalFogDensity.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_GlobalFogDensity.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.fogScattering.globalFogDensity = new AzureFloatProperty
					(
						1.0f,
						AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
						AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
						AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Global fog distance
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Distance", "The distance of the global fog in real world scale(meters). This is the distance at which the scene will be completely covered by the global fog."), GUILayout.Width(m_labelWidth));
				switch (m_target.fogScattering.globalFogDistance.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.fogScattering.globalFogDistance.slider = EditorGUILayout.Slider(m_target.fogScattering.globalFogDistance.slider,0.0f, 25000.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_GlobalFogDistance.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 25000.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_GlobalFogDistance.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 25000.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_GlobalFogDistance.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 25000.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_GlobalFogDistance.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.fogScattering.globalFogDistance = new AzureFloatProperty
					(
						1000.0f,
						AnimationCurve.Linear (0.0f, 1000.0f, 24.0f, 1000.0f),
						AnimationCurve.Linear (-1.0f, 1000.0f, 1.0f, 1000.0f),
						AnimationCurve.Linear (-1.0f, 1000.0f, 1.0f, 1000.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.Space();
				m_controlRect = EditorGUILayout.GetControlRect();
				GUI.Label(m_controlRect, "", "IN Title");
				GUILayout.Label("Height Fog:");
				
				// Height fog smooth
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Smooth", "Smooths the fog transition between where the height fog starts and where it is completely covered by the height fog."), GUILayout.Width(m_labelWidth));
				switch (m_target.fogScattering.heightFogSmooth.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.fogScattering.heightFogSmooth.slider = EditorGUILayout.Slider(m_target.fogScattering.heightFogSmooth.slider,-1.0f, 2.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_HeightFogSmooth.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, -1.0f, 24.0f, 3.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_HeightFogSmooth.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, -1.0f, 2.0f, 3.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_HeightFogSmooth.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, -1.0f, 2.0f, 3.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_HeightFogSmooth.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.fogScattering.heightFogSmooth = new AzureFloatProperty
					(
						1.0f,
						AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
						AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
						AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Height fog density
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Density", "The density of the height fog. Use this to make the entire fog disappear at the same time regardless of distance."), GUILayout.Width(m_labelWidth));
				switch (m_target.fogScattering.heightFogDensity.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.fogScattering.heightFogDensity.slider = EditorGUILayout.Slider(m_target.fogScattering.heightFogDensity.slider,0.0f, 1.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_HeightFogDensity.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_HeightFogDensity.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_HeightFogDensity.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_HeightFogDensity.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.fogScattering.heightFogDensity = new AzureFloatProperty
					(
						0.0f,
						AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Height fog distance
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Distance", "The distance of the height fog in real world scale(meters). This is the distance at which the scene will be completely covered by the height fog."), GUILayout.Width(m_labelWidth));
				switch (m_target.fogScattering.heightFogDistance.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.fogScattering.heightFogDistance.slider = EditorGUILayout.Slider(m_target.fogScattering.heightFogDistance.slider,0.0f, 1500.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_HeightFogDistance.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 1500.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_HeightFogDistance.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 1500.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_HeightFogDistance.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 1500.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_HeightFogDistance.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.fogScattering.heightFogDistance = new AzureFloatProperty
					(
						100.0f,
						AnimationCurve.Linear (0.0f, 100.0f, 24.0f, 100.0f),
						AnimationCurve.Linear (-1.0f, 100.0f, 1.0f, 100.0f),
						AnimationCurve.Linear (-1.0f, 100.0f, 1.0f, 100.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Height fog start
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Start", "The height where the fog will start"), GUILayout.Width(m_labelWidth));
				switch (m_target.fogScattering.heightFogStart.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.fogScattering.heightFogStart.slider = EditorGUILayout.Slider(m_target.fogScattering.heightFogStart.slider,-500.0f, 500.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_HeightFogStart.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, -500.0f, 24.0f, 1000.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_HeightFogStart.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, -500.0f, 2.0f, 1000.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_HeightFogStart.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, -500.0f, 2.0f, 1000.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_HeightFogStart.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.fogScattering.heightFogStart = new AzureFloatProperty
					(
						0.0f,
						AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Height fog end
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("End", "The height where the fog will end"), GUILayout.Width(m_labelWidth));
				switch (m_target.fogScattering.heightFogEnd.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.fogScattering.heightFogEnd.slider = EditorGUILayout.Slider(m_target.fogScattering.heightFogEnd.slider,0.0f, 2000.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_HeightFogEnd.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 2000.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_HeightFogEnd.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 2000.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_HeightFogEnd.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 2000.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_HeightFogEnd.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.fogScattering.heightFogEnd = new AzureFloatProperty
					(
						100.0f,
						AnimationCurve.Linear (0.0f, 100.0f, 24.0f, 100.0f),
						AnimationCurve.Linear (-1.0f, 100.0f, 1.0f, 100.0f),
						AnimationCurve.Linear (-1.0f, 100.0f, 1.0f, 100.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
			}
			
			// Clouds tab
			m_controlRect = EditorGUILayout.GetControlRect();
			GUI.Label(m_controlRect, "", m_headerStyle);
			if (GUI.Button(m_controlRect, "CLOUDS", m_headerStyle)) m_target.editorSettings.showCloudsTab = !m_target.editorSettings.showCloudsTab;
			GUI.Label(new Rect(m_controlRect.width - 40, m_controlRect.y, m_controlRect.width, 15), m_cloudsFoldout);
			if (m_target.editorSettings.showCloudsTab)
			{
				GUILayout.Label("Static Clouds:");
				
				// Texture
				GUI.color = m_customGreenColor;
				if (!m_target.clouds.staticCloudTexture) { GUI.color = m_customRedColor; }
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Texture", "The texture used to render the static cloud layer."), GUILayout.Width(m_labelWidth));
				m_target.clouds.staticCloudTexture = (Texture2D)EditorGUILayout.ObjectField(m_target.clouds.staticCloudTexture, typeof(Texture2D), true);
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(2);
				GUI.color = Color.white;
				
				// Layer1 rotation speed
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Layer1 Speed", "The rotation speed of the first layer from static cloud texture."), GUILayout.Width(m_labelWidth));
				switch (m_target.clouds.staticCloudLayer1Speed.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.clouds.staticCloudLayer1Speed.slider = EditorGUILayout.Slider(m_target.clouds.staticCloudLayer1Speed.slider,-0.01f, 0.01f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_StaticCloudLayer1Speed.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, -0.01f, 24.0f, 0.02f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_StaticCloudLayer1Speed.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, -0.01f, 2.0f, 0.02f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_StaticCloudLayer1Speed.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, -0.01f, 2.0f, 0.02f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_StaticCloudLayer1Speed.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.clouds.staticCloudLayer1Speed = new AzureFloatProperty
					(
						0.0f,
						AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Layer2 rotation speed
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Layer2 Speed", "The rotation speed of the second layer from static cloud texture."), GUILayout.Width(m_labelWidth));
				switch (m_target.clouds.staticCloudLayer2Speed.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.clouds.staticCloudLayer2Speed.slider = EditorGUILayout.Slider(m_target.clouds.staticCloudLayer2Speed.slider,-0.01f, 0.01f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_StaticCloudLayer2Speed.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, -0.01f, 24.0f, 0.02f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_StaticCloudLayer2Speed.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, -0.01f, 2.0f, 0.02f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_StaticCloudLayer2Speed.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, -0.01f, 2.0f, 0.02f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_StaticCloudLayer2Speed.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.clouds.staticCloudLayer2Speed = new AzureFloatProperty
					(
						0.0f,
						AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Static clouds color
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Color", "The color of the static clouds."), GUILayout.Width(m_labelWidth));
				switch (m_target.clouds.staticCloudColor.propertyType)
				{
					case AzureColorProperty.PropertyType.Color:
						m_target.clouds.staticCloudColor.color = EditorGUILayout.ColorField(m_target.clouds.staticCloudColor.color);
						break;
					case AzureColorProperty.PropertyType.GradientTimeline:
						EditorGUILayout.PropertyField(m_StaticCloudColor.FindPropertyRelative("timelineGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientSunElevation:
						EditorGUILayout.PropertyField(m_StaticCloudColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientMoonElevation:
						EditorGUILayout.PropertyField(m_StaticCloudColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_StaticCloudColor.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.clouds.staticCloudColor = new AzureColorProperty
					(
						Color.white,
						new Gradient(),
						new Gradient(),
						new Gradient()
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Static cloud scattering
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Scattering", "The amount of light scattered by the clouds."), GUILayout.Width(m_labelWidth));
				switch (m_target.clouds.staticCloudScattering.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.clouds.staticCloudScattering.slider = EditorGUILayout.Slider(m_target.clouds.staticCloudScattering.slider,0.0f, 2.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_StaticCloudScattering.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 2.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_StaticCloudScattering.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 2.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_StaticCloudScattering.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 2.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_StaticCloudScattering.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.clouds.staticCloudScattering = new AzureFloatProperty
					(
						1.0f,
						AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
						AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
						AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Static cloud extinction
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Extinction", "The fading of distant clouds on the horizon."), GUILayout.Width(m_labelWidth));
				switch (m_target.clouds.staticCloudExtinction.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.clouds.staticCloudExtinction.slider = EditorGUILayout.Slider(m_target.clouds.staticCloudExtinction.slider,1.0f, 5.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_StaticCloudExtinction.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 1.0f, 24.0f, 4.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_StaticCloudExtinction.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 1.0f, 2.0f, 4.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_StaticCloudExtinction.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 1.0f, 2.0f, 4.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_StaticCloudExtinction.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.clouds.staticCloudExtinction = new AzureFloatProperty
					(
						1.5f,
						AnimationCurve.Linear (0.0f, 1.5f, 24.0f, 1.5f),
						AnimationCurve.Linear (-1.0f, 1.5f, 1.0f, 1.5f),
						AnimationCurve.Linear (-1.0f, 1.5f, 1.0f, 1.5f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Static cloud saturation
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Saturation", "Color saturation of static clouds."), GUILayout.Width(m_labelWidth));
				switch (m_target.clouds.staticCloudSaturation.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.clouds.staticCloudSaturation.slider = EditorGUILayout.Slider(m_target.clouds.staticCloudSaturation.slider,1.0f, 8.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_StaticCloudSaturation.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 1.0f, 24.0f, 7.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_StaticCloudSaturation.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 1.0f, 2.0f, 7.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_StaticCloudSaturation.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 1.0f, 2.0f, 7.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_StaticCloudSaturation.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.clouds.staticCloudSaturation = new AzureFloatProperty
					(
						2.5f,
						AnimationCurve.Linear (0.0f, 2.5f, 24.0f, 2.5f),
						AnimationCurve.Linear (-1.0f, 2.5f, 1.0f, 2.5f),
						AnimationCurve.Linear (-1.0f, 2.5f, 1.0f, 2.5f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Static cloud opacity
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Opacity", "The opacity of static clouds."), GUILayout.Width(m_labelWidth));
				switch (m_target.clouds.staticCloudOpacity.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.clouds.staticCloudOpacity.slider = EditorGUILayout.Slider(m_target.clouds.staticCloudOpacity.slider,0.0f, 2.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_StaticCloudOpacity.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 2.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_StaticCloudOpacity.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 2.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_StaticCloudOpacity.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 2.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_StaticCloudOpacity.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.clouds.staticCloudOpacity = new AzureFloatProperty
					(
						1.25f,
						AnimationCurve.Linear (0.0f, 1.25f, 24.0f, 1.25f),
						AnimationCurve.Linear (-1.0f, 1.25f, 1.0f, 1.25f),
						AnimationCurve.Linear (-1.0f, 1.25f, 1.0f, 1.25f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.Space();
				m_controlRect = EditorGUILayout.GetControlRect();
				GUI.Label(m_controlRect, "", "IN Title");
				GUILayout.Label("Dynamic Clouds:");
				
				// Dynamic cloud altitude
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Altitude", "The fake altitude of dynamic clouds."), GUILayout.Width(m_labelWidth));
				switch (m_target.clouds.dynamicCloudAltitude.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.clouds.dynamicCloudAltitude.slider = EditorGUILayout.Slider(m_target.clouds.dynamicCloudAltitude.slider,0.0f, 20.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_DynamicCloudAltitude.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 20.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_DynamicCloudAltitude.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 20.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_DynamicCloudAltitude.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 20.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_DynamicCloudAltitude.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.clouds.dynamicCloudAltitude = new AzureFloatProperty
					(
						7.5f,
						AnimationCurve.Linear (0.0f, 7.5f, 24.0f, 7.5f),
						AnimationCurve.Linear (-1.0f, 7.5f, 1.0f, 7.5f),
						AnimationCurve.Linear (-1.0f, 7.5f, 1.0f, 7.5f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Dynamic cloud direction
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Direction", "The movement direction of dynamic clouds."), GUILayout.Width(m_labelWidth));
				switch (m_target.clouds.dynamicCloudDirection.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.clouds.dynamicCloudDirection.slider = EditorGUILayout.Slider(m_target.clouds.dynamicCloudDirection.slider,-3.0f, 3.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_DynamicCloudDirection.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, -3.0f, 24.0f, 6.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_DynamicCloudDirection.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, -3.0f, 2.0f, 6.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_DynamicCloudDirection.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, -3.0f, 2.0f, 6.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_DynamicCloudDirection.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.clouds.dynamicCloudDirection = new AzureFloatProperty
					(
						1.0f,
						AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
						AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
						AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Dynamic cloud speed
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Speed", "The movement speed of dynamic clouds."), GUILayout.Width(m_labelWidth));
				switch (m_target.clouds.dynamicCloudSpeed.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.clouds.dynamicCloudSpeed.slider = EditorGUILayout.Slider(m_target.clouds.dynamicCloudSpeed.slider,0.0f, 1.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_DynamicCloudSpeed.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_DynamicCloudSpeed.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_DynamicCloudSpeed.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_DynamicCloudSpeed.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.clouds.dynamicCloudSpeed = new AzureFloatProperty
					(
						0.1f,
						AnimationCurve.Linear (0.0f, 0.1f, 24.0f, 0.1f),
						AnimationCurve.Linear (-1.0f, 0.1f, 1.0f, 0.1f),
						AnimationCurve.Linear (-1.0f, 0.1f, 1.0f, 0.1f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Dynamic cloud density
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Density", "The density of dynamic clouds used to simulate the coverage."), GUILayout.Width(m_labelWidth));
				switch (m_target.clouds.dynamicCloudDensity.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.clouds.dynamicCloudDensity.slider = EditorGUILayout.Slider(m_target.clouds.dynamicCloudDensity.slider,0.0f, 1.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_DynamicCloudDensity.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_DynamicCloudDensity.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_DynamicCloudDensity.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_DynamicCloudDensity.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.clouds.dynamicCloudDensity = new AzureFloatProperty
					(
						0.75f,
						AnimationCurve.Linear (0.0f, 0.75f, 24.0f, 0.75f),
						AnimationCurve.Linear (-1.0f, 0.75f, 1.0f, 0.75f),
						AnimationCurve.Linear (-1.0f, 0.75f, 1.0f, 0.75f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Dynamic clouds color 1
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Color1", ""), GUILayout.Width(m_labelWidth));
				switch (m_target.clouds.dynamicCloudColor1.propertyType)
				{
					case AzureColorProperty.PropertyType.Color:
						m_target.clouds.dynamicCloudColor1.color = EditorGUILayout.ColorField(m_target.clouds.dynamicCloudColor1.color);
						break;
					case AzureColorProperty.PropertyType.GradientTimeline:
						EditorGUILayout.PropertyField(m_DynamicCloudColor1.FindPropertyRelative("timelineGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientSunElevation:
						EditorGUILayout.PropertyField(m_DynamicCloudColor1.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientMoonElevation:
						EditorGUILayout.PropertyField(m_DynamicCloudColor1.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_DynamicCloudColor1.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.clouds.dynamicCloudColor1 = new AzureColorProperty
					(
						Color.white,
						new Gradient(),
						new Gradient(),
						new Gradient()
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Dynamic clouds color 2
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Color2", ""), GUILayout.Width(m_labelWidth));
				switch (m_target.clouds.dynamicCloudColor2.propertyType)
				{
					case AzureColorProperty.PropertyType.Color:
						m_target.clouds.dynamicCloudColor2.color = EditorGUILayout.ColorField(m_target.clouds.dynamicCloudColor2.color);
						break;
					case AzureColorProperty.PropertyType.GradientTimeline:
						EditorGUILayout.PropertyField(m_DynamicCloudColor2.FindPropertyRelative("timelineGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientSunElevation:
						EditorGUILayout.PropertyField(m_DynamicCloudColor2.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientMoonElevation:
						EditorGUILayout.PropertyField(m_DynamicCloudColor2.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_DynamicCloudColor2.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.clouds.dynamicCloudColor2 = new AzureColorProperty
					(
						Color.white,
						new Gradient(),
						new Gradient(),
						new Gradient()
					);
				}
				EditorGUILayout.EndHorizontal();
			}
			
			// Lighting tab
			m_controlRect = EditorGUILayout.GetControlRect();
			GUI.Label(m_controlRect, "", m_headerStyle);
			if (GUI.Button(m_controlRect, "LIGHTING", m_headerStyle)) m_target.editorSettings.showLightingTab = !m_target.editorSettings.showLightingTab;
			GUI.Label(new Rect(m_controlRect.width - 40, m_controlRect.y, m_controlRect.width, 15), m_lightingFoldout);
			if (m_target.editorSettings.showLightingTab)
			{
				GUILayout.Label("Directional Light:");
				
				// Intensity
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Intensity", "The intensity of the directional light."), GUILayout.Width(m_labelWidth));
				switch (m_target.lighting.directionalLightIntensity.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.lighting.directionalLightIntensity.slider = EditorGUILayout.Slider(m_target.lighting.directionalLightIntensity.slider,0.0f, 8.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_DirectionalLightIntensity.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 8.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_DirectionalLightIntensity.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 8.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_DirectionalLightIntensity.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 8.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_DirectionalLightIntensity.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.lighting.directionalLightIntensity = new AzureFloatProperty
					(
						1.0f,
						AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
						AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
						AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Color
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Color", "The color of the directional light."), GUILayout.Width(m_labelWidth));
				switch (m_target.lighting.directionalLightColor.propertyType)
				{
					case AzureColorProperty.PropertyType.Color:
						m_target.lighting.directionalLightColor.color = EditorGUILayout.ColorField(m_target.lighting.directionalLightColor.color);
						break;
					case AzureColorProperty.PropertyType.GradientTimeline:
						EditorGUILayout.PropertyField(m_DirectionalLightColor.FindPropertyRelative("timelineGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientSunElevation:
						EditorGUILayout.PropertyField(m_DirectionalLightColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientMoonElevation:
						EditorGUILayout.PropertyField(m_DirectionalLightColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_DirectionalLightColor.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.lighting.directionalLightColor = new AzureColorProperty
					(
						Color.white,
						new Gradient(),
						new Gradient(),
						new Gradient()
					);
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.Space();
				m_controlRect = EditorGUILayout.GetControlRect();
				GUI.Label(m_controlRect, "", "IN Title");
				GUILayout.Label("Environment Lighting:");
				
				// Intensity
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Intensity", "The intensity multiplier of the environment lighting."), GUILayout.Width(m_labelWidth));
				switch (m_target.lighting.ambientIntensity.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.lighting.ambientIntensity.slider = EditorGUILayout.Slider(m_target.lighting.ambientIntensity.slider,0.0f, 8.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_AmbientIntensity.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 8.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_AmbientIntensity.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 8.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_AmbientIntensity.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 8.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_AmbientIntensity.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.lighting.ambientIntensity = new AzureFloatProperty
					(
						1.0f,
						AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
						AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
						AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Ambient color
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Ambient Color", "The ambient color of the environment lighting."), GUILayout.Width(m_labelWidth));
				switch (m_target.lighting.ambientColor.propertyType)
				{
					case AzureColorProperty.PropertyType.Color:
						m_target.lighting.ambientColor.color = EditorGUILayout.ColorField(m_target.lighting.ambientColor.color);
						break;
					case AzureColorProperty.PropertyType.GradientTimeline:
						EditorGUILayout.PropertyField(m_AmbientColor.FindPropertyRelative("timelineGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientSunElevation:
						EditorGUILayout.PropertyField(m_AmbientColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientMoonElevation:
						EditorGUILayout.PropertyField(m_AmbientColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_AmbientColor.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.lighting.ambientColor = new AzureColorProperty
					(
						Color.white,
						new Gradient(),
						new Gradient(),
						new Gradient()
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Ambient sky color
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Sky Color", "The sky color of the environment lighting."), GUILayout.Width(m_labelWidth));
				switch (m_target.lighting.ambientSkyColor.propertyType)
				{
					case AzureColorProperty.PropertyType.Color:
						m_target.lighting.ambientSkyColor.color = EditorGUILayout.ColorField(m_target.lighting.ambientSkyColor.color);
						break;
					case AzureColorProperty.PropertyType.GradientTimeline:
						EditorGUILayout.PropertyField(m_AmbientSkyColor.FindPropertyRelative("timelineGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientSunElevation:
						EditorGUILayout.PropertyField(m_AmbientSkyColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientMoonElevation:
						EditorGUILayout.PropertyField(m_AmbientSkyColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_AmbientSkyColor.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.lighting.ambientSkyColor = new AzureColorProperty
					(
						Color.white,
						new Gradient(),
						new Gradient(),
						new Gradient()
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Ambient equator color
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Equator Color", "The equator color of the environment lighting."), GUILayout.Width(m_labelWidth));
				switch (m_target.lighting.ambientEquatorColor.propertyType)
				{
					case AzureColorProperty.PropertyType.Color:
						m_target.lighting.ambientEquatorColor.color = EditorGUILayout.ColorField(m_target.lighting.ambientEquatorColor.color);
						break;
					case AzureColorProperty.PropertyType.GradientTimeline:
						EditorGUILayout.PropertyField(m_AmbientEquatorColor.FindPropertyRelative("timelineGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientSunElevation:
						EditorGUILayout.PropertyField(m_AmbientEquatorColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientMoonElevation:
						EditorGUILayout.PropertyField(m_AmbientEquatorColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_AmbientEquatorColor.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.lighting.ambientEquatorColor = new AzureColorProperty
					(
						Color.white,
						new Gradient(),
						new Gradient(),
						new Gradient()
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Ambient ground color
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Ground Color", "The ground color of the environment lighting."), GUILayout.Width(m_labelWidth));
				switch (m_target.lighting.ambientGroundColor.propertyType)
				{
					case AzureColorProperty.PropertyType.Color:
						m_target.lighting.ambientGroundColor.color = EditorGUILayout.ColorField(m_target.lighting.ambientGroundColor.color);
						break;
					case AzureColorProperty.PropertyType.GradientTimeline:
						EditorGUILayout.PropertyField(m_AmbientGroundColor.FindPropertyRelative("timelineGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientSunElevation:
						EditorGUILayout.PropertyField(m_AmbientGroundColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientMoonElevation:
						EditorGUILayout.PropertyField(m_AmbientGroundColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_AmbientGroundColor.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.lighting.ambientGroundColor = new AzureColorProperty
					(
						Color.white,
						new Gradient(),
						new Gradient(),
						new Gradient()
					);
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.Space();
				m_controlRect = EditorGUILayout.GetControlRect();
				GUI.Label(m_controlRect, "", "IN Title");
				GUILayout.Label("Sky Reflection Probe:");
				
				// Intensity
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Intensity", "The intensity of the sky reflection probe."), GUILayout.Width(m_labelWidth));
				switch (m_target.lighting.skyReflectionIntensity.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.lighting.skyReflectionIntensity.slider = EditorGUILayout.Slider(m_target.lighting.skyReflectionIntensity.slider,0.0f, 8.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_SkyReflectionIntensity.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 8.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_SkyReflectionIntensity.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 8.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_SkyReflectionIntensity.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 8.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_SkyReflectionIntensity.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.lighting.skyReflectionIntensity = new AzureFloatProperty
					(
						1.0f,
						AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
						AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
						AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
			}
			
			// Weather tab
			m_controlRect = EditorGUILayout.GetControlRect();
			GUI.Label(m_controlRect, "", m_headerStyle);
			if (GUI.Button(m_controlRect, "WEATHER", m_headerStyle)) m_target.editorSettings.showWeatherTab = !m_target.editorSettings.showWeatherTab;
			GUI.Label(new Rect(m_controlRect.width - 40, m_controlRect.y, m_controlRect.width, 15), m_weatherFoldout);
			if (m_target.editorSettings.showWeatherTab)
			{
				GUILayout.Label("Particles Intensity:");
				
				// Rain light intensity
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Rain Light", "The intensity of light rain particle."), GUILayout.Width(m_labelWidth));
				switch (m_target.weather.rainLightIntensity.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.weather.rainLightIntensity.slider = EditorGUILayout.Slider(m_target.weather.rainLightIntensity.slider,0.0f, 1.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_RainLightIntensity.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_RainLightIntensity.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_RainLightIntensity.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_RainLightIntensity.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.weather.rainLightIntensity = new AzureFloatProperty
					(
						0.0f,
						AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Rain medium intensity
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Rain Medium", "The intensity of medium rain particle."), GUILayout.Width(m_labelWidth));
				switch (m_target.weather.rainMediumIntensity.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.weather.rainMediumIntensity.slider = EditorGUILayout.Slider(m_target.weather.rainMediumIntensity.slider,0.0f, 1.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_RainMediumIntensity.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_RainMediumIntensity.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_RainMediumIntensity.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_RainMediumIntensity.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.weather.rainMediumIntensity = new AzureFloatProperty
					(
						0.0f,
						AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Rain heavy intensity
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Rain Heavy", "The intensity of heavy rain particle."), GUILayout.Width(m_labelWidth));
				switch (m_target.weather.rainHeavyIntensity.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.weather.rainHeavyIntensity.slider = EditorGUILayout.Slider(m_target.weather.rainHeavyIntensity.slider,0.0f, 1.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_RainHeavyIntensity.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_RainHeavyIntensity.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_RainHeavyIntensity.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_RainHeavyIntensity.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.weather.rainHeavyIntensity = new AzureFloatProperty
					(
						0.0f,
						AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Snow intensity
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Snow", "The intensity of snow particle."), GUILayout.Width(m_labelWidth));
				switch (m_target.weather.snowIntensity.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.weather.snowIntensity.slider = EditorGUILayout.Slider(m_target.weather.snowIntensity.slider,0.0f, 1.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_SnowIntensity.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_SnowIntensity.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_SnowIntensity.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_SnowIntensity.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.weather.snowIntensity = new AzureFloatProperty
					(
						0.0f,
						AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.Space();
				GUILayout.Label("Sounds Volume:");
				
				// Rain light volume
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Rain Light", "The volume of the light rain sound effect."), GUILayout.Width(m_labelWidth));
				switch (m_target.weather.rainLightVolume.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.weather.rainLightVolume.slider = EditorGUILayout.Slider(m_target.weather.rainLightVolume.slider,0.0f, 1.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_RainLightVolume.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_RainLightVolume.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_RainLightVolume.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_RainLightVolume.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.weather.rainLightVolume = new AzureFloatProperty
					(
						0.0f,
						AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Rain medium volume
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Rain Medium", "The volume of the medium rain sound effect."), GUILayout.Width(m_labelWidth));
				switch (m_target.weather.rainMediumVolume.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.weather.rainMediumVolume.slider = EditorGUILayout.Slider(m_target.weather.rainMediumVolume.slider,0.0f, 1.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_RainMediumVolume.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_RainMediumVolume.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_RainMediumVolume.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_RainMediumVolume.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.weather.rainMediumVolume = new AzureFloatProperty
					(
						0.0f,
						AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Rain heavy volume
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Rain Heavy", "The volume of the heavy rain sound effect."), GUILayout.Width(m_labelWidth));
				switch (m_target.weather.rainHeavyVolume.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.weather.rainHeavyVolume.slider = EditorGUILayout.Slider(m_target.weather.rainHeavyVolume.slider,0.0f, 1.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_RainHeavyVolume.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_RainHeavyVolume.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_RainHeavyVolume.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_RainHeavyVolume.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.weather.rainHeavyVolume = new AzureFloatProperty
					(
						0.0f,
						AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				/////////////////////////
				/// /////////////////////
				/// /////////////////////
				// Wind light volume
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Wind Light", "The volume of the light wind sound effect."), GUILayout.Width(m_labelWidth));
				switch (m_target.weather.windLightVolume.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.weather.windLightVolume.slider = EditorGUILayout.Slider(m_target.weather.windLightVolume.slider,0.0f, 1.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_WindLightVolume.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_WindLightVolume.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_WindLightVolume.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_WindLightVolume.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.weather.windLightVolume = new AzureFloatProperty
					(
						0.0f,
						AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Wind medium volume
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Wind Medium", "The volume of the medium wind sound effect."), GUILayout.Width(m_labelWidth));
				switch (m_target.weather.windMediumVolume.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.weather.windMediumVolume.slider = EditorGUILayout.Slider(m_target.weather.windMediumVolume.slider,0.0f, 1.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_WindMediumVolume.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_WindMediumVolume.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_WindMediumVolume.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_WindMediumVolume.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.weather.windMediumVolume = new AzureFloatProperty
					(
						0.0f,
						AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Wind heavy volume
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Wind Heavy", "The volume of the heavy wind sound effect."), GUILayout.Width(m_labelWidth));
				switch (m_target.weather.windHeavyVolume.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.weather.windHeavyVolume.slider = EditorGUILayout.Slider(m_target.weather.windHeavyVolume.slider,0.0f, 1.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_WindHeavyVolume.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_WindHeavyVolume.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_WindHeavyVolume.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_WindHeavyVolume.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.weather.windHeavyVolume = new AzureFloatProperty
					(
						0.0f,
						AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.Space();
				GUILayout.Label("Particles Color:");
				
				// Rain color
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Rain Color", "The color of the rain particles effect."), GUILayout.Width(m_labelWidth));
				switch (m_target.weather.rainColor.propertyType)
				{
					case AzureColorProperty.PropertyType.Color:
						m_target.weather.rainColor.color = EditorGUILayout.ColorField(m_target.weather.rainColor.color);
						break;
					case AzureColorProperty.PropertyType.GradientTimeline:
						EditorGUILayout.PropertyField(m_RainColor.FindPropertyRelative("timelineGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientSunElevation:
						EditorGUILayout.PropertyField(m_RainColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientMoonElevation:
						EditorGUILayout.PropertyField(m_RainColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_RainColor.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.weather.rainColor = new AzureColorProperty
					(
						Color.white,
						new Gradient(),
						new Gradient(),
						new Gradient()
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Snow color
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Snow Color", "The color of the snow particles effect."), GUILayout.Width(m_labelWidth));
				switch (m_target.weather.snowColor.propertyType)
				{
					case AzureColorProperty.PropertyType.Color:
						m_target.weather.snowColor.color = EditorGUILayout.ColorField(m_target.weather.snowColor.color);
						break;
					case AzureColorProperty.PropertyType.GradientTimeline:
						EditorGUILayout.PropertyField(m_SnowColor.FindPropertyRelative("timelineGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientSunElevation:
						EditorGUILayout.PropertyField(m_SnowColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
					case AzureColorProperty.PropertyType.GradientMoonElevation:
						EditorGUILayout.PropertyField(m_SnowColor.FindPropertyRelative("sunElevationGradient"), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_SnowColor.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.weather.snowColor = new AzureColorProperty
					(
						Color.white,
						new Gradient(),
						new Gradient(),
						new Gradient()
					);
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.Space();
				GUILayout.Label("Wind:");
				
				// Wind speed
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Speed", "Controls the wind speed."), GUILayout.Width(m_labelWidth));
				switch (m_target.weather.windSpeed.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.weather.windSpeed.slider = EditorGUILayout.Slider(m_target.weather.windSpeed.slider,0.0f, 1.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_WindSpeed.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_WindSpeed.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_WindSpeed.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_WindSpeed.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.weather.windSpeed = new AzureFloatProperty
					(
						0.0f,
						AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				// Wind direction
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Direction", "Controls the wind direction."), GUILayout.Width(m_labelWidth));
				switch (m_target.weather.windDirection.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.weather.windDirection.slider = EditorGUILayout.Slider(m_target.weather.windDirection.slider,0.0f, 1.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_WindDirection.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_WindDirection.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_WindDirection.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_WindDirection.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.weather.windDirection = new AzureFloatProperty
					(
						0.5f,
						AnimationCurve.Linear (0.0f, 0.5f, 24.0f, 0.5f),
						AnimationCurve.Linear (-1.0f, 0.5f, 1.0f, 0.5f),
						AnimationCurve.Linear (-1.0f, 0.5f, 1.0f, 0.5f)
					);
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.Space();
				GUILayout.Label("Thunder:");
				
				// Thunder multiplier
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Multiplier", "How much the thunder will lighting the clouds."), GUILayout.Width(m_labelWidth));
				switch (m_target.weather.thunderMultiplier.propertyType)
				{
					case AzureFloatProperty.PropertyType.Slider:
						m_target.weather.thunderMultiplier.slider = EditorGUILayout.Slider(m_target.weather.thunderMultiplier.slider,0.0f, 1.0f);
						break;
					case AzureFloatProperty.PropertyType.CurveTimeline:
						EditorGUILayout.CurveField(m_ThunderMultiplier.FindPropertyRelative("timelineCurve"), Color.green, new Rect(0.0f, 0.0f, 24.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveSunElevation:
						EditorGUILayout.CurveField(m_ThunderMultiplier.FindPropertyRelative("sunElevationCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
					case AzureFloatProperty.PropertyType.CurveMoonElevation:
						EditorGUILayout.CurveField(m_ThunderMultiplier.FindPropertyRelative("sunElevationCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
						break;
				}

				EditorGUILayout.PropertyField(m_ThunderMultiplier.FindPropertyRelative("propertyType"), GUIContent.none, GUILayout.Width(m_propertyTypeWidth));
				if (GUILayout.Button(new GUIContent("R", "Reset the property."), EditorStyles.miniButton, GUILayout.Width(17), GUILayout.Height(15)))
				{
					m_target.weather.thunderMultiplier = new AzureFloatProperty
					(
						0.0f,
						AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
						AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
					);
				}
				EditorGUILayout.EndHorizontal();
			}
			
			// Outputs tab
			m_controlRect = EditorGUILayout.GetControlRect();
			GUI.Label(m_controlRect, "", m_headerStyle);
			if (GUI.Button(m_controlRect, "OUTPUTS", m_headerStyle)) m_target.editorSettings.showOutputsTab = !m_target.editorSettings.showOutputsTab;
			GUI.Label(new Rect(m_controlRect.width - 40, m_controlRect.y, m_controlRect.width, 15), m_outputsFoldout);
			if (m_target.editorSettings.showOutputsTab)
			{
				m_target.outputTag = EditorGUILayout.TagField(new GUIContent("Output Tag", "The Sky Manager will create and perform the outputs only to the profiles using this same tag."), m_target.outputTag);
				m_reorderableOutputsList.DoLayoutList();
			}


			// End custom Inspector
			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(target);
		}
	}
}