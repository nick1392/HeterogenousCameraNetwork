using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Geometry;
using Containers;


public class GridController : MonoBehaviour {

    public int currentTime; //current observation step id
    public int t_max = 10; //time after which I have 0 confidence on the previous observation

	public bool logMetrics = false;
    public bool plotMaps = false;

    public Cell[,] observationGrid, timeConfidenceGrid, spatialConfidenceGrid, overralConfidenceGrid, overralConfidenceGridTime, overralConfidenceGridNewObs, spatialConfidenceGridNewObs, timeConfidenceGridNewObs, lastObsGrid, observationGridNewObs;

    public Cell[,] priorityGrid;

    public float cellWidth = 1f, cellDepth = 1f;

    public float ped_max = 5;

    [Range(0.0f,1.0f)]
    public float alfa = 0.5f;


	// Variable for metrics
	public float spatialThreshold = 0.2f;
	private float GCM, PCM;
    private int numberOfCellsWidth, numberOfCellsDepth;

	public float peopleThreshold = 0.2f;
	private int peopleTimeCount, peopleHistory, peopleCovered;
	public List<GameObject> people = new List<GameObject>();
	private List<GameObject> totalPeople = new List<GameObject>();
	private List<Vector3> groundProjections = new List<Vector3>();
	private Bounds mapVolume;
	private Bounds map;

    private Text map_name;

    private void InitializeGrid(ref Cell[,] grid, bool plot, Vector2 factor) //intialise grid values
    {
        MapController map = GameObject.Find("Map").GetComponent<MapController>();
        
        numberOfCellsWidth = (int)Mathf.Round(map.mapBounds.size.x / cellWidth);
        numberOfCellsDepth = (int)Mathf.Round(map.mapBounds.size.z / cellDepth);

        cellWidth = map.mapBounds.size.x / numberOfCellsWidth;
        cellDepth = map.mapBounds.size.z / numberOfCellsDepth;

        grid = new Cell[numberOfCellsWidth, numberOfCellsDepth];
        for (int i = 0; i < numberOfCellsWidth; i++)
        {
            for (int j = 0; j < numberOfCellsDepth; j++)
            {
                grid[i, j] = new Cell(new Vector2((cellWidth / 2f + i * cellWidth) - map.mapBounds.extents.x + map.mapBounds.center.x, (cellDepth / 2f + j * cellDepth) - map.mapBounds.extents.z + map.mapBounds.center.z), new Vector2(cellWidth, cellDepth), plot, factor, new Vector2 (map.mapBounds.size.x, map.mapBounds.size.z));
            }
        }

        //Debug.Log(observationGrid[numberOfCellsWidth -1, numberOfCellsDepth-1].value);
        //Debug.Log("here");
    }

    private void UpdateTimeConfidenceGrid()
    {
        //Debug.Log("time"+currentTime);
        //Debug.Log(lastObsGrid[0, 0].value);
        for (int i = 0; i < numberOfCellsWidth; i++)
        {
            for (int j = 0; j < numberOfCellsDepth; j++)
            {
                
                float check_pos = currentTime - lastObsGrid[i, j].value;
                
                //Debug.Log(check_pos);
                //Debug.Log(t_max);
                float value = 0;
                if (check_pos == 0)
                {
                    value = 1;
                    //Debug.Log(value);
                }
                if (check_pos < t_max)
                {
                    value = 1 - ((check_pos) / t_max);
                    //Debug.Log("value = " + value);
                }
                else if (check_pos > t_max)
                {
                    value = 0;
                    //Debug.Log("value = " + value);
                }


                timeConfidenceGrid[i, j].SetValue(value);
                
            }
        }
    }

    public void UpdateTimeConfidenceGridNewObs(int i, int j)
    {
        timeConfidenceGridNewObs[i, j].SetValue(1);
        //Debug.Log("timeConfidenceGridNewObs = " + timeConfidenceGridNewObs[i, j].value);
    }

    public void UpdateSpatialConfidenceGridNewObs(int i, int j, float conf)
    {
        //Debug.Log(conf);
        if (spatialConfidenceGridNewObs[i, j].value < conf)
        {
            spatialConfidenceGridNewObs[i, j].SetValue(conf);
            //Debug.Log("spatialConfidenceGridNewObs = " + spatialConfidenceGridNewObs[i, j].value);
        }
    }

    private void UpdateOverralConfidenceGridNewObs()
    {
        for (int i = 0; i < numberOfCellsWidth; i++)
        {
            for (int j = 0; j < numberOfCellsDepth; j++)
            {
                overralConfidenceGridNewObs[i, j].SetValue(timeConfidenceGridNewObs[i, j].value * spatialConfidenceGridNewObs[i, j].value);
            }
        }
    }

    private void UpdateOverralConfidenceGridTime()
    {
        for (int i = 0; i < numberOfCellsWidth; i++)
        {
            for (int j = 0; j < numberOfCellsDepth; j++)
            {

                if (timeConfidenceGrid[i, j].value == 0 || spatialConfidenceGrid[i, j].value == 0)
                {
                    overralConfidenceGridTime[i, j].SetValue(0);
                    //Debug.Log(" overrall confidence (time) value = " + overralConfidenceGridTime[i, j].value);
                    //Debug.Log(" timeConfidenceGrid value = " + timeConfidenceGrid[i, j].value);
                    //Debug.Log(" spatialConfidenceGrid value = " + spatialConfidenceGrid[i, j].value);
                }
                else
                {
                    overralConfidenceGridTime[i, j].SetValue(timeConfidenceGrid[i, j].value * spatialConfidenceGrid[i, j].value);
                    //Debug.Log(" overrall confidence (time) value = " + overralConfidenceGridTime[i, j].value);
                }
                
              
            }
        }
    }

    public void UpdateObservationGridNewObs()
    {
        for (int i = 0; i < numberOfCellsWidth; i++)
        {
            for (int j = 0; j < numberOfCellsDepth; j++)
            {
                observationGridNewObs[i, j].SetValue(observationGridNewObs[i, j].value / ped_max);
            }
        }
    }

    public void CountObservationGridNewObs(int i, int j)
    {
        observationGridNewObs[i, j].SetValue(observationGridNewObs[i, j].value + 1);
        //Debug.Log("new obs value = " + observationGridNewObs[i, j].value);
    }

    private void UpdatePriorityGrid()
    {
        for (int i = 0; i < numberOfCellsWidth; i++)
        {
            for (int j = 0; j < numberOfCellsDepth; j++)
            {
				priorityGrid[i, j].SetValue(alfa * observationGrid[i,j].value + (1 - alfa)*0.5f);// (1 - alfa) * (1 - overralConfidenceGrid[i,j].value));
            }
        }
    }

	private void GlobalCoverageMetric()
	{
		int conf = 0;
		for (int i = 0; i < numberOfCellsWidth; i++)
		{
			for (int j = 0; j < numberOfCellsDepth; j++)
			{
				if (overralConfidenceGrid [i, j].value > spatialThreshold) 
				{
					conf += 1;
				}
			}
		}

		GCM = GCM + ((conf*100)/(numberOfCellsWidth * numberOfCellsDepth)); //(GCM + conf / numberOfCellsWidth * numberOfCellsDepth)/(currentTime + 1);
		if(logMetrics) Debug.Log ("GCM = " + GCM/(currentTime + 1));
	}

	private void PeopleCoverageMetric()
	{

		GameObject[] peop = GameObject.FindGameObjectsWithTag("Person");
		int conf = 0;

		foreach (GameObject child in peop) 
		{
			if (mapVolume.Contains(child.transform.position)) 
			{
				totalPeople.Add (child);

				groundProjections.Add (map.ClosestPoint(child.transform.position));
				//map.ClosestPoint(child.transform.position);
				//observationGrid[i,j].Contains(;
			}
		}

		if(totalPeople.Count != 0)
		{

			foreach (Vector3 pos in groundProjections) 
			{
				for (int i = 0; i < numberOfCellsWidth; i++)
				{
					for (int j = 0; j < numberOfCellsDepth; j++)
					{
						if (observationGrid[i,j].Contains(pos) && overralConfidenceGrid [i, j].value > peopleThreshold) 
						{
							conf += 1;
						}
					}
				}
			}



			//Debug.Log ("tot people count =" + totalPeople.Count);
			peopleHistory = peopleHistory + totalPeople.Count;
			peopleCovered = peopleCovered + conf;

			PCM = 100*(float)peopleCovered/(float)peopleHistory;
			peopleTimeCount = peopleTimeCount + 1;
			if(logMetrics) Debug.Log ("PCM = " + PCM);
		}
	}


    private void ResetNewObs()
    {
        for (int i = 0; i < numberOfCellsWidth; i++)
        {
            for (int j = 0; j < numberOfCellsDepth; j++)
            {
                observationGridNewObs[i, j].SetValue(0);
                overralConfidenceGridNewObs[i, j].SetValue(0);
                spatialConfidenceGridNewObs[i, j].SetValue(0);
                timeConfidenceGridNewObs[i, j].SetValue(0);
            }
        }
    }

    //Unity methods
    private void OnEnable()
    {
;
    }

    private void Start()
    {
        InitializeGrid(ref observationGrid, plotMaps, new Vector2(-1f, 0f));
        InitializeGrid(ref observationGridNewObs, plotMaps, new Vector2(-1f, 1f));        
        InitializeGrid(ref timeConfidenceGrid, plotMaps, new Vector2(0f,0f));
        InitializeGrid(ref timeConfidenceGridNewObs, plotMaps, new Vector2(0f, 1f));
        InitializeGrid(ref spatialConfidenceGrid, plotMaps, new Vector2(1f, 0f));
        InitializeGrid(ref spatialConfidenceGridNewObs, plotMaps, new Vector2(1f, 1f));
        InitializeGrid(ref overralConfidenceGrid, plotMaps, new Vector2(2f, 0f));
        InitializeGrid(ref overralConfidenceGridNewObs, plotMaps, new Vector2(2f, 1f));
        InitializeGrid(ref overralConfidenceGridTime, plotMaps, new Vector2(2f, -1f));
        InitializeGrid(ref priorityGrid, plotMaps, new Vector2(-2f, 0f));

        InitializeGrid(ref lastObsGrid, false, new Vector2(0f, 0f));
        //GenerateCameras()

		mapVolume = GameObject.Find("Map").GetComponent<MapController>().mapBounds;
		map = GameObject.Find ("Floor").GetComponent<BoxCollider> ().bounds;

        currentTime = 0;
		GCM = 0;
		PCM = 0;
		peopleHistory = 0;
		peopleCovered = 0;
		peopleTimeCount = 0;
    }

    private void Update()
    {
        currentTime++;
        //Debug.Log("current" + currentTime );
        //reset all newObs grids before new time step
    }

    private void LateUpdate()
    {
        UpdateTimeConfidenceGrid();
        UpdateObservationGridNewObs();
        UpdateOverralConfidenceGridTime();
        UpdateOverralConfidenceGridNewObs();

        for (int i = 0; i < numberOfCellsWidth; i++)
        {
            for (int j = 0; j < numberOfCellsDepth; j++)
            {
                if (overralConfidenceGridTime[i, j].value != 0)
                {
                    //Debug.Log("newobs = " + overralConfidenceGridNewObs[i, j].value);
                    //Debug.Log("Time = " + overralConfidenceGridTime[i, j].value);
                }

                if (overralConfidenceGridNewObs[i, j].value >= overralConfidenceGridTime[i, j].value)
                {
                    if(overralConfidenceGridNewObs[i, j].value != 0)
                    {
                        lastObsGrid[i, j].SetValue(currentTime);
                    }
                    
                    overralConfidenceGrid[i, j].SetValue(overralConfidenceGridNewObs[i, j].value);
                    observationGrid[i, j].SetValue(observationGridNewObs[i, j].value);
                    spatialConfidenceGrid[i, j].SetValue(spatialConfidenceGridNewObs[i, j].value);

                    //Debug.Log("newobs");
                }
                else
                {
                    overralConfidenceGrid[i, j].SetValue(overralConfidenceGridTime[i, j].value);
                    //Debug.Log("HEREEEEEE: Time");
                }

                priorityGrid[i, j].SetValue(alfa * observationGrid[i, j].value + (1 - alfa) * (1 - overralConfidenceGrid[i, j].value));

                if(plotMaps)
                {
                    priorityGrid[i, j].UpdateColorPriority();

                    observationGrid[i, j].UpdateColor();
                    timeConfidenceGrid[i, j].UpdateColor();
                    spatialConfidenceGrid[i, j].UpdateColor();
                    overralConfidenceGrid[i, j].UpdateColor();
                    overralConfidenceGridTime[i, j].UpdateColor();
                    overralConfidenceGridNewObs[i, j].UpdateColor();
                    spatialConfidenceGridNewObs[i, j].UpdateColor();
                    timeConfidenceGridNewObs[i, j].UpdateColor();
                    observationGridNewObs[i, j].UpdateColor();
                }


                //lastObsGrid,
            }
        }

		GlobalCoverageMetric ();
		PeopleCoverageMetric ();
		people.Clear ();
		totalPeople.Clear ();
		groundProjections.Clear ();

        ResetNewObs();

    }
}
