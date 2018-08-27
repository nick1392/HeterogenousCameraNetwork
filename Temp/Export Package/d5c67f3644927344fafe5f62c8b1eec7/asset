using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
//using OpenCVForUnityExample;
using UnityEngine.SceneManagement;


public class CameraControllerPTZ : MonoBehaviour {

    public float tempo;
	public Camera cam;

	public Greedy_PTZ c;
    public float x_coordinate;
    public float y_coordinate;

    public bool enableTracking = false;
    public bool loop;

    public float speedZoomIn = 0.3f;
	public float speedZoomOut = 0.3f;

	public float verSpeed = 2.0F;
	public float horSpeed = 2.0F;

    private float damping = 1;

    public Transform target;

    public float posInizx;
    public float posInizy;
    private float posInizz;
    private float posInizFOV;

    private Vector3 starting_position;
    private float starting_FOV;
    public bool backToStart = false;

    private float xRotate;
	private float yRotate;
	private float zRotate;

	private Vector3 currentRotation;

	private float minX;
	private float maxX;
	private float minY;
	private float maxY;

	private float height0;

	private int counter;

    private Vector3 offset;

    private void Start()
	{
		counter = 0;
		height0 = 0;
        Vector3 currentRotation = transform.rotation.eulerAngles;
        posInizx = currentRotation.x;
        posInizy = currentRotation.y;
        posInizz = currentRotation.z;
        posInizFOV = GetComponent<Camera>().fieldOfView;
        //Debug.Log("x " + posInizx + ", y " + posInizy + ", z " + posInizz + ", zoom" + posInizFOV);


        Vector3 point = new Vector3(Screen.width/2, Screen.height/2, 0);
        Ray ray = cam.ScreenPointToRay(point);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.green);
        Vector3 point2 = new Vector3(point.x, point.y, hit.distance);
        starting_position = cam.ScreenToWorldPoint(point2);
        starting_FOV = cam.fieldOfView;
        //Debug.Break();
    }
    
    void LateUpdate()
	{

		//*********** rotazione camera ***************
		float rotationX = Input.GetAxis ("Vertical") * verSpeed;
		float rotationY = Input.GetAxis ("Horizontal") * horSpeed;

		rotationX *= Time.deltaTime;
		rotationY *= Time.deltaTime;
		transform.Rotate (-rotationX, 0, 0);                                 //rotazione x
		transform.Rotate (0, rotationY, 0);                                  //rotazione y
		Vector3 currentRotation = transform.rotation.eulerAngles;
		currentRotation.z = Mathf.Clamp (currentRotation.z, 0f, 0f);     //limite rotazione z ( rotation = 0)
		transform.rotation = Quaternion.Euler (currentRotation);



		Vector3 objPosition = Greedy_PTZ.FindObjectOfType<Greedy_PTZ>().GetComponent<Greedy_PTZ>().nextPosition;
		//Debug.Log("objPosition" + objPosition);
		//objPosition.y = 0;

		var rotation = Quaternion.LookRotation(objPosition - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);

	}
}