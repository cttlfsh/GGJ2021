//Based on Unity's GlobalFog.
namespace UnityEngine.AzureSky
{
    [ImageEffectAllowedInSceneView]
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Azure[Sky]/Fog Scattering")]
    public class AzureSkyFogScattering : MonoBehaviour
    {
        public Material fogScatteringMaterial;
        private Camera m_camera = null;
        private Transform m_cameraTransform = null;
        private Vector3[] m_frustumCorners = new Vector3[4];
        private Rect m_rect = new Rect(0, 0, 1, 1);
        private Matrix4x4 m_frustumCornersArray;

        private static readonly int FrustumCorners = Shader.PropertyToID("_FrustumCorners");
        /*public LayerMask excludeLayers = 0;
        public CameraClearFlags clearFlags = CameraClearFlags.Nothing;
        private GameObject m_tmpCam;*/

        private void Start()
        {
            m_camera = GetComponent<Camera>();
            m_cameraTransform = m_camera.transform;
        }

        [ImageEffectOpaque] // Apply the fog scattering effect after opaque geometry but before transparent geometry.
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            m_camera.depthTextureMode |= DepthTextureMode.Depth;

            if (fogScatteringMaterial == null)
            {
                Graphics.Blit(source, destination);
                return;
            }

            m_camera.CalculateFrustumCorners(m_rect, m_camera.farClipPlane, m_camera.stereoActiveEye, m_frustumCorners);

            m_frustumCornersArray = Matrix4x4.identity;
            m_frustumCornersArray.SetRow(0, m_cameraTransform.TransformVector(m_frustumCorners[0]));  // bottom left
            m_frustumCornersArray.SetRow(2, m_cameraTransform.TransformVector(m_frustumCorners[1]));  // top left
            m_frustumCornersArray.SetRow(3, m_cameraTransform.TransformVector(m_frustumCorners[2]));  // top right
            m_frustumCornersArray.SetRow(1, m_cameraTransform.TransformVector(m_frustumCorners[3]));  // bottom right

            fogScatteringMaterial.SetMatrix(FrustumCorners, m_frustumCornersArray);
            Graphics.Blit(source, destination, fogScatteringMaterial, 0);

            //Exclude layers.
            /*if (Application.isPlaying)
            {
                //m_camera = null;
                if (excludeLayers.value != 0)
                {
                    m_camera = GetTmpCam();
                }

                if (m_camera && excludeLayers.value != 0)
                {
                    //cam.targetTexture = destination;
                    m_camera.cullingMask = excludeLayers;
                    //cam.Render();
                }
            }*/
        }

        //From: https://github.com/UnityCommunity/UnityLibrary/blob/master/Assets/Shaders/ImageEffects/GrayscaleLayers.cs
        /*Camera GetTmpCam()
        {
            if (m_tmpCam == null)
            {
                if (m_camera == null) m_camera = GetComponent<Camera>();

                string name = "Exclude Fog Scattering";
                GameObject go = GameObject.Find(name);

                if (go == null)//Couldn't find, recreate.
                {
                    m_tmpCam = new GameObject(name, typeof(Camera));
                }
                else
                {
                    m_tmpCam = go;
                }
            }

            m_tmpCam.transform.position = transform.position;
            m_tmpCam.transform.rotation = transform.rotation;
            m_tmpCam.transform.localScale = transform.localScale;
            m_tmpCam.transform.parent = this.transform;
            m_tmpCam.GetComponent<Camera>().CopyFrom(m_camera);

            //tmpCam.GetComponent<Camera>().enabled = false;
            m_tmpCam.GetComponent<Camera>().depthTextureMode = DepthTextureMode.None;
            m_tmpCam.GetComponent<Camera>().clearFlags = clearFlags;
            m_tmpCam.GetComponent<Camera>().depth = m_camera.depth + 1;

            return m_tmpCam.GetComponent<Camera>();
        }*/
    }
}