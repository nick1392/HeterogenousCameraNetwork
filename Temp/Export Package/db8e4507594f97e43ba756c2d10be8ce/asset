using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public Bounds[] walls;
    public Transform ceilingTransform, floorTransform;
    public Bounds mapBounds;
    public static float cameraToWallMinDistance = 0.21f;

    private void InitializeMapDimensions() //gets map dimensions - works only for parallelepipeds as mapbounds
    {
        ceilingTransform = GameObject.Find("Ceiling").transform; floorTransform = GameObject.Find("Floor").transform; //set ceiling and floor transforms
        Vector3 center, size;
        center = new Vector3 (ceilingTransform.position.x, (ceilingTransform.position.y - floorTransform.position.y) / 2f, ceilingTransform.position.z);
        size = new Vector3(floorTransform.localScale.x, ceilingTransform.position.y - floorTransform.position.y, floorTransform.localScale.z);
        mapBounds = new Bounds(center, size);
        //Debug.Log(mapBounds);
    }

    private void PopulateWallsArray() //fills the walls array - uses Tag "Wall" to do so
    {
        GameObject[] wallObjects = GameObject.FindGameObjectsWithTag("Wall");
        walls = new Bounds[wallObjects.Length];
        for (int i = 0; i < walls.Length; i++)
        {
            walls[i] = new Bounds(wallObjects[i].transform.position, wallObjects[i].transform.localScale);
        }
    }

    private bool IsAtDistanceFromWalls(Vector3 coordinates) //true if coordinates are at a good distance from walls, defined by class attributes
    {
        Vector3 closestPoint;
        for (int i = 0; i < walls.Length; i++)
        {
            closestPoint = walls[i].ClosestPoint(coordinates);
            if (Vector3.Distance(coordinates, closestPoint) < cameraToWallMinDistance)
            {
                return false;
            }
        }
        return true;
    }


    //Unity methods
    private void OnEnable()
    {
        
        PopulateWallsArray();
        InitializeMapDimensions();
    }

    private void Start()
    {
        
    }
}
