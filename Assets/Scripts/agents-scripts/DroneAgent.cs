using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Geometry;
using MLAgents;
using UnityEngine;
using Random = UnityEngine.Random;

public class DroneAgent : Agent
{
    [Header("Specific to Drone cameras")] public CameraControllerFOV ccfov;

    Rigidbody drone;

    // drone variables
    private float movementForwardSpeed = 5;
    private float movementRightSpeed = 5;
    [SerializeField]private bool logReward;

    // POI variables

    private float _angle;
    private bool showText = false;

    // Collision Sensing variables

    private int CollisionSensingDistance = 5;

    // Mission variables
    [SerializeField] private float movementForwardSpeedMission = 20;
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
    public int VisionSize = 4;
    public RenderTexture rt;
    private Academy m_Academy;
    public float timeBetweenDecisionsAtInference;
    private float m_TimeSinceDecision;
    
    private void Awake()
    {
        _gridController = GameObject.Find("Map").GetComponent<GridController>();
        map = GameObject.Find("Floor").GetComponent<BoxCollider>().bounds;
    }

    public override void InitializeAgent()
    {
        drone = GetComponent<Rigidbody>();
        windowSize1 = 10;
        m_Academy = FindObjectOfType<Academy>();
//        RequestDecision();
    }


    float GetCellValue(Cell[,] grid, int x, int y)
    {
        if (x < 0 || y < 0 || x >= grid.GetLength(0) || y >= grid.GetLength(1))
            return 1;
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
        var list = new List<float>();
        var obsSize = VisionSize;
        for (int i = x_coord - obsSize; i <= x_coord + obsSize; i++)
        {
            for (int j = y_coord - obsSize; j <= y_coord + obsSize; j++)
                list.Add(GetCellValue(grid, i, j));
        }

        return list;
    }

    public override void CollectObservations()
    {
        (int x_coord, int y_coord) = UpdateCoords();
        _gridController.maskedx = x_coord;
        _gridController.maskedy = y_coord;
        for (int i = x_coord - 1; i <= x_coord + 1; i++)
        for (int j = y_coord - 1; j <= y_coord + 1; j++)
            if (i >= 0 && j >= 0 && i < _gridController.overralConfidenceGrid.GetLength(0) &&
                j < _gridController.overralConfidenceGrid.GetLength(0))
                _gridController.overralConfidenceGrid[i, j].UpdateColor();
        _gridController.overralConfidenceTexture.SetPixel(x_coord, y_coord, Color.yellow);
        _gridController.overralConfidenceTexture.Apply();
        Graphics.Blit(_gridController.overralConfidenceTexture, rt);
        SetActionMask(GetActionMask(x_coord, y_coord));
    }

    IEnumerable<int> GetActionMask(int x_coord, int y_coord)
    {
        var actionMask = new List<int>();
        if (x_coord + 1 >= _gridController.priorityGrid.GetLength(0))
            actionMask.AddRange(new[] {k_Right, k_TopRight, k_BottomRight});
        if (x_coord - 1 < 0)
            actionMask.AddRange(new[] {k_Left, k_TopLeft, k_BottomLeft});
        if (y_coord + 1 >= _gridController.priorityGrid.GetLength(1))
            actionMask.AddRange(new[] {k_Up, k_TopLeft, k_TopRight});
        if (y_coord - 1 < 0)
            actionMask.AddRange(new[] {k_Down, k_BottomLeft, k_BottomRight});
        return actionMask.Distinct();
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

    private float lastTime;

    private float lastGCM = -1;
    private int decisions = 0, requestedDecisions = 0;

    public bool training = false;

    private void OnDrawGizmos()
    {
        if (!_gridController)
            return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position,
            new Vector3(_gridController.cellWidth * VisionSize * 2, 0.1f, _gridController.cellDepth * VisionSize * 2));
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position,
            new Vector3(_gridController.cellWidth * 3, 0.1f, _gridController.cellDepth * 3));
    }

    [SerializeField]
    private int maxStepsCode;
    public override void AgentAction(float[] vectorAction)
    {
//        Debug.Log("Act " + (Time.time-lastTime));
        //    lastTime = Time.time;

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

        nextPosition = new Vector3(_gridController.timeConfidenceGrid[x_coord + x, y_coord + y].GetPosition().x,
            drone.transform.position.y, _gridController.timeConfidenceGrid[x_coord + x, y_coord + y].GetPosition().z);
        //

//        if (training)
//        {
        drone.transform.position = nextPosition;
        float max_steps = maxStepsCode + 1f;
        float grid_size = _gridController.priorityGrid.GetLength(0) * _gridController.priorityGrid.GetLength(1);
        float gen_reward = 1f / (max_steps + grid_size);
        if (_gridController.overralConfidenceGrid[x_coord + x, y_coord + y].value > 0)
            AddReward(-gen_reward);
        else if (_gridController.overralConfidenceGrid[x_coord + x, y_coord + y].value < 0.1)
            AddReward(gen_reward);
        if (decisions > grid_size)
            AddReward(-gen_reward);
//        AddReward(-1f/max_steps);
        ccfov.Project();
//        _gridController.();

        _gridController.UpdateGCMValues(); //We force the update of the values 
//        }

        mission = true;
        
        float gcm = _gridController.GlobalCoverageMetric_Current();
//       if (lastGCM != -1)
//       {
//        //    float reward = -1f + 2f * gcm;
//           AddReward(gcm - lastGCM);
        if (Math.Abs(gcm - 1) < 0.01f)
        {
//            AddReward(1f);
            AddReward(1-gen_reward*grid_size);
            Done();
        }
//       }


        if (logReward)
            Debug.Log("Agent step " + GetStepCount() + ": Reward " + GetReward() + "\nGCM: " + gcm);

        lastGCM = gcm;
        _gridController.currentTime++;
        decisions++;
//        if (decisions >= stepsMax)
//            Done();
    }

    public override float[] Heuristic()
    {
        float[] ret = {k_NoAction};
        if (Input.GetKey(KeyCode.D))
            ret = new float[] {k_Right};
        if (Input.GetKey(KeyCode.W))
            ret = new float[] {k_Up};
        if (Input.GetKey(KeyCode.A))
            ret = new float[] {k_Left};
        if (Input.GetKey(KeyCode.X))
            ret = new float[] {k_Down};
        if (Input.GetKey(KeyCode.E))
            ret = new float[] {k_TopRight};
        if (Input.GetKey(KeyCode.Q))
            ret = new float[] {k_TopLeft};
        if (Input.GetKey(KeyCode.Z))
            ret = new float[] {k_BottomLeft};
        if (Input.GetKey(KeyCode.C))
            ret = new float[] {k_BottomRight};
        (int x, int y) = UpdateCoords();
        if (GetActionMask(x, y).Contains((int) ret[0]))
            ret = new float[] {k_NoAction};
        return ret;
    }

/*    public void Update()
    {
        if (!training)
            if (mission)
            {
                drone.transform.position = Vector3.MoveTowards(drone.transform.position, nextPosition,
                    movementForwardSpeedMission * Time.deltaTime);
                dist = Vector3.Distance(drone.transform.position, nextPosition);
                if (Math.Abs(dist) < 0.01f)
                {
                    mission = false;
                    RequestDecision();
                    requestedDecisions++;
                }
            }
            else
            {
                RequestDecision();
                requestedDecisions++;
            }
    }*/
    public void FixedUpdate()
    {
        WaitTimeInference();
    }

    private void WaitTimeInference()
    {
        if (!m_Academy.GetIsInference())
        {
            RequestDecision();
        }
        else
        {
            if (m_TimeSinceDecision >= timeBetweenDecisionsAtInference)
            {
                m_TimeSinceDecision = 0f;
                RequestDecision();
            }
            else
            {
                m_TimeSinceDecision += Time.fixedDeltaTime;
            }
        }
    }

    public override void AgentReset()
    {
        if (!training)
            return;
        _gridController.Reset();
        ResetReward();
        decisions = requestedDecisions = 0;
        transform.position = new Vector3(Random.Range(-21f, 21f), 6.55f, Random.Range(-21f, 21f));
        lastGCM = -1;
        if (logReward)
        Debug.Log("#################### AGENT RESET ####################");
        base.AgentReset();
    }
}