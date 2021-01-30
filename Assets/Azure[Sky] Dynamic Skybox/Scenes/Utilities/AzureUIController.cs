using UnityEngine.UI;

namespace UnityEngine.AzureSky
{

	public class AzureUIController : MonoBehaviour
	{
		public AzureSkyManager azureSky;
		public Slider timelineSlider;
		public Slider altitudeSlider;
		private Camera m_mainCamera;
		private Vector3 m_cameraPosition;
		public RectTransform transition;
		private Vector3 m_transitionScale = Vector3.one;
		public AzureSkyEffects azureEffects;
		public Transform thunderSource;

		// Use this for initialization
		void Start()
		{
			m_mainCamera = Camera.main;
		}

		// Update is called once per frame
		void Update()
		{
			azureSky.timeController.timeline = timelineSlider.value;
			m_transitionScale.x = azureSky.profileController.transitionProgress;
			transition.localScale = m_transitionScale;

			if (altitudeSlider && m_mainCamera)
			{
				m_cameraPosition = new Vector3(m_mainCamera.transform.position.x, altitudeSlider.value * 10000.0f, m_mainCamera.transform.position.z);
				m_mainCamera.transform.position = m_cameraPosition;
			}
		}

		public void ChangeAzureWeather(int index)
		{
			if (!azureSky.profileController.isWeatherTransition)
			{
				azureSky.SetNewWeatherProfile(index);
			}
		}

		public void CreateAzureThunder(int index)
		{
			if (thunderSource && azureEffects)
			{
				azureEffects.InstantiateThunderEffect(index, thunderSource.position);
			}
		}
	}
}