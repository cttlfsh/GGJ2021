using System;

namespace UnityEngine.AzureSky
{
    [Serializable]
    public sealed class AzureEditorSettings
    {
        // Profiles
        public bool showScatteringTab = true;
        public bool showOuterSpaceTab = false;
        public bool showFogScatteringTab = false;
        public bool showCloudsTab = false;
        public bool showLightingTab = false;
        public bool showWeatherTab = false;
        
        public bool showOutputsTab = false;
        
        // Sky Manager
        public bool showTimeControllerTab = true;
        public bool showReferencesTab = false;
        public bool showProfileControllerTab = false;
        public bool showEventSystemTab = false;
        public bool showOptionsTab = false;
    }
}