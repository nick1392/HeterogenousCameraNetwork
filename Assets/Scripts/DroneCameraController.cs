using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneCameraController : MonoBehaviour {

	Rigidbody Camera;
	public GameObject drone;

	private float currentYRotation;
	private float wantedXRotation = 90f;
	private float rotateXAmountByKeys = 0.8f;
	private float rotationXVelocity;

	private Quaternion nextRotation;
	private float rotationDiff;

	private static bool CameraStatus = false;

	private static bool initializeMission;

	private static bool CameraRot;

	void Start () {
		Camera = GetComponent<Rigidbody> ();
	}


	void FixedUpdate () {
		MovementPitchCamera();
		currentYRotation = MovementDrone.GetYrot(); // ricarico la CurrentYrotation del drone
		if (MovementDrone.GetMode() == 1) {
			if (initializeMission == true) {
				Camera.transform.rotation = Quaternion.Euler (new Vector3 (MovementDrone.GetXrot (), currentYRotation, 0));
				initializeMission = false;
			} else {
				if (CameraRot) {					
					nextRotation = Quaternion.Euler (MovementDrone.GetXrot (), currentYRotation, 0);
					rotationDiff = Quaternion.Angle (Camera.rotation, nextRotation);
					Camera.transform.rotation = Quaternion.RotateTowards (Camera.transform.rotation, nextRotation, 10 * Time.deltaTime);
					if (rotationDiff == 0) {
						CameraStatus = true;
					}
				}
			}
			//Camera.rotation = Quaternion.Euler (new Vector3 (wantedXRotationMission, currentYRotation, 0));
		} else {
			Camera.rotation = Quaternion.Euler (new Vector3 (wantedXRotation, currentYRotation, 0));
			Camera.position = drone.transform.position;
		}
	}

	public static bool GetCameraStatus(){
		return CameraStatus;
	}

	public static void SetCameraStatus(bool tmp){
		CameraStatus = tmp;
	}

	public static void SetCameraRot(bool tmp){
		CameraRot = tmp;
	}

	public static void SetInitMiss(bool tmp){
		initializeMission = tmp;
	}
		
	void MovementPitchCamera () {
		if (Input.GetKey (KeyCode.E)) {
			wantedXRotation -= rotateXAmountByKeys;
			if (wantedXRotation<0){
				wantedXRotation = 0;
			}

		}
		if (Input.GetKey (KeyCode.Q)) {
			wantedXRotation += rotateXAmountByKeys;
			if (wantedXRotation>90){
				wantedXRotation = 90;
			}
		}
	}
}
