using System.Collections;
using System.Collections.Generic;
using MLAgents.Sensor;
using UnityEngine;

public class GridSensorComponent : SensorComponent
{
    public GridController gridController;
    public string sensorName = "GridSensor";
    public bool grayscale = false;

    public override ISensor CreateSensor()
    {
        return new GridSensor(gridController, grayscale, sensorName);
    }
}
