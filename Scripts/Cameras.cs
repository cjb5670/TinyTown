using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cameras : MonoBehaviour {

	// Camera array that holds a refrence to every camera in the scene
	public List<Camera> cameras;

	// Current Camera
	public int currentCameraIndex;

	// Use this for initialization
	void Start()
	{
		currentCameraIndex = 0;
		// turn all cameras off, except the first default one
		for (int i = 1; i < cameras.Count;  i++)
		{
			cameras[i].gameObject.SetActive(false);
		}

		// If any cameras were added to the controller, enable the first one
		if (cameras.Count > 0)
		{
			cameras[0].gameObject.SetActive(true);
		}
	}

	// Update is called once per frame
	void Update()
	{

		// Press the F key to cycle through the cameras
		if (Input.GetKeyDown(KeyCode.F))
		{

			// Cycle to the next camera
			currentCameraIndex++;

			// If cameraIndex is in bounds, set this camera to active and the last one to inactive
			if (currentCameraIndex < cameras.Count)
			{
				cameras[currentCameraIndex - 1].gameObject.SetActive(false);
				cameras[currentCameraIndex].gameObject.SetActive(true);
			}

			// If last camera, cycle back to first camera
			else
			{
				cameras[currentCameraIndex - 1].gameObject.SetActive(false);
				currentCameraIndex = 0;
				cameras[currentCameraIndex].gameObject.SetActive(true);
			}
		}
	}
}
