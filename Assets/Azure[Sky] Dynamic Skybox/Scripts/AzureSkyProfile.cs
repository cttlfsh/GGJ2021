using System.Collections.Generic;

namespace UnityEngine.AzureSky
{
    [CreateAssetMenu(fileName = "Day Profile", menuName = "Azure[Sky] Dynamic Skybox/New Day Profile", order = 1)]
    public sealed class AzureSkyProfile : ScriptableObject
    {
        // Not included in build
        #if UNITY_EDITOR
            public AzureEditorSettings editorSettings;
        #endif

        public string outputTag = "Untagged";

        // Components
        public AzureScatteringComponent scattering = new AzureScatteringComponent();
        public AzureOuterSpaceComponent outerSpace = new AzureOuterSpaceComponent();
        public AzureLightingComponent lighting = new AzureLightingComponent();
        public AzureFogScatteringComponent fogScattering = new AzureFogScatteringComponent();
        public AzureCloudsComponent clouds = new AzureCloudsComponent();
        public AzureWeatherComponent weather = new AzureWeatherComponent();
        public List<AzureOutputProperty> outputsList = new List<AzureOutputProperty>();
    }
}