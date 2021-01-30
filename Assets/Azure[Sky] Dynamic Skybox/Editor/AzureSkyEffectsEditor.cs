using UnityEngine.AzureSky;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.AzureSky
{
	[CustomEditor(typeof(AzureSkyEffects))]
	public class AzureSkyEffectsEditor : Editor
	{
		private AzureSkyEffects m_target;
		private GUIStyle m_headerStyle;
		private readonly Color m_customGreenColor = new Color(0.75f, 1.0f, 0.75f);
		private readonly Color m_customRedColor = new Color(1.0f, 0.5f, 0.5f);
		
		// Serialized Properties
		private SerializedProperty m_WindMultiplier;
		private SerializedProperty m_RainLight;
		private SerializedProperty m_RainMedium;
		private SerializedProperty m_RainHeavy;
		private SerializedProperty m_WindLight;
		private SerializedProperty m_WindMedium;
		private SerializedProperty m_WindHeavy;
		private SerializedProperty m_ThunderSettingsList;
		private SerializedProperty m_ParticlesDriverList;
		
		private ReorderableList m_reorderableThunderSettingsList;
		private ReorderableList m_reorderableParticlesList;

		private void OnEnable()
		{
			// Get Target
			m_target = (AzureSkyEffects) target;
			
			// Initialize serialized properties
			m_WindMultiplier = serializedObject.FindProperty("windMultiplier");
			m_RainLight = serializedObject.FindProperty("rainLight");
			m_RainMedium = serializedObject.FindProperty("rainMedium");
			m_RainHeavy = serializedObject.FindProperty("rainHeavy");
			m_WindLight = serializedObject.FindProperty("windLight");
			m_WindMedium = serializedObject.FindProperty("windMedium");
			m_WindHeavy = serializedObject.FindProperty("windHeavy");
			m_ThunderSettingsList = serializedObject.FindProperty("thunderSettingsList");
			m_ParticlesDriverList = serializedObject.FindProperty("particlesDriverList");
			
			m_reorderableThunderSettingsList = new ReorderableList(serializedObject, m_ThunderSettingsList, true, true, true, true)
			{
				drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
				{
					var element = m_reorderableThunderSettingsList.serializedProperty.GetArrayElementAtIndex(index);
					rect.y += 2;
					
					// Header title
					EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Thunder " + index.ToString(), m_headerStyle);
					rect.y += EditorGUIUtility.singleLineHeight +1;
					EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("thunderPrefab"), new GUIContent("Thunder Prefab", "The prefab that will be instantiated to create the thunder effect."));
					
					rect.y += EditorGUIUtility.singleLineHeight +1;
					EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("audioClip"), new GUIContent("Audio Clip", "The audio clip used for this instance of the thunder effect."));

					rect.y += EditorGUIUtility.singleLineHeight +1;
					EditorGUI.CurveField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("lightFrequency"), Color.yellow, new Rect(0.0f, 0.0f, 1.0f, 1.0f), new GUIContent("Light Frequency", "The light intensity frequency of this thunder effect."));
					
					rect.y += EditorGUIUtility.singleLineHeight +1;
					EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("audioDelay"), new GUIContent("Audio Delay", "The delay time to play the audio after this thunder effect has been created."));
					
					rect.y += EditorGUIUtility.singleLineHeight +1;
					EditorGUI.LabelField(rect, new GUIContent("Position:", "The position in the world where the thunder must be instantiated."));
					
					rect.y += EditorGUIUtility.singleLineHeight +1;
					EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("position"), GUIContent.none);
					
					rect.y += EditorGUIUtility.singleLineHeight +1;
					if (GUI.Button(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Test"))
					{
						if(Application.isPlaying)
							m_target.InstantiateThunderEffect(index);
						else
							Debug.Log("The application must be playing to perform a thunder effect.");
					}
				},
				
				elementHeightCallback = (int index) => EditorGUIUtility.singleLineHeight * 9.0f,

				drawHeaderCallback = (Rect rect) =>
				{
					EditorGUI.LabelField(rect,new GUIContent("Thunder List", ""), EditorStyles.boldLabel);
				},

				drawElementBackgroundCallback = (rect, index, active, focused) =>
				{
					if (active)
						GUI.Box(new Rect(rect.x +2, rect.y -1, rect.width -4, rect.height +1), "","selectionRect");
				},
				
				onAddCallback = (ReorderableList l) =>
				{
					var index = l.serializedProperty.arraySize;
					l.serializedProperty.arraySize++;
					l.index = index;
					var element = l.serializedProperty.GetArrayElementAtIndex(index);
					element.FindPropertyRelative("lightFrequency").animationCurveValue = AnimationCurve.Constant(0.0f, 1.0f, 0.0f);
				}
			};
			
			m_reorderableParticlesList = new ReorderableList(serializedObject, m_ParticlesDriverList, false, true, true, true)
			{
				drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
				{
					var element = m_reorderableParticlesList.serializedProperty.GetArrayElementAtIndex(index);
					rect.y += 2;
					
					// Header title
					EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Particle Group " + index.ToString(), m_headerStyle);
					
					rect.y += EditorGUIUtility.singleLineHeight +1;
					EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("particleTransform"), new GUIContent("Particle Transform", "The particle system group to be managed."));

					rect.y += EditorGUIUtility.singleLineHeight +1;
					EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("followTargetTransform"), new GUIContent("Target Transform", "Target transform used to set the position of particle emitters. (Generally the MainCamera or Player transform)"));
				},
				
				elementHeightCallback = (int index) => EditorGUIUtility.singleLineHeight * 3.25f,

				drawHeaderCallback = (Rect rect) =>
				{
					EditorGUI.LabelField(rect,new GUIContent("Follow Manager", ""), EditorStyles.boldLabel);
				},

				drawElementBackgroundCallback = (rect, index, active, focused) =>
				{
					if (active)
						GUI.Box(new Rect(rect.x +2, rect.y -1, rect.width -4, rect.height +1), "","selectionRect");
				},
			};
		}

		public override void OnInspectorGUI()
		{
			// Start custom Inspector
			Undo.RecordObject(m_target, "Undo Azure Sky Effects");
			serializedObject.Update();
			
			// Initializing the custom styles
			m_headerStyle = new GUIStyle("selectionRect")
			{
				fontStyle = FontStyle.Bold,
				contentOffset = new Vector2(0f, -2f)
			};
			
			// Sky Manager
			GUI.color = m_customGreenColor;
			if (!m_target.azureSky) { GUI.color = m_customRedColor; }
			m_target.azureSky = (AzureSkyManager)EditorGUILayout.ObjectField(new GUIContent("Sky Manager", "Reference to Azure Sky Manager."), m_target.azureSky, typeof(AzureSkyManager), true);
			GUILayout.Space(2);
			
			// Wind zone
			GUI.color = m_customGreenColor;
			if (!m_target.windZone) { GUI.color = m_customRedColor; }
			m_target.windZone = (WindZone)EditorGUILayout.ObjectField(new GUIContent("Wind Zone", "The wind zone."), m_target.windZone, typeof(WindZone), true);
			GUILayout.Space(2);
			
			GUI.color = Color.white;
			
			// Wind multiplier
			EditorGUILayout.PropertyField(m_WindMultiplier, new GUIContent("Wind Multiplier", "The wind zone multiplier."));

			GUI.Label(EditorGUILayout.GetControlRect(), "Sounds", m_headerStyle);
			EditorGUILayout.PropertyField(m_RainLight, new GUIContent("Rain Light", "The light rain audio fx."));
			EditorGUILayout.PropertyField(m_RainMedium, new GUIContent("Rain Medium", "The medium rain audio fx."));
			EditorGUILayout.PropertyField(m_RainHeavy, new GUIContent("Rain Heavy", "The heavy rain audio fx."));
			EditorGUILayout.PropertyField(m_WindLight, new GUIContent("Wind Light", "The light wind audio fx."));
			EditorGUILayout.PropertyField(m_WindMedium, new GUIContent("Wind Medium", "The medium wind audio fx."));
			EditorGUILayout.PropertyField(m_WindHeavy, new GUIContent("Wind Heavy", "The heavy wind audio fx."));
			EditorGUILayout.Space();
			
			GUI.Label(EditorGUILayout.GetControlRect(), "Particles", m_headerStyle);
			
			// Rain material
			GUI.color = m_customGreenColor;
			if (!m_target.particleRainMaterial) { GUI.color = m_customRedColor; }
			m_target.particleRainMaterial = (Material)EditorGUILayout.ObjectField(new GUIContent("Rain Material", "The material used to render the rain particles."), m_target.particleRainMaterial, typeof(Material), true);
			GUILayout.Space(2);
			
			// Heavy rain material
			GUI.color = m_customGreenColor;
			if (!m_target.particleHeavyRainMaterial) { GUI.color = m_customRedColor; }
			m_target.particleHeavyRainMaterial = (Material)EditorGUILayout.ObjectField(new GUIContent("Heavy Rain Material", "The material used to render the heavy rain particles."), m_target.particleHeavyRainMaterial, typeof(Material), true);
			GUILayout.Space(2);
			
			// Snow material
			GUI.color = m_customGreenColor;
			if (!m_target.particleSnowMaterial) { GUI.color = m_customRedColor; }
			m_target.particleSnowMaterial = (Material)EditorGUILayout.ObjectField(new GUIContent("Snow Material", "The material used to render the snow particles."), m_target.particleSnowMaterial, typeof(Material), true);
			GUILayout.Space(2);
			
			// Ripple material
			GUI.color = m_customGreenColor;
			if (!m_target.particleRippleMaterial) { GUI.color = m_customRedColor; }
			m_target.particleRippleMaterial = (Material)EditorGUILayout.ObjectField(new GUIContent("Ripple Material", "The material used to render the ripple particles."), m_target.particleRippleMaterial, typeof(Material), true);
			GUILayout.Space(2);
			
			// Light rain particle
			GUI.color = m_customGreenColor;
			if (!m_target.rainLightParticle) { GUI.color = m_customRedColor; }
			m_target.rainLightParticle = (ParticleSystem)EditorGUILayout.ObjectField(new GUIContent("Light Rain Particle", "The light rain particle system."), m_target.rainLightParticle, typeof(ParticleSystem), true);
			GUILayout.Space(2);
			
			// Medium rain particle
			GUI.color = m_customGreenColor;
			if (!m_target.rainMediumParticle) { GUI.color = m_customRedColor; }
			m_target.rainMediumParticle = (ParticleSystem)EditorGUILayout.ObjectField(new GUIContent("Medium Rain Particle", "The medium rain particle system."), m_target.rainMediumParticle, typeof(ParticleSystem), true);
			GUILayout.Space(2);
			
			// Heavy rain particle
			GUI.color = m_customGreenColor;
			if (!m_target.rainHeavyParticle) { GUI.color = m_customRedColor; }
			m_target.rainHeavyParticle = (ParticleSystem)EditorGUILayout.ObjectField(new GUIContent("Heavy Rain Particle", "The heavy rain particle system."), m_target.rainHeavyParticle, typeof(ParticleSystem), true);
			GUILayout.Space(2);
			
			// Snow particle
			GUI.color = m_customGreenColor;
			if (!m_target.snowParticle) { GUI.color = m_customRedColor; }
			m_target.snowParticle = (ParticleSystem)EditorGUILayout.ObjectField(new GUIContent("Snow Particle", "The snow particle system."), m_target.snowParticle, typeof(ParticleSystem), true);
			GUILayout.Space(2);
			
			GUI.color = Color.white;
			
			m_reorderableParticlesList.DoLayoutList();
			
			
			EditorGUILayout.Space();
			GUI.Label(EditorGUILayout.GetControlRect(), "Thunders", m_headerStyle);
			m_reorderableThunderSettingsList.DoLayoutList();
			
			
			// End custom Inspector
			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(target);
		}
	}
}