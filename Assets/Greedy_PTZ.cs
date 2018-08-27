using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Geometry;

public class Greedy_PTZ : MonoBehaviour {


	public Vector3 nextPosition;

	public GameObject cam;

	// Three step search variables

	private int windowSize1;
	private int time;

	private float startingRotation;


	// Use this for initialization
	void Start () {
		startingRotation = cam.transform.rotation.eulerAngles.y;
		Debug.Log (startingRotation);
	}

	void Awake()
	{
		windowSize1 = 10;
	}

	// Update is called once per frame
	void Update ()
	{

		time = GameObject.Find("Map").GetComponent<GridController>().currentTime;

		if (time % 2 == 0) 
		{
			ThreeStepSearch();
		}
			
	}


	void ThreeStepSearch()
	{

		Bounds map = GameObject.Find("Floor").GetComponent<BoxCollider>().bounds;

		Vector3 onTheGroundProjection = map.ClosestPoint(cam.transform.position);

		Cell[,] grid = GameObject.Find("Map").GetComponent<GridController>().priorityGrid;
		Cell[,] gridConf = GameObject.Find("Map").GetComponent<GridController>().overralConfidenceGrid;

		//declaring a fictious grid and initializing it, for quality of view of this camera
		float[,] proposedGrid = new float[grid.GetLength(0), grid.GetLength(1)];
		for (int i = 0; i < proposedGrid.GetLength(0); i++)
		{
			for (int j = 0; j < proposedGrid.GetLength(1); j++)
			{
				proposedGrid[i, j] = 0f;
			}
		}

		int x_coord = 0, y_coord = 0;

		for (int i = 0; i < proposedGrid.GetLength(0); i++)
		{
			for (int j = 0; j < proposedGrid.GetLength(1); j++)
			{
				if (grid[i, j].Contains(onTheGroundProjection))
				{
					x_coord = i;
					y_coord = j;
				}
			}
		}
			
		int x_min = 0;
		int y_min = 0;
		int x_max = 0;
		int y_max = 0;

		if ((startingRotation <= 45 && startingRotation > 0) || (startingRotation >= 315 && startingRotation < 360)) 
		{
			x_min = x_coord - windowSize1;
			y_min = y_coord;
			x_max = x_coord + windowSize1;
			y_max = y_coord + windowSize1;
			//Debug.Log("if1");
		}
		else if (startingRotation <= 135 && startingRotation > 45) 
		{
			x_min = x_coord;
			y_min = y_coord - windowSize1;
			x_max = x_coord + windowSize1;
			y_max = y_coord + windowSize1;
			//Debug.Log("if2");
		}
		else if (startingRotation <= 225 && startingRotation > 135) 
		{
			x_min = x_coord - windowSize1;
			y_min = y_coord - windowSize1;
			x_max = x_coord + windowSize1;
			y_max = y_coord;
			//Debug.Log("if3");
		}
		else if (startingRotation < 315 && startingRotation > 225) 
		{
			x_min = x_coord - windowSize1;
			y_min = y_coord - windowSize1;
			x_max = x_coord;
			y_max = y_coord + windowSize1;
			//Debug.Log("if4");
		}

		//Debug.Log("x_coord = " + x_coord);
		//Debug.Log("y_coord = " + y_coord);


		int x_min_search = 0, y_min_search = 0, x_max_search = 0, y_max_search = 0;

		if (x_min < 0) x_min = 0;
		if (y_min < 0) y_min = 0;
		if (x_max > proposedGrid.GetLength(0)) x_max = proposedGrid.GetLength(0);
		if (y_max > proposedGrid.GetLength(1)) y_max = proposedGrid.GetLength(1);

		//        Debug.Log("x_ min = " + x_min);
		//        Debug.Log("y_ min = " + y_min);
		//        Debug.Log("x_ max = " + x_max);
		//        Debug.Log("y_ max = " + y_max);

		List<Vector2Int> max_index = new List<Vector2Int>();
		float max_value = 0;

		//float[,] window1 = new float[2*windowSize1 + 1, 2*windowSize1 + 1];
		for (int i = x_min; i < x_max; i++)
		{
			for (int j = y_min; j < y_max; j++)
			{
				//window1[i - x_min, j - y_min] = proposedGrid[i, j];

				int search1 = (int)(windowSize1 / 5);

				x_min_search = i - search1;
				y_min_search = j - search1;
				x_max_search = i + search1;
				y_max_search = j + search1;

				if (x_min_search < 0) x_min_search = 0;
				//Debug.Log("check if ceil or floor" + search1);
				if (y_min_search < 0) y_min_search = 0;
				if (x_max_search > proposedGrid.GetLength(0)) x_max_search = proposedGrid.GetLength(0);
				if (y_max_search > proposedGrid.GetLength(1)) y_max_search = proposedGrid.GetLength(1);

				for (int i_search = x_min_search; i_search < x_max_search; i_search++)
				{
					for (int j_search = y_min_search; j_search < y_max_search; j_search++)
					{
						proposedGrid[i, j] = proposedGrid[i, j] + Mathf.Abs( gridConf[i,j].value - grid[i, j].value); 
					}
				}
				if (proposedGrid[i, j] > max_value)
				{

					max_index.Clear ();
					max_value = proposedGrid [i, j];
					max_index.Add (new Vector2Int (i, j));

				}
				if (proposedGrid [i, j] == max_value) 
				{
					max_index.Add (new Vector2Int (i, j));
				}
				//Debug.Log("i = " + i + ", j =" + j + ", value = " +  proposedGrid[i, j]);
			}
		}


		Vector2 index = max_index[Random.Range(0,max_index.Count)];

		//Debug.Log("i = " + index.x + ", j =" + index.y + ", value = " +  proposedGrid[(int)index.x, (int)index.y]);
		Cell[,] grid_position = GameObject.Find("Map").GetComponent<GridController>().timeConfidenceGrid;

		nextPosition = new Vector3(grid_position[(int)index.x, (int)index.y].GetPosition().x, grid_position[(int)index.x, (int)index.y].GetPosition().y, grid_position[(int)index.x, (int)index.y].GetPosition().z);
		//Debug.Log ("nextpos = " + nextPosition);
	}


}
