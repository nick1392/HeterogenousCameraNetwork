using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonCollection : MonoBehaviour
{
	public static PersonCollection Instance;
	private void Awake()
	{
		People = new List<GameObject>();
		Instance = this;
	}

	public List<GameObject> People;
}
