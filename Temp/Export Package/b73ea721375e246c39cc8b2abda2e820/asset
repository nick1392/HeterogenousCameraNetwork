using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class MovementDrone : MonoBehaviour {

	Rigidbody drone;
	public GameObject Camera;
	// drone variables

	private float movementForwardSpeed = 20;
	private float movementRightSpeed = 20;
	private float rotateAmountByKeys = 2.5f;
	private float upForce;

	private float wantedYRotation;
	private static float currentYRotation;
	private float rotationYVelocity;
	private Vector3 velocityToSmoothDampToZero;

	// camera control variables

	private static float wantedXRotation = 0;

	// Mission Select

	private static int modeFlag = 0;

	// Mission variables

	private float movementForwardSpeedMission = 20;
	private float rotateAmountMission = 2.5f;
	private static float rotationDiff;
	private Vector3 nextPosition;
	private Quaternion nextRotation;
	private static float dist;
	private int coordup = 0;
	private bool initializeMission = true;

	// POI variables

	private float _angle;
	private bool showText = false;

	// Collision Sensing variables

	private int CollisionSensingDistance = 5;

	// Reading from Files

	private float[] ParSet;
	private float[,] Mission;
	private float[] POI;


	/************************************
	 * 	         GetMission			 	*
	 *  Read mission coord form file 	*
	 * **********************************/

	float[,] GetMission(){
		string[] lines = System.IO.File.ReadAllLines(@"Assets/Scripts/mission.txt");
		float[,] tmp = new float[lines.Length, 5];
		for(int k=0; k<lines.Length; k++){
			string[] test = lines[k].Split (' ');
			for(int i=0; i<test.Length; i++){
				tmp[k,i] = float.Parse(test[i]);
			}
		}
		return tmp;
	}

	/************************************
	 * 	         	GetPOI			 	*
	 *    	Read POI coord form file 	*
	 * **********************************/

	float[] GetPOI(){
		string[] lines = System.IO.File.ReadAllLines(@"Assets/Scripts/POI.txt");
		if (lines.Length != 0 && lines.Length <= 1) {
			float[] tmp = new float[6];
			string[] test = lines [0].Split (' ');
			for (int i = 0; i < test.Length; i++) {
				tmp [i] = float.Parse (test [i]);
			}
			return tmp;
		} else {
			float[] tmp = new float[0];
			return tmp;
		}
	}

	/************************************
	 * 	          GetSettings		 	*
	 *    	  Read Setting form file	*
	 * **********************************/

	void GetSettings(){
		string[] lines = System.IO.File.ReadAllLines(@"Assets/Scripts/ParametersSetting.txt");
		if (lines.Length != 0 && lines.Length <= 1) {
			float[] tmp = new float[7];
			string[] test = lines [0].Split (' ');
			for (int i = 0; i < test.Length; i++) {
				tmp [i] = float.Parse (test [i]);
			}
			movementForwardSpeed = tmp[0];
			movementRightSpeed = tmp[1];
			rotateAmountByKeys = tmp[2];
			movementForwardSpeedMission = tmp[3];
			rotateAmountMission = tmp[4];
			modeFlag = (int)tmp[5];
			CollisionSensingDistance = (int)tmp[6];
		}
	}

	/************************************
	 * 	         	Awake			 	*
	 *  	Initalizate variables		*
	 * **********************************/

	void Awake () 
	{
		drone = GetComponent<Rigidbody> ();
		Mission = GetMission ();
		POI = GetPOI ();
		GetSettings();
	}

	void FixedUpdate ()
	{
		// Inizializza la flag: 0(default)=Guida, 1=Missione, 2=Point Of Interest
		if (Input.GetKey (KeyCode.M)) {
			modeFlag = 1;
		}
		else if (Input.GetKey (KeyCode.P)) {
			modeFlag = 2;
		}

		// Missione
		if (modeFlag == 1 && Mission.Length != 0 && coordup < Mission.GetLength(0) - 1) {
			if (initializeMission) {
				wantedXRotation = Mission [coordup, 4];
				drone.transform.position = new Vector3 (Mission [coordup, 0], Mission [coordup, 1], Mission [coordup, 2]);
				drone.transform.rotation = Quaternion.Euler (new Vector3 (0, Mission [coordup, 3], 0));
				currentYRotation = Mission [coordup, 3];
				DroneCameraController.SetInitMiss (true);
				initializeMission = false;
			} else {
//			currentPosition = new Vector3 (coord [coordup, 0], coord [coordup, 1], coord [coordup, 2]);
				nextPosition = new Vector3 (Mission [coordup + 1, 0], Mission [coordup + 1, 1], Mission [coordup + 1, 2]);
				drone.transform.position = Vector3.MoveTowards (drone.transform.position, nextPosition, movementForwardSpeedMission * Time.deltaTime);
				dist = Vector3.Distance (drone.transform.position, nextPosition);
				if (dist == 0) {
					nextRotation = Quaternion.Euler (0, Mission [coordup + 1, 3], 0);
					rotationDiff = Quaternion.Angle (drone.rotation, nextRotation);
					drone.transform.rotation = Quaternion.RotateTowards (drone.transform.rotation, nextRotation, rotateAmountMission * Time.deltaTime);
					currentYRotation = Mission [coordup + 1, 3];
					if (rotationDiff == 0) {
						DroneCameraController.SetCameraRot (true);
						wantedXRotation = Mission [coordup+1, 4];
						if (DroneCameraController.GetCameraStatus ()) {
							DroneCameraController.SetCameraRot (false);
							DroneCameraController.SetCameraStatus (false);
							coordup += 1;
							if (coordup == Mission.GetLength (0) - 1) {
								drone.transform.position = new Vector3 (Mission [coordup, 0], Mission [coordup, 1], Mission [coordup, 2]);
								wantedYRotation = Mission [coordup, 3];
								wantedXRotation = Mission [coordup, 4];
								modeFlag = 0;
								coordup = 0;
								initializeMission = true;
							}
						}
					}
				}
			}
		// Point Of Interest
		}else if (modeFlag == 2 && POI.Length != 0) {
			_angle += POI[5] * Time.deltaTime;
			drone.transform.position = new Vector3 (POI [0], POI [1], POI [2]) + new Vector3(Mathf.Sin(_angle) * POI[3], POI[1] ,Mathf.Cos(_angle)* POI[3]);
			Camera.transform.LookAt(new Vector3(POI[0],POI[4],POI[2])); 
			if (Input.GetKey (KeyCode.T)) {
				modeFlag = 0;
			}
		}else{
			MovementPitch ();
			MovementRoll ();
			MovementYaw ();
			ClampingSpeedValues ();
		}
		MovementUpDown ();
	}

	/********************************
	 * 	       MovementUpDown		*
	 *  Force proportional to mass	*
	 * ******************************/

	void MovementUpDown () { 
		if (Input.GetKey (KeyCode.I)) {
			upForce = 45;
		} else if (Input.GetKey (KeyCode.K)) {
			upForce = -20;
		} else if (!Input.GetKey (KeyCode.I) && !Input.GetKey (KeyCode.K)) {
			upForce = 9.81f;
		}
		drone.AddRelativeForce (Vector3.up * upForce);
	}

	/********************************
	 * 	       MovementPitch		*
	 *  							*
	 * ******************************/


	void MovementPitch () {
		if (Input.GetAxis ("Vertical") != 0) {
			Vector3 fwd = drone.transform.TransformDirection (Vector3.forward);
			RaycastHit hit;
			if (Physics.Raycast (drone.transform.position, fwd, out hit, CollisionSensingDistance)) {
				showText = true;
				drone.AddRelativeForce (Vector3.forward * -1 * (CollisionSensingDistance - hit.distance) * movementForwardSpeed);
			} else {
				showText = false;
                Debug.Log(Input.GetAxis("Vertical"));
				drone.AddRelativeForce (Vector3.forward * Input.GetAxis ("Vertical") * movementForwardSpeed);
			}				
		}
	}

	/********************************
	 * 	       MovemetRoll			*
	 *  							*
	 * ******************************/

	void MovementRoll () {
		if (Input.GetAxis ("Horizontal") != 0) {
			drone.AddRelativeForce (Vector3.right * Input.GetAxis ("Horizontal") * movementRightSpeed);
		}
	}

	/********************************
	 * 	       MovementYaw			*
	 *  							*
	 * ******************************/

	void MovementYaw () {
		if (Input.GetKey (KeyCode.J)) {
			wantedYRotation -= rotateAmountByKeys;
		}
		if (Input.GetKey (KeyCode.L)) {
			wantedYRotation += rotateAmountByKeys;
		}

		currentYRotation = Mathf.SmoothDamp (currentYRotation, wantedYRotation, ref rotationYVelocity, 0.25f);
		drone.rotation = Quaternion.Euler (new Vector3 (0, currentYRotation, 0));
	}

	/********************************
	 * 	       ClampingSpeed		*
	 *  Reduce speed to zero		*
	 * ******************************/


	void ClampingSpeedValues () {
		if (Mathf.Abs (Input.GetAxis ("Vertical")) > 0.2f && Mathf.Abs (Input.GetAxis ("Horizontal")) > 0.2f) {
			drone.velocity = Vector3.ClampMagnitude (drone.velocity, Mathf.Lerp (drone.velocity.magnitude, 10.0f, Time.deltaTime * 5f));
		}
		if (Mathf.Abs (Input.GetAxis ("Vertical")) > 0.2f && Mathf.Abs (Input.GetAxis ("Horizontal")) < 0.2f) {
			drone.velocity = Vector3.ClampMagnitude (drone.velocity, Mathf.Lerp (drone.velocity.magnitude, 10.0f, Time.deltaTime * 5f));
		}
		if (Mathf.Abs (Input.GetAxis ("Vertical")) < 0.2f && Mathf.Abs (Input.GetAxis ("Horizontal")) > 0.2f) {
			drone.velocity = Vector3.ClampMagnitude (drone.velocity, Mathf.Lerp (drone.velocity.magnitude, 10.0f, Time.deltaTime * 5f));
		}
		if (Mathf.Abs (Input.GetAxis ("Vertical")) < 0.2f && Mathf.Abs (Input.GetAxis ("Horizontal")) < 0.2f) {
			drone.velocity = Vector3.SmoothDamp (drone.velocity, Vector3.zero, ref velocityToSmoothDampToZero, 0.5f);
		}
	}

	public static float GetYrot(){
		return currentYRotation;
	}

	public static float GetXrot(){
		return wantedXRotation;
	}

	public static int GetMode(){
		return modeFlag;
	}

	public static float GetrotationDiff(){
		return rotationDiff;
	}

	public static float Getdist(){
		return dist;
	}

	void OnGUI () {
		if (showText) {
			GUI.Label(new Rect(100, 100, 400, 100), "Collision Alert!");
		}
	}
}
