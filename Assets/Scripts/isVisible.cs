using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isVisible : MonoBehaviour {

	private GameObject[] persone;
	private string tagPerson = "Person";

	void Update()
	{
		persone = GameObject.FindGameObjectsWithTag(tagPerson);
		foreach (GameObject person in persone) {
			//person.GetComponentsInChildren
			if (person.GetComponentsInChildren<Renderer>()[0].IsVisibleFrom(GetComponentInParent<Camera>())) Debug.Log("Visible" + person.GetComponent<Person>().PersonID);
			//else Debug.Log("Not visible");
		}

	}
}