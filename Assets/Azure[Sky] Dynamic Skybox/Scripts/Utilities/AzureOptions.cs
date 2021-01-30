using System;

namespace UnityEngine.AzureSky
{
    [Serializable]
    public sealed class AzureOptions
    {
        public enum SkyModel
        {
            Stylized,
            Precomputed
        }
        public SkyModel skyModel = SkyModel.Stylized;
        
        public enum StylizedSunsetMode
        {
            Automatic,
            CustomColor
        }
        public StylizedSunsetMode stylizedSunsetMode = StylizedSunsetMode.Automatic;
        
        public enum CloudMode
        {
            Off,
            Static,
            StaticMultiLayer,
            Dynamic
        }
        public CloudMode cloudMode = CloudMode.Off;

        public enum PrecomputedCloudMode
        {
            Off
        }
        public PrecomputedCloudMode precomputedCloudMode = PrecomputedCloudMode.Off;

        public bool doDayTransition = true;
        public float dayTransitionTime = 5.0f;
        
        public enum ReflectionProbeMode
        {
            Off,
            On
        }
        public ReflectionProbeMode reflectionProbeMode = ReflectionProbeMode.Off;
        public Rendering.ReflectionProbeRefreshMode reflectionProbeRefreshMode = Rendering.ReflectionProbeRefreshMode.ViaScripting;
        public float reflectionProbeUpdateTime = 0.0f;
        public bool reflectionProbeUpdateAtFirstFrame = true;
        
        public Vector3 starFieldPosition = Vector3.zero;
        public Vector3 starFieldColor = Vector3.one;

        public float earthScale = 1000.0f;
        public Vector3 earthPosition = new Vector3(0.0f, 6360.0f, 0.0f);
    }
}