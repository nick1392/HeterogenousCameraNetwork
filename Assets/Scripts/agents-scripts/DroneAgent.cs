using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using MLAgents;
using UnityEngine;
using Random = UnityEngine.Random;

public class DroneAgent : Agent
{
    [Header("Specific to Drone cameras")] Rigidbody drone;

    // drone variables

    private float movementForwardSpeed = 5;
    private float movementRightSpeed = 5;

    // POI variables

    private float _angle;
    private bool showText = false;

    // Collision Sensing variables

    private int CollisionSensingDistance = 5;

    // Mission variables
[SerializeField]
    private float movementForwardSpeedMission = 20;
    private static float rotationDiff;
    private Vector3 nextPosition;
    private Quaternion nextRotation;
    private static float dist;
    private int coordup = 0;
    private bool mission = false;

    // Three step search variables

    private int windowSize1;
    private float time;


    private Bounds map;
    
    private Rigidbody _rb;
    private GridController _gridController;

    private void Awake()
    {
        _gridController = GameObject.Find("Map").GetComponent<GridController>();
        map = GameObject.Find("Floor").GetComponent<BoxCollider>().bounds;
    }

    public override void InitializeAgent()
    {
        drone = GetComponent<Rigidbody>();
        windowSize1 = 10;
    }
    

    float GetCellValue(Cell[,] grid, int x, int y)
    {
        if (x < 0 || y < 0 || x >= grid.GetLength(0) || y >= grid.GetLength(1))
            return 0;
        return grid[x, y].value;
    }

    (int x, int y) UpdateCoords()
    {
        int x_coord = 0, y_coord = 0;
        Vector3 onTheGroundProjection = map.ClosestPoint(drone.transform.position);
        for (int i = 0; i < _gridController.priorityGrid.GetLength(0); i++)
        {
            for (int j = 0; j < _gridController.priorityGrid.GetLength(1); j++)
            {
                if (_gridController.priorityGrid[i, j].Contains(onTheGroundProjection))
                {
                    x_coord = i;
                    y_coord = j;
                }
            }
        }

        return (x_coord, y_coord);
    }

    List<float> GetCells(Cell[,] grid, int x_coord, int y_coord)
    {
        return new List<float>()
        {
            GetCellValue(grid, x_coord - 1, y_coord - 1),
            GetCellValue(grid, x_coord - 1, y_coord),
            GetCellValue(grid, x_coord - 1, y_coord + 1),
            GetCellValue(grid, x_coord, y_coord),
            GetCellValue(grid, x_coord, y_coord - 1),
            GetCellValue(grid, x_coord, y_coord + 1),
            GetCellValue(grid, x_coord + 1, y_coord + 1),
            GetCellValue(grid, x_coord + 1, y_coord - 1),
            GetCellValue(grid, x_coord + 1, y_coord),
        };
    }
    
    public override void CollectObservations()
    {
        (int x_coord,int y_coord) = UpdateCoords();

        //P_t
        AddVectorObs(GetCells(_gridController.priorityGrid, x_coord, y_coord));
        //F_t
        AddVectorObs(GetCells(_gridController.overralConfidenceGrid, x_coord, y_coord));
    }

    private const int k_NoAction = 0; // do nothing!
    private const int k_Up = 1;
    private const int k_Down = 2;
    private const int k_Left = 3;
    private const int k_Right = 4;
    private const int k_TopRight = 5;
    private const int k_TopLeft = 6;
    private const int k_BottomLeft = 7;
    private const int k_BottomRight = 8;

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(-0.01f);
        var action = Mathf.FloorToInt(vectorAction[0]);

        int x = 0;
        int y = 0;

        switch (action)
        {
            case k_NoAction:
                x = y = 0;
                break;
            case k_Right:
                x = 1;
                break;
            case k_Left:
                x = -1;
                break;
            case k_Up:
                y = 1;
                break;
            case k_Down:
                y = -1;
                break;
            case k_TopRight:
                x = y = 1;
                break;
            case k_TopLeft:
                x = -1;
                y = 1;
                break;
            case k_BottomLeft:
                x = y = -1;
                break;
            case k_BottomRight:
                x = 1;
                y = -1;
                break;
            default:
                throw new ArgumentException("Invalid action value");
        }
        (int x_coord, int y_coord) = UpdateCoords();
        float reward = GetCells(_gridController.priorityGrid, x_coord, y_coord).Sum() -
                       GetCells(_gridController.priorityGrid, x_coord + x, y_coord + y).Sum();
        AddReward(reward);
        nextPosition = new Vector3(_gridController.timeConfidenceGrid[x_coord+x, y_coord+y].GetPosition().x,
            drone.transform.position.y, _gridController.timeConfidenceGrid[x_coord+x, y_coord+y].GetPosition().z);

        mission = true;
    }
    
    public override float[] Heuristic()
    {
        if (Input.GetKey(KeyCode.D))
            return new float[] { k_Right };
        if (Input.GetKey(KeyCode.W))
            return new float[] { k_Up };
        if (Input.GetKey(KeyCode.A))
            return new float[] { k_Left };
        if (Input.GetKey(KeyCode.X))
            return new float[] { k_Down };
        if (Input.GetKey(KeyCode.E))
            return new float[] { k_TopRight };
        if (Input.GetKey(KeyCode.Q))
            return new float[] { k_TopLeft };
        if (Input.GetKey(KeyCode.Z))
            return new float[] { k_BottomLeft };
        if (Input.GetKey(KeyCode.C))
            return new float[] { k_BottomRight };
        return new float[] { k_NoAction };
    }

    public void Update()
    {
        // Missione
        if (mission)
        {
            drone.transform.position = Vector3.MoveTowards(drone.transform.position, nextPosition,
                movementForwardSpeedMission * Time.deltaTime);
            dist = Vector3.Distance(drone.transform.position, nextPosition);
            if (dist == 0)
            {
                mission = false;
            }
        }
    }
    
    public override void AgentReset()
    {
        transform.position = new Vector3(Random.Range(-22f, 22f), 6.55f, Random.Range(-22f, 22f));
    }
}