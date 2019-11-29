using System.Collections;
using System.Collections.Generic;
using MLAgents;
using MLAgents.InferenceBrain;
using MLAgents.Sensor;
using UnityEngine;

public class GridSensor : ISensor
{
    private GridController m_GridController;
    private bool m_Grayscale;
    private string m_Name;
    private int[] m_Shape;

    public GridSensor(GridController gridController, bool grayscale, string name)
    {
        m_GridController = gridController;
        m_Grayscale = grayscale;
        m_Name = name;
        m_Shape = new[] {grayscale ? 1 : 3};
    }

    public string GetName()
    {
        return m_Name;
    }

    public int[] GetFloatObservationShape()
    {
        return m_Shape;
    }

    public byte[] GetCompressedObservation()
    {
        using (TimerStack.Instance.Scoped("GridSensor.GetCompressedObservation"))
        {
            var texture = m_GridController.spatialConfidenceTexture;
            Debug.Log("compressed " + texture);

            var compressed = texture.EncodeToPNG();
//            UnityEngine.Object.Destroy(texture);
            return compressed;
        }
    }

    public void WriteToTensor(TensorProxy tensorProxy, int agentIndex)
    {
        using (TimerStack.Instance.Scoped("GridSensor.WriteToTensor"))
        {
            var texture = m_GridController.spatialConfidenceTexture;
            Debug.Log(texture);
            Utilities.TextureToTensorProxy(texture, tensorProxy, m_Grayscale, agentIndex);
//            UnityEngine.Object.Destroy(texture);
        }
    }

    public SensorCompressionType GetCompressionType()
    {
        return SensorCompressionType.PNG;
    }
}