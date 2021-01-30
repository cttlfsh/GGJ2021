using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPMPlayerMove : MonoBehaviour
{
	CharacterController Controller;

	public float Speed;

	public Transform Cam;

	void Start()
	{
		Controller = GetComponent<CharacterController>();
	}

	void Update()
	{
		float Horizontal = Input.GetAxis("Horizontal") * Speed * Time.deltaTime;
		float Vertical = Input.GetAxis("Vertical") * Speed * Time.deltaTime;

		Vector3 Movement = Cam.transform.right * Horizontal + Cam.transform.forward * Vertical;
		Movement.y = 0f;

		Controller.Move(Movement);

		if (Movement.magnitude != 0f)
		{
			transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Cam.GetComponent<TPMCameraMove>().sensivity * Time.deltaTime);


			Quaternion CamRotation = Cam.rotation;
			CamRotation.x = 0f;
			CamRotation.z = 0f;

			transform.rotation = Quaternion.Lerp(transform.rotation, CamRotation, 0.1f);
		}
	}
}
