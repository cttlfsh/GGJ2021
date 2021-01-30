using System;
using UnityEngine;
using UnityEngine.AzureSky;
using UnityEditorInternal;

namespace UnityEditor.AzureSky
{
	[CustomEditor(typeof(AzureSkyManager))]
	public class AzureSkyManagerEditor : Editor
	{
		private AzureSkyManager m_target;
		private readonly AzureEditorUtilities m_editorUtilities = new AzureEditorUtilities();
		
		// Header bar foldout strings
		private string m_timeControllerFoldout;
		private string m_referencesFoldout;
		private string m_profileControllerFoldout;
		private string m_eventSystemFoldout;
		private string m_optionsFoldout;
		private string m_outputsFoldout;
		
		// Custom InspectorGUI
		private Rect m_controlRect;
		private Texture2D m_logoTexture;
		private GUIStyle m_headerStyle;
		private GUIStyle m_textBarStyle;
		private Vector2 m_timeDisplay;
		private bool m_showDateSelector = false;
		private readonly Color m_customGreenColor = new Color(0.75f, 1.0f, 0.75f);
		private readonly Color m_customRedColor = new Color(1.0f, 0.5f, 0.5f);
		
		// Serialized Properties
		private SerializedProperty m_TimeMode;
		private SerializedProperty m_RepeatMode;
		private SerializedProperty m_DayLengthCurve;
		private SerializedProperty m_DefaultProfileTransition;
		private SerializedProperty m_SkyModel;
		private SerializedProperty m_StylizedSunsetMode;
		private SerializedProperty m_EarthScale;
		private SerializedProperty m_EarthPosition;
		private SerializedProperty m_CloudMode;
		private SerializedProperty m_PrecomputedCloudMode;
		private SerializedProperty m_ReflectionProbe;
		private SerializedProperty m_ReflectionProbeRefreshMode;
		private SerializedProperty m_EventScanMode;
		
		private SerializedProperty m_WeatherProfileList;
		private SerializedProperty m_DefaultProfileList;
		private SerializedProperty m_WeatherZoneList;
		private SerializedProperty m_EventSystemList;
		private SerializedProperty m_OutputSystemList;
		
		// Reorderable lists
		private ReorderableList m_reorderableWeatherProfileList;
		private ReorderableList m_reorderableDefaultProfileList;
		private ReorderableList m_reorderableWeatherZoneList;
		private ReorderableList m_reorderableEventSystemList;
		private ReorderableList m_reorderableOutputSystemList;

		private void OnEnable()
		{
			// Get target
			m_target = (AzureSkyManager) target;
			m_target.timeController.UpdateCalendar();

			// Initialize serialized properties
			m_TimeMode = serializedObject.FindProperty("timeController.timeMode");
			m_RepeatMode = serializedObject.FindProperty("timeController.repeatMode");
			m_DayLengthCurve = serializedObject.FindProperty("timeController.dayLengthCurve");
			m_DefaultProfileTransition = serializedObject.FindProperty("profileController.defaultProfileTransitionTime");
			m_SkyModel = serializedObject.FindProperty("options.skyModel");
			m_StylizedSunsetMode = serializedObject.FindProperty("options.stylizedSunsetMode");
			m_CloudMode = serializedObject.FindProperty("options.cloudMode");
			m_PrecomputedCloudMode = serializedObject.FindProperty("options.precomputedCloudMode");
			m_EarthScale = serializedObject.FindProperty("options.earthScale");
			m_EarthPosition = serializedObject.FindProperty("options.earthPosition");
			m_ReflectionProbe = serializedObject.FindProperty("options.reflectionProbeMode");
			m_ReflectionProbeRefreshMode = serializedObject.FindProperty("options.reflectionProbeRefreshMode");
			m_EventScanMode = serializedObject.FindProperty("eventScanMode");
			
			m_WeatherProfileList = serializedObject.FindProperty("profileController.weatherProfileList");
			m_DefaultProfileList = serializedObject.FindProperty("profileController.defaultProfileList");
			m_WeatherZoneList = serializedObject.FindProperty("profileController.weatherZoneList");
			m_EventSystemList = serializedObject.FindProperty("eventSystemList");
			m_OutputSystemList = serializedObject.FindProperty("outputSystemList");
			
			// Create weather profile list
            m_reorderableWeatherProfileList = new ReorderableList(serializedObject, m_WeatherProfileList, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += 2;
                    string standardProfileName = "None Profile";
                    var element = m_reorderableWeatherProfileList.serializedProperty.GetArrayElementAtIndex(index);
                    var transition = element.FindPropertyRelative("transitionTime");
                    var myList = m_target.profileController.weatherProfileList[index];

                    // Profile index
                    EditorGUI.LabelField(rect, "Profile  " + index.ToString());
                    
                    // Real object field
                    GUI.color = Color.clear;
                    myList.profile = (AzureSkyProfile)EditorGUI.ObjectField(new Rect(rect.x + 65, rect.y, rect.width - 100 - 28, EditorGUIUtility.singleLineHeight), GUIContent.none, myList.profile, typeof(AzureSkyProfile), false);
                    GUI.color = Color.white;

                    // Draws a custom object field just to display the profile name
                    GUI.color = m_customRedColor;
                    if (m_target.profileController.weatherProfileList[index].profile)
                    {
                        GUI.color = m_customGreenColor;
                        standardProfileName = m_target.profileController.weatherProfileList[index].profile.name;
                    }
                    EditorGUI.LabelField(new Rect(rect.x + 65, rect.y, rect.width - 100 - 28, EditorGUIUtility.singleLineHeight), standardProfileName, EditorStyles.objectField);
                    GUI.color = Color.white;

                    // Transition time field
                    EditorGUI.PropertyField(new Rect(rect.x + rect.width - 61, rect.y, 28, EditorGUIUtility.singleLineHeight), transition, GUIContent.none);
   
                    // Go button
                    if (GUI.Button(new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight), new GUIContent("Go", "Changes the global weather to this specific profile in the list.")))
                    {
                        if (Application.isPlaying)
                        {
	                        m_target.SetNewWeatherProfile (index);
                        }
                        else
                        {
                            Debug.Log("To perform a weather transition, the application must be playing.");
                        }
                    }
                    m_target.profileController.weatherProfileList[index] = myList;
                },

                onAddCallback = (ReorderableList l) =>
                {
                    var index = l.serializedProperty.arraySize;
                    l.serializedProperty.arraySize++;
                    l.index = index;

                    var element = l.serializedProperty.GetArrayElementAtIndex(index);
                    element.FindPropertyRelative("transitionTime").floatValue = 10.0f;
                },

                drawElementBackgroundCallback = (rect, index, active, focused) =>
                {
                    if (active)
	                    GUI.Box(new Rect(rect.x +2, rect.y -1, rect.width -4, rect.height +1), "","selectionRect");
                }
        	};
            
            // Create default profile list
            m_reorderableDefaultProfileList = new ReorderableList(serializedObject, m_DefaultProfileList, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
	                rect.y += 2;
	                string profileName = "None Profile";
	                Rect fieldRect = new Rect(rect.x + 65, rect.y, rect.width - 65, EditorGUIUtility.singleLineHeight);
                   
	                // Real object field
	                GUI.color = Color.clear;
	                EditorGUI.LabelField(rect, "profile " + index.ToString());
	                m_target.profileController.defaultProfileList[index] = (AzureSkyProfile)EditorGUI.ObjectField(fieldRect, GUIContent.none, m_target.profileController.defaultProfileList[index], typeof(AzureSkyProfile), false);

	                // Draws a custom object field just to display the profile name
	                GUI.color = m_customRedColor;
	                if (m_target.profileController.defaultProfileList[index])
	                {
		                profileName = m_target.profileController.defaultProfileList[index].name;
		                GUI.color = m_customGreenColor;
	                }
	                EditorGUI.LabelField(rect, "Profile  " + index.ToString());
	                EditorGUI.LabelField(fieldRect, profileName, EditorStyles.objectField);
	                GUI.color = Color.white;
                },

                onAddCallback = (ReorderableList l) =>
                {
                    var index = l.serializedProperty.arraySize;
                    l.serializedProperty.arraySize++;
                    l.index = index;
                },

                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect,new GUIContent("Default Day Profiles", "Stores the default day profiles. A random profile from this list will be used by sky manager every time the next day starts."), EditorStyles.boldLabel);
                },

                drawElementBackgroundCallback = (rect, index, active, focused) =>
                {
                    if (active)
	                    GUI.Box(new Rect(rect.x +2, rect.y -1, rect.width -4, rect.height +1), "","selectionRect");
                }
        	};
            
            // Create climate zones list
            m_reorderableWeatherZoneList = new ReorderableList(serializedObject, m_WeatherZoneList, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
	                rect.y += 2;
	                string zoneName = "None Weather Zone";
	                Rect fieldRect = new Rect(rect.x + 65, rect.y, rect.width - 65, EditorGUIUtility.singleLineHeight);
                   
	                // Real object field
	                GUI.color = Color.clear;
	                EditorGUI.LabelField(rect, "Priority " + index.ToString());
	                m_target.profileController.weatherZoneList[index] = (AzureSkyWeatherZone)EditorGUI.ObjectField(fieldRect, GUIContent.none, m_target.profileController.weatherZoneList[index], typeof(AzureSkyWeatherZone), true);

	                // Draws a custom object field just to display the profile name
	                GUI.color = m_customRedColor;
	                if (m_target.profileController.weatherZoneList[index])
	                {
		                zoneName = m_target.profileController.weatherZoneList[index].name;
		                GUI.color = m_customGreenColor;
	                }
	                EditorGUI.LabelField(rect, "Priority  " + index.ToString());
	                EditorGUI.LabelField(fieldRect, zoneName, EditorStyles.objectField);
	                GUI.color = Color.white;
                },

                onAddCallback = (ReorderableList l) =>
                {
                    var index = l.serializedProperty.arraySize;
                    l.serializedProperty.arraySize++;
                    l.index = index;
                },

                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect,new GUIContent("Local Weather Zones", "Place here all the local weather zones and arrange according to its priorities."), EditorStyles.boldLabel);
                },

                drawElementBackgroundCallback = (rect, index, active, focused) =>
                {
                    if (active)
	                    GUI.Box(new Rect(rect.x +2, rect.y -1, rect.width -4, rect.height +1), "","selectionRect");
                }
        	};
            
            // Create event system list
            m_reorderableEventSystemList = new ReorderableList(serializedObject, m_EventSystemList, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
	                var element = m_reorderableEventSystemList.serializedProperty.GetArrayElementAtIndex(index);
	                element.isExpanded = false;
	                rect.y += 2;
	                var lineHeight = EditorGUIUtility.singleLineHeight;
	                
	                // Header title
	                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, lineHeight), "Event Action " + index.ToString(), m_headerStyle);
	                
	                // Header foldout 
	                GUI.color = Color.clear;
	                if (GUI.Button( new Rect(rect.x, rect.y, rect.width, lineHeight), GUIContent.none))
	                {
		                m_target.eventSystemList[index].isExpanded = !m_target.eventSystemList[index].isExpanded;
	                }
	                GUI.color = Color.white;
	                EditorGUI.Foldout(new Rect(rect.width+25, rect.y, 25, lineHeight), m_target.eventSystemList[index].isExpanded, GUIContent.none);

	                // Draw the event interface
	                if (m_target.eventSystemList[index].isExpanded)
	                {
		                float offset = rect.width / 3;
		                rect.y += EditorGUIUtility.singleLineHeight - 1;

		                // Date settings
		                EditorGUI.LabelField( new Rect(rect.x, rect.y, rect.width, lineHeight + 7), "", m_headerStyle);
		                rect.y += 4;
		                EditorGUI.LabelField(rect, "Day");
		                EditorGUI.PropertyField( new Rect(rect.x + 32, rect.y, offset - 48, lineHeight), element.FindPropertyRelative("day"), GUIContent.none);
		                EditorGUI.LabelField( new Rect(offset + 25, rect.y, rect.width, lineHeight), "Month");
		                EditorGUI.PropertyField( new Rect(offset + 65, rect.y, offset - 50, lineHeight), element.FindPropertyRelative("month"), GUIContent.none);
		                EditorGUI.LabelField( new Rect(offset * 2 + 30, rect.y, rect.width, lineHeight), "Year");
		                EditorGUI.PropertyField( new Rect(offset * 2 + 62, rect.y, rect.width - (offset * 2 + 30), lineHeight), element.FindPropertyRelative("year"), GUIContent.none);

		                rect.y += EditorGUIUtility.singleLineHeight + 2;
		                offset = rect.width / 2;

		                // Time settings
		                EditorGUI.LabelField( new Rect(rect.x, rect.y, rect.width, lineHeight + 7), "", m_headerStyle);
		                rect.y += 4;
		                EditorGUI.LabelField(rect, "Hour");
		                EditorGUI.PropertyField( new Rect(rect.x + 32, rect.y, offset - 50, lineHeight), element.FindPropertyRelative("hour"), GUIContent.none);
		                EditorGUI.LabelField( new Rect(offset + 20, rect.y, rect.width, lineHeight), "Minute");
		                EditorGUI.PropertyField( new Rect(offset + 62, rect.y, rect.width - offset - 30, lineHeight), element.FindPropertyRelative("minute"), GUIContent.none);

		                // Events
		                rect.y += EditorGUIUtility.singleLineHeight + 4;
		                EditorGUI.PropertyField( new Rect(rect.x, rect.y, rect.width, lineHeight), element.FindPropertyRelative("eventAction"), GUIContent.none);
	                }
                },

                onAddCallback = (ReorderableList l) =>
                {
                    var index = l.serializedProperty.arraySize;
                    l.serializedProperty.arraySize++;
                    l.index = index;
                },

                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect,new GUIContent("Events", " "), EditorStyles.boldLabel);
                },
                
                elementHeightCallback = (int index) =>
                {
	                var elementHeight = m_target.eventSystemList[index].eventAction.GetPersistentEventCount();
	                var isExpanded = m_target.eventSystemList[index].isExpanded;
	                if (elementHeight >= 1)
	                {
		                elementHeight--;
	                }

	                if (isExpanded)
	                {
		                return (EditorGUIUtility.singleLineHeight * 8) + elementHeight * (EditorGUIUtility.singleLineHeight * 2.7f) + 20;
	                }
	                else
	                {
		                return EditorGUIUtility.singleLineHeight * 1.25f;
	                }
                },

                drawElementBackgroundCallback = (rect, index, active, focused) =>
                {
                    if (active)
	                    GUI.Box(new Rect(rect.x +2, rect.y -1, rect.width -4, rect.height +1), "","selectionRect");
                }
        	};
            
            // Create output list
            m_reorderableOutputSystemList = new ReorderableList(serializedObject, m_OutputSystemList, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
	                var element = m_reorderableOutputSystemList.serializedProperty.GetArrayElementAtIndex(index);
	                rect.y += 2;
	                
	                // Header title
	                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Output Index " + index.ToString(), m_headerStyle);
					
	                // Description
	                rect.y += EditorGUIUtility.singleLineHeight +1;
	                EditorGUI.PropertyField( new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("azureOutputType"), new GUIContent("Output Type", "Set the property type of this output."));
	                rect.y += EditorGUIUtility.singleLineHeight;
	                
	                element.FindPropertyRelative("description").stringValue = EditorGUI.TextArea(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 3.0f), element.FindPropertyRelative("description").stringValue);
                },

                onAddCallback = (ReorderableList l) =>
                {
                    var index = l.serializedProperty.arraySize;
                    l.serializedProperty.arraySize++;
                    l.index = index;
                    var element = l.serializedProperty.GetArrayElementAtIndex(index);
                    element.FindPropertyRelative("description").stringValue = "Description:";
                    
                    //string[] guids;
                    // search for a ScriptObject called ScriptObj
                    //guids = AssetDatabase.FindAssets("t:AzureSkyProfile");
                    //foreach (string guid in guids)
                    //{
	                    //Debug.Log("AzureSkyProfile+bottle: " + AssetDatabase.GUIDToAssetPath(guid));
	                    //AzureSkyProfile temProfile = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid),typeof(AzureSkyProfile)) as AzureSkyProfile;
	                    //temProfile.name = temProfile.name + "test";
	                    //Debug.Log(temProfile.name);
                    //}
                },
                
                elementHeightCallback = (int index) => EditorGUIUtility.singleLineHeight * 5.5f,

                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect,new GUIContent("Outputs", ""), EditorStyles.boldLabel);
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
			serializedObject.Update();
			Undo.RecordObject(m_target, "Undo Azure Sky Manager");
			
			// Initializing the custom styles
			m_headerStyle = new GUIStyle("selectionRect")
			{
				fontStyle = FontStyle.Bold,
				contentOffset = new Vector2(0f, -2f)
			};
			
			m_textBarStyle = new GUIStyle("WhiteMiniLabel")
			{
				alignment = TextAnchor.MiddleCenter
			};
			
			// Setting header bar foldout strings
			m_timeControllerFoldout = m_target.editorSettings.showTimeControllerTab ? "| Hide" : "| Show";
			m_referencesFoldout = m_target.editorSettings.showReferencesTab ? "| Hide" : "| Show";
			m_profileControllerFoldout = m_target.editorSettings.showProfileControllerTab ? "| Hide" : "| Show";
			m_eventSystemFoldout = m_target.editorSettings.showEventSystemTab ? "| Hide" : "| Show";
			m_optionsFoldout = m_target.editorSettings.showOptionsTab ? "| Hide" : "| Show";
			m_outputsFoldout = m_target.editorSettings.showOutputsTab ? "| Hide" : "| Show";
			
			// Azure logo
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical("selectionRect");
			m_controlRect = EditorGUILayout.GetControlRect();
			m_logoTexture = AssetDatabase.LoadAssetAtPath("Assets/Azure[Sky] Dynamic Skybox/Editor/InspectorGui/AzureSkyControllerBar.png",typeof(Texture2D)) as Texture2D;
			GUI.DrawTexture(new Rect(m_controlRect.x, m_controlRect.y, 256, 55), m_logoTexture);
			GUILayout.Space(32);
			GUILayout.Label("Version 5.0.1", EditorStyles.miniLabel);
			EditorGUILayout.EndVertical();
			
			// Time Controller tab
			m_controlRect = EditorGUILayout.GetControlRect();
			GUI.Label(m_controlRect, "", m_headerStyle);
			if (GUI.Button(m_controlRect, "TIME CONTROLLER", m_headerStyle)) m_target.editorSettings.showTimeControllerTab = !m_target.editorSettings.showTimeControllerTab;
			GUI.Label(new Rect(m_controlRect.width - 40, m_controlRect.y, m_controlRect.width, 15), m_timeControllerFoldout);
			if(m_target.editorSettings.showTimeControllerTab)
			{
				// Draws the date selector above the calendar header buttons
				if (m_showDateSelector)
				{
					m_editorUtilities.DrawDateSelector(m_target, ref m_showDateSelector);
				}
				
				// Draws the calendar header buttons
				m_editorUtilities.DrawCalendarHeaderButtons(m_target, ref m_showDateSelector);
				
				// Draws the calendar
				m_editorUtilities.DrawCalendar(m_target);
				
				// Adds a splitter bar
				m_controlRect = EditorGUILayout.GetControlRect();
				GUI.Label(m_controlRect, "", "IN Title");
				GUILayout.Space(-14);
				
				// Draws the properties
				EditorGUILayout.PropertyField(m_TimeMode, new GUIContent("Time Mode", "Sets the method used to apply the sun and moon rotation."));
				EditorGUILayout.PropertyField(m_RepeatMode, new GUIContent("Repeat Mode", "Sets the repeat method used to change the date."));
				m_target.timeController.timeline = EditorGUILayout.Slider(new GUIContent("Timeline", "The current 'time position' in the day-night cycle."), m_target.timeController.timeline, 0, 24);
				m_target.timeController.latitude = EditorGUILayout.Slider(new GUIContent("Latitude", "The north-south angle of a position on the Earth's surface."), m_target.timeController.latitude, -90, 90);
				m_target.timeController.longitude = EditorGUILayout.Slider(new GUIContent("Longitude", "The east-west angle of a position on the Earth's surface."), m_target.timeController.longitude, -180, 180);
				m_target.timeController.utc = EditorGUILayout.Slider(new GUIContent("Utc", "Universal Time Coordinated."), m_target.timeController.utc, -12, 12);
				m_target.timeController.dayLength = EditorGUILayout.FloatField(new GUIContent("Day Length", "Duration of the day-night cycle in minutes."), m_target.timeController.dayLength);
				
				// Day and Night Length - Begin
				EditorGUILayout.BeginVertical("Box");
				
				// Toggle
				GUILayout.Space(-8);
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				GUILayout.Label("Day and Night Length", EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Set Time of Day by Curve?", "Will be the 'time of day' based on the timeline or based on the day-night length curve?"));
				m_target.timeController.setTimelineByCurve = EditorGUILayout.Toggle(m_target.timeController.setTimelineByCurve, GUILayout.Width(15));
				EditorGUILayout.EndHorizontal();
				
				// Reset Button
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("R", GUILayout.Width(25), GUILayout.Height(25)))
				{
					m_DayLengthCurve.animationCurveValue = AnimationCurve.Linear(0, 0, 24, 24);
				}
				
				// Curve field
				EditorGUILayout.CurveField(m_DayLengthCurve, Color.yellow, new Rect(0, 0, 24, 24), GUIContent.none, GUILayout.Height(25));
				EditorGUILayout.EndHorizontal();
				
				// Date and time display
				m_timeDisplay = m_target.timeController.GetTimeOfDay();
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Time of Day:", "Displays the 'time of day' based on the current 'time position' in the day-night cycle"));
				GUILayout.Label(m_target.timeController.CalendarWeekList[m_target.timeController.GetCurrentDayOfWeek()]
				                + " " + m_timeDisplay.x.ToString("00")
				                + ":" + m_timeDisplay.y.ToString("00"), GUILayout.ExpandWidth(false));
				EditorGUILayout.EndHorizontal();
				
				// Day and Night Length - End
				EditorGUILayout.EndVertical();
			}
			
			// References tab
			m_controlRect = EditorGUILayout.GetControlRect();
			GUI.Label(m_controlRect, "", m_headerStyle);
			if (GUI.Button(m_controlRect, "REFERENCES", m_headerStyle)) m_target.editorSettings.showReferencesTab = !m_target.editorSettings.showReferencesTab;
			GUI.Label(new Rect(m_controlRect.width - 40, m_controlRect.y, m_controlRect.width, 15), m_referencesFoldout);
			if (m_target.editorSettings.showReferencesTab)
			{
				// Sun transform
				GUILayout.Space(2);
				GUI.color = m_customGreenColor;
				if (!m_target.sunTransform) { GUI.color = m_customRedColor; }
				m_target.sunTransform = (Transform)EditorGUILayout.ObjectField(new GUIContent("Sun Transform", "The Transform used to simulate the sun position in the sky."), m_target.sunTransform, typeof(Transform), true);
				
				// Moon transform
				GUI.color = m_customGreenColor;
				if (!m_target.moonTransform) { GUI.color = m_customRedColor; }
				m_target.moonTransform = (Transform)EditorGUILayout.ObjectField(new GUIContent("Moon Transform", "The Transform used to simulate the moon position in the sky."), m_target.moonTransform, typeof(Transform), true);
				
				// Directional light
				GUI.color = m_customGreenColor;
				if (!m_target.directionalLight) { GUI.color = m_customRedColor; }
				m_target.directionalLight = (Light)EditorGUILayout.ObjectField(new GUIContent("Directional Light", "The directional light used to apply the lighting of the sun and moon in the scene."), m_target.directionalLight, typeof(Light), true);

				// Sky material
				GUI.color = m_customGreenColor;
				if (!m_target.skyMaterial) { GUI.color = m_customRedColor; }
				m_target.skyMaterial = (Material)EditorGUILayout.ObjectField(new GUIContent("Sky Material", "The material used to render the sky."), m_target.skyMaterial, typeof(Material), true);
				GUILayout.Space(2);
				
				// Sky material
				GUI.color = m_customGreenColor;
				if (!m_target.fogMaterial) { GUI.color = m_customRedColor; }
				m_target.fogMaterial = (Material)EditorGUILayout.ObjectField(new GUIContent("Fog Material", "The material used to render the fog scattering."), m_target.fogMaterial, typeof(Material), true);
				GUILayout.Space(2);
				
				// Sun texture
				m_controlRect = EditorGUILayout.GetControlRect();
				GUI.color = m_customGreenColor;
				if (!m_target.sunTexture) { GUI.color = m_customRedColor; }
				m_target.sunTexture = (Texture2D)EditorGUI.ObjectField(m_controlRect, new GUIContent("Sun Texture", "The texture used to render the sun disk in the sky."), m_target.sunTexture, typeof(Texture2D), true);
				GUILayout.Space(2);
				
				// Moon texture
				m_controlRect = EditorGUILayout.GetControlRect();
				GUI.color = m_customGreenColor;
				if (!m_target.moonTexture) { GUI.color = m_customRedColor; }
				m_target.moonTexture = (Texture2D)EditorGUI.ObjectField(m_controlRect, new GUIContent("Moon Texture", "The texture used to render the moon disk in the sky."), m_target.moonTexture, typeof(Texture2D), true);
				GUILayout.Space(2);
				
				// Dynamic cloud texture
				m_controlRect = EditorGUILayout.GetControlRect();
				GUI.color = m_customGreenColor;
				if (!m_target.dynamicCloudNoiseTexture) { GUI.color = m_customRedColor; }
				m_target.dynamicCloudNoiseTexture = (Texture2D)EditorGUI.ObjectField(m_controlRect, new GUIContent("Dynamic Cloud Texture", "The texture used to render the dynamic clouds."), m_target.dynamicCloudNoiseTexture, typeof(Texture2D), true);
				GUILayout.Space(2);
				
				// Star field texture
				m_controlRect = EditorGUILayout.GetControlRect();
				GUI.color = m_customGreenColor;
				if (!m_target.starFieldTexture) { GUI.color = m_customRedColor; }
				m_target.starFieldTexture = (Texture2D)EditorGUI.ObjectField(m_controlRect, new GUIContent("Starfield Texture", "The texture used to render the stars."), m_target.starFieldTexture, typeof(Texture2D), true);
				GUILayout.Space(2);
				
				// Star noise texture
				m_controlRect = EditorGUILayout.GetControlRect();
				GUI.color = m_customGreenColor;
				if (!m_target.starNoiseTexture) { GUI.color = m_customRedColor; }
				m_target.starNoiseTexture = (Cubemap)EditorGUI.ObjectField(m_controlRect, new GUIContent("Star Noise Texture", "The cubemap texture used to apply the star scintillation effect."), m_target.starNoiseTexture, typeof(Cubemap), true);
				GUILayout.Space(2);
				
				// Transmittance texture
				m_controlRect = EditorGUILayout.GetControlRect();
				GUI.color = m_customGreenColor;
				if (!m_target.precomputedTransmittanceTexture) { GUI.color = m_customRedColor; }
				m_target.precomputedTransmittanceTexture = (Texture2D)EditorGUI.ObjectField(m_controlRect, new GUIContent("Transmittance", "The Transmittance precomputed table."), m_target.precomputedTransmittanceTexture, typeof(Texture2D), true);
				GUILayout.Space(2);
				
				// In scatter texture
				m_controlRect = EditorGUILayout.GetControlRect();
				GUI.color = m_customGreenColor;
				if (!m_target.precomputedInScatterTexture) { GUI.color = m_customRedColor; }
				m_target.precomputedInScatterTexture = (Texture3D)EditorGUI.ObjectField(m_controlRect, new GUIContent("InScatter", "The InScatter precomputed table."), m_target.precomputedInScatterTexture, typeof(Texture3D), true);
				GUILayout.Space(2);

				// Reflection probe
				m_controlRect = EditorGUILayout.GetControlRect();
				GUI.color = m_customGreenColor;
				if (!m_target.reflectionProbe) { GUI.color = m_customRedColor; }
				m_target.reflectionProbe = (ReflectionProbe)EditorGUI.ObjectField(m_controlRect, new GUIContent("Reflection Probe", "The reflection probe used to compute the sky reflection"), m_target.reflectionProbe, typeof(ReflectionProbe), true);
				GUILayout.Space(2);
			}
			GUI.color = Color.white;
			
			// Profile Controller tab
			m_controlRect = EditorGUILayout.GetControlRect();
			GUI.Label(m_controlRect, "", m_headerStyle);
			if (GUI.Button(m_controlRect, "PROFILE CONTROLLER", m_headerStyle)) m_target.editorSettings.showProfileControllerTab = !m_target.editorSettings.showProfileControllerTab;
			GUI.Label(new Rect(m_controlRect.width - 40, m_controlRect.y, m_controlRect.width, 15), m_profileControllerFoldout);
			if (m_target.editorSettings.showProfileControllerTab)
			{
				//EditorGUILayout.ColorField(m_target.settings.testColor);
				
				// Climate zone trigger
				GUILayout.Space(2);
				EditorGUILayout.LabelField("", "", "RL DragHandle");
				GUILayout.Space(-10);
				GUI.color = m_customGreenColor;
				if (!m_target.profileController.weatherZoneTrigger) { GUI.color = m_customRedColor; }
				m_target.profileController.weatherZoneTrigger = (Transform)EditorGUILayout.ObjectField(
					new GUIContent("Local Zone Trigger", "Transform that will be drive the local weather zone blending feature. Setting this field to 'null' will disable local weather zones (global one will still work)."),
					m_target.profileController.weatherZoneTrigger,
					typeof(Transform), 
					true);
				GUI.color = Color.white;
				EditorGUILayout.LabelField("", "", "RL DragHandle");
				
				// Progress bar
				GUILayout.Space(-5);
				m_controlRect = EditorGUILayout.GetControlRect();
				EditorGUI.ProgressBar (new Rect(m_controlRect.x +1, m_controlRect.y, m_controlRect.width -2, m_controlRect.height -3), m_target.profileController.transitionProgress, "");
				EditorGUI.LabelField(new Rect(m_controlRect.x, m_controlRect.y -2, m_controlRect.width, m_controlRect.height), "Transition Progress", m_textBarStyle);
				
				// Draw custom header for the global climate reorderable list
				GUILayout.Space(-6);
				EditorGUILayout.BeginHorizontal("RL Header");
				m_controlRect = EditorGUILayout.GetControlRect();
				EditorGUI.LabelField(new Rect(m_controlRect.x +2,m_controlRect.y,m_controlRect.width,m_controlRect.height), new GUIContent("Global Weather Profiles", "Stores the profiles used to control the global climate."), EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal();
				
				// Draw the global climate reorderable list
				GUILayout.Space(2);
				m_reorderableWeatherProfileList.DoLayoutList();
				
				// Add the default profile options above the reorderable list elements
				EditorGUI.LabelField(new Rect(m_controlRect.x -3, m_controlRect.y +17, m_controlRect.width +6, m_controlRect.height +4), "", "", "CN EntryBackEven");
				GUI.color = new Color(1f, 1f, 1f, 0.5f);
				EditorGUI.LabelField(new Rect(m_controlRect.x -3, m_controlRect.y +36, m_controlRect.width +6, m_controlRect.height), "", "", "IN Title");
				EditorGUI.LabelField(new Rect(m_controlRect.x +1, m_controlRect.y +23, 10, m_controlRect.height), "", "", "RL DragHandle");
				GUI.color = Color.white;
				EditorGUI.LabelField(new Rect(m_controlRect.x +16, m_controlRect.y +18, 65, m_controlRect.height), "Profile -1");
				EditorGUI.LabelField(new Rect(m_controlRect.x +80, m_controlRect.y +18, m_controlRect.width -163, m_controlRect.height), "@Default", EditorStyles.textField);
				EditorGUI.PropertyField(new Rect(m_controlRect.x + m_controlRect.width - 63, m_controlRect.y +18, 28, m_controlRect.height), m_DefaultProfileTransition, GUIContent.none);
				// Go button
				if (GUI.Button(new Rect(m_controlRect.x + m_controlRect.width - 32, m_controlRect.y + 18, 30, m_controlRect.height), new GUIContent("Go", "Go back to the default profile current in use by sky manager.")))
				{
					if (Application.isPlaying)
					{
						m_target.SetNewWeatherProfile (-1);
					}
					else
					{
						Debug.Log("To perform a weather transition, the application must be playing.");
					}
				}
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				
				// Draw the default profiles reorderable list
				m_reorderableDefaultProfileList.DoLayoutList();
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				
				// Draw the local climate zones reorderable list
				m_reorderableWeatherZoneList.DoLayoutList();
			}
			
			// Event system tab
			m_controlRect = EditorGUILayout.GetControlRect();
			GUI.Label(m_controlRect, "", m_headerStyle);
			if (GUI.Button(m_controlRect, "EVENT SYSTEM", m_headerStyle)) m_target.editorSettings.showEventSystemTab = !m_target.editorSettings.showEventSystemTab;
			GUI.Label(new Rect(m_controlRect.width - 40, m_controlRect.y, m_controlRect.width, 15), m_eventSystemFoldout);
			if (m_target.editorSettings.showEventSystemTab)
			{
				EditorGUILayout.PropertyField(m_EventScanMode, new GUIContent("Scan Mode", "Sets when the 'Sky Manager' will scan the event list to check if it needs to invoke some event action."));
				m_reorderableEventSystemList.DoLayoutList();
			}

			// Options tab
			m_controlRect = EditorGUILayout.GetControlRect();
			GUI.Label(m_controlRect, "", m_headerStyle);
			if (GUI.Button(m_controlRect, "OPTIONS", m_headerStyle)) m_target.editorSettings.showOptionsTab = !m_target.editorSettings.showOptionsTab;
			GUI.Label(new Rect(m_controlRect.width - 40, m_controlRect.y, m_controlRect.width, 15), m_optionsFoldout);
			if (m_target.editorSettings.showOptionsTab)
			{
				// Sky model
				EditorGUILayout.BeginVertical("selectionRect");
				EditorGUILayout.PropertyField(m_SkyModel, new GUIContent("Sky Model", "Sets the sky model used to render the sky."));
				if (m_target.options.skyModel == AzureOptions.SkyModel.Stylized)
				{
					EditorGUILayout.PropertyField(m_StylizedSunsetMode, new GUIContent("Sunset Mode", "Sets the sunset mode used to render the stylized sky model. " +
					                                                                                  "If set to 'Custom Color', the sunset will be evaluated by the 'Sunset Color' property from the profile's 'Scattering' tab."));
				}
				else
				{
					EditorGUILayout.Slider(m_EarthScale, 0.0f, 1000.0f);
					EditorGUILayout.PropertyField(m_EarthPosition);
				}
				EditorGUILayout.EndVertical();
				
				// Cloud mode
				if (m_target.options.skyModel == AzureOptions.SkyModel.Stylized)
				{
					EditorGUILayout.BeginVertical("selectionRect");
					EditorGUILayout.PropertyField(m_CloudMode, new GUIContent("Cloud Mode", "Sets the cloud render mode, the performance will differ depending on the type of cloud used."));
					EditorGUILayout.EndVertical();
				}
				else
				{
					EditorGUILayout.BeginVertical("selectionRect");
					EditorGUILayout.PropertyField(m_PrecomputedCloudMode, new GUIContent("Cloud Mode", "Sets the cloud render mode, the performance will differ depending on the type of cloud used."));
					EditorGUILayout.EndVertical();
				}
				m_target.SetMaterialsShader();

				EditorGUILayout.BeginVertical("Box");
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(new GUIContent("Day Transition", "Performs a profile transition when starts a different day at midnight."));
				m_target.options.doDayTransition = EditorGUILayout.Toggle(m_target.options.doDayTransition, GUILayout.Width(15));
				EditorGUILayout.EndHorizontal();
				m_target.options.dayTransitionTime = EditorGUILayout.Slider(new GUIContent("Transition Time", "The time in seconds that the day transition will last."), m_target.options.dayTransitionTime, 0.0f, 60.0f);
				EditorGUILayout.EndVertical();
				
				// Reflection probe
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.PropertyField(m_ReflectionProbe, new GUIContent("Refl. Probe", "Enable or disable the sky reflection probe."));
                if (m_target.options.reflectionProbeMode == AzureOptions.ReflectionProbeMode.On)
                {
	                m_target.reflectionProbe.gameObject.SetActive(true);

                    // Refresh mode
                    EditorGUILayout.PropertyField(m_ReflectionProbeRefreshMode, new GUIContent("Refresh Mode", "Sets the reflection probe refresh mode. If 'Via Scripting' is set, the probe will be updated by the 'Sky Manager'."));
                    switch (m_target.options.reflectionProbeRefreshMode)
                    {
                        case UnityEngine.Rendering.ReflectionProbeRefreshMode.OnAwake:
	                        m_target.reflectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.OnAwake;
                            break;
                        case UnityEngine.Rendering.ReflectionProbeRefreshMode.EveryFrame:
	                        m_target.reflectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.EveryFrame;
                            break;
                        case UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting:
	                        m_target.reflectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
	                        
                            // Refresh time
	                        m_target.options.reflectionProbeUpdateTime = EditorGUILayout.FloatField(new GUIContent("Refresh Time", "Sets the update interval in seconds. If is zero, the sky reflection probe will be updated every frame."), m_target.options.reflectionProbeUpdateTime);
	                        
                            // Update at first frame?
	                        EditorGUILayout.BeginHorizontal();
	                        GUILayout.Label(new GUIContent("Update at Start?", "Updates the reflection probe in the first frame? If disabled, the reflection probe will be updated for the first time only after the interval set in refresh mode."));
	                        m_target.options.reflectionProbeUpdateAtFirstFrame = EditorGUILayout.Toggle(m_target.options.reflectionProbeUpdateAtFirstFrame, GUILayout.Width(15));
	                        EditorGUILayout.EndHorizontal();
                            EditorGUILayout.Space();
                            break;
                    }
					EditorGUILayout.HelpBox("Unity's GI is very expensive, so performance dropping is normal when the Reflection Probe is on. If you do not need Global Illumination, please turn the Reflection Probe off.",MessageType.Warning);
                }
                else
                {
	                m_target.reflectionProbe.gameObject.SetActive(false);
                }
                EditorGUILayout.EndVertical();
				
				// Star field position
				EditorGUILayout.BeginVertical("Box");
				GUILayout.Label("Starfield Position:");
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Position X");
				m_target.options.starFieldPosition.x = EditorGUILayout.Slider(m_target.options.starFieldPosition.x, -180.0f, 180.0f);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Position Y");
				m_target.options.starFieldPosition.y = EditorGUILayout.Slider(m_target.options.starFieldPosition.y, -180.0f, 180.0f);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Position Z");
				m_target.options.starFieldPosition.z = EditorGUILayout.Slider(m_target.options.starFieldPosition.z, -180.0f, 180.0f);
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.Space();
				GUILayout.Label("Starfield Color Balance:");
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Color R");
				m_target.options.starFieldColor.x = EditorGUILayout.Slider(m_target.options.starFieldColor.x, 1.0f, 2.0f);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Color G");
				m_target.options.starFieldColor.y = EditorGUILayout.Slider(m_target.options.starFieldColor.y, 1.0f, 2.0f);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Color B");
				m_target.options.starFieldColor.z = EditorGUILayout.Slider(m_target.options.starFieldColor.z, 1.0f, 2.0f);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			}
			
			// Outputs tab
			m_controlRect = EditorGUILayout.GetControlRect();
			GUI.Label(m_controlRect, "", m_headerStyle);
			if (GUI.Button(m_controlRect, "OUTPUT SYSTEM", m_headerStyle)) m_target.editorSettings.showOutputsTab = !m_target.editorSettings.showOutputsTab;
			GUI.Label(new Rect(m_controlRect.width - 40, m_controlRect.y, m_controlRect.width, 15), m_outputsFoldout);
			if (m_target.editorSettings.showOutputsTab)
			{
				// Output tag
				m_target.outputTag = EditorGUILayout.TagField(new GUIContent("Output Tag", "The Sky Manager will create and perform the outputs only to the profiles using this same tag."), m_target.outputTag);
				
				// Update profiles outputs
				if (GUILayout.Button("Apply Changes to Profiles"))
				{
					var guids = AssetDatabase.FindAssets("t:AzureSkyProfile");
					foreach (string guid in guids)
					{
						var temProfile =
							AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(AzureSkyProfile))
								as AzureSkyProfile;
						int outputCount = m_target.outputSystemList.Count;
						if (temProfile)
						{
							// Resize the profile's output list, update type end description
							if (temProfile.outputTag == m_target.outputTag)
							{
								if (outputCount <= 0)
								{
									temProfile.outputsList.Clear();
								}
								else
								{
									// Remove item
									while (temProfile.outputsList.Count > outputCount)
									{
										temProfile.outputsList.RemoveAt(temProfile.outputsList.Count - 1);
									}

									// Add item
									while (temProfile.outputsList.Count < outputCount)
									{
										temProfile.outputsList.Add
										(
											new AzureOutputProperty
											(
												AzureOutputType.Slider,
												0.0f,
												Vector2.zero,
												Vector3.zero,
												AnimationCurve.Linear(0.0f, 0.0f, 24.0f, 0.0f),
												AnimationCurve.Linear(-1.0f, 0.0f, 1.0f, 0.0f),
												AnimationCurve.Linear(-1.0f, 0.0f, 1.0f, 0.0f),
												Color.white,
												new Gradient(),
												new Gradient(),
												new Gradient()
											)
										);
									}

									for (int i = 0; i < outputCount; i++)
									{
										temProfile.outputsList[i].azureOutputType =
											m_target.outputSystemList[i].azureOutputType;
										temProfile.outputsList[i].description =
											m_target.outputSystemList[i].description;
									}
								}
							}
						}
					}
				}

				// Outputs list
				m_reorderableOutputSystemList.DoLayoutList();
				EditorGUILayout.Space();
			}
			
			if (!Application.isPlaying)
			{
				m_target.profileController.currentProfile = m_target.profileController.defaultProfileList[0];
				m_target.profileController.defaultProfile = m_target.profileController.defaultProfileList[0];
			}

			// End custom Inspector
			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(target);
		}
	}
}