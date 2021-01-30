using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPMCameraMove : MonoBehaviour
{
	private const float YMin = -50.0f;
	private const float YMax = 50.0f;

	public Transform lookAtClient;
	public Transform lookAtServer;
	private Transform lookAt;
	public bool isServer;

	public float distance = 10.0f;
	private float currentX = 0.0f;
	private float currentY = 0.0f;
	public float sensivity = 4.0f;

	void Start()
	{
	}

	public void setServer(bool val){
		isServer = val;
	}

	void LateUpdate()
	{
		if (isServer)
		{
			lookAt = lookAtServer;
		}
		else
		{
			lookAt = lookAtClient;
		}
		currentX += Input.GetAxis("Mouse X") * sensivity * Time.deltaTime;
		currentY += Input.GetAxis("Mouse Y") * sensivity * Time.deltaTime *(-1);

		currentY = Mathf.Clamp(currentY, YMin, YMax);

		Vector3 Direction = new Vector3(0, 0, -distance);
		Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
		transform.position = lookAt.position + rotation * Direction;

		transform.LookAt(lookAt.position);
	}
}