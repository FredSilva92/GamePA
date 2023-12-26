using Unity.MLAgents.Sensors;
using UnityEngine;

[AddComponentMenu("TVector Sensor", (int)MenuGroupAI.Sensors)]
public class VectorSensorComponent : SensorComponent
{
    public string SensorName
    {
        get { return m_SensorName; }
        set { m_SensorName = value; }
    }

    [HideInInspector, SerializeField]
    private string m_SensorName = "TVectorSensor";

    public int ObservationSize
    {
        get { return m_ObservationSize; }
        set { m_ObservationSize = value; }
    }

    [HideInInspector, SerializeField]
    int m_ObservationSize;

    [HideInInspector, SerializeField]
    ObservationType m_ObservationType;

    TVectorSensor m_Sensor;

    public ObservationType ObservationType
    {
        get { return m_ObservationType; }
        set { m_ObservationType = value; }
    }

    public override ISensor[] CreateSensors()
    {
        m_Sensor = new TVectorSensor(m_ObservationSize, m_SensorName, m_ObservationType);
        return new ISensor[] { m_Sensor };
    }

    public TVectorSensor GetSensor()
    {
        return m_Sensor;
    }
}