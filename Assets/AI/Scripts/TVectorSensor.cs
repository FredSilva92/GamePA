using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class TVectorSensor : ISensor
{
    List<float> m_Observations;
    ObservationSpec m_ObservationSpec;
    string m_Name;

    public TVectorSensor(int observationSize, string name = null, ObservationType observationType = ObservationType.Default)
    {
        if (string.IsNullOrEmpty(name))
        {
            name = $"VectorSensor_size{observationSize}";
            if (observationType != ObservationType.Default)
            {
                name += $"_{observationType.ToString()}";
            }
        }

        m_Observations = new List<float>(observationSize);
        m_Name = name;
        m_ObservationSpec = ObservationSpec.Vector(observationSize, observationType);
    }

    public int Write(ObservationWriter writer)
    {
        var expectedObservations = m_ObservationSpec.Shape[0];

        if (m_Observations.Count > expectedObservations)
        {
            m_Observations.RemoveRange(expectedObservations, m_Observations.Count - expectedObservations);
        }
        else if (m_Observations.Count < expectedObservations)
        {
            for (int i = m_Observations.Count; i < expectedObservations; i++)
            {
                m_Observations.Add(0);
            }
        }

        writer.AddList(m_Observations);

        return expectedObservations;
    }

    public ReadOnlyCollection<float> GetObservations()
    {
        return m_Observations.AsReadOnly();
    }

    public string GetObservations(bool asString)
    {
        if (asString)
        {
            return string.Join(", ", GetObservations());
        }
        else
        {
            return GetObservations().ToString();
        }
    }

    public void Update()
    {
        Clear();
    }

    public void Reset()
    {
        Clear();
    }

    public ObservationSpec GetObservationSpec()
    {
        return m_ObservationSpec;
    }

    public string GetName()
    {
        return m_Name;
    }

    public virtual byte[] GetCompressedObservation()
    {
        return null;
    }

    public CompressionSpec GetCompressionSpec()
    {
        return CompressionSpec.Default();
    }

    public BuiltInSensorType GetBuiltInSensorType()
    {
        return BuiltInSensorType.VectorSensor;
    }

    void Clear()
    {
        m_Observations.Clear();
    }

    void AddFloatObs(float obs)
    {
        DebugCheckNanAndInfinity(obs, nameof(obs), nameof(AddFloatObs));
        m_Observations.Add(obs);
    }

    public void AddObservation(float observation)
    {
        AddFloatObs(observation);
    }

    public void AddObservation(int observation)
    {
        AddFloatObs(observation);
    }

    public void AddObservation(Vector3 observation)
    {
        AddFloatObs(observation.x);
        AddFloatObs(observation.y);
        AddFloatObs(observation.z);
    }

    public void AddObservation(Vector2 observation)
    {
        AddFloatObs(observation.x);
        AddFloatObs(observation.y);
    }

    public void AddObservation(IList<float> observation)
    {
        for (var i = 0; i < observation.Count; i++)
        {
            AddFloatObs(observation[i]);
        }
    }

    public void AddObservation(Quaternion observation)
    {
        AddFloatObs(observation.x);
        AddFloatObs(observation.y);
        AddFloatObs(observation.z);
        AddFloatObs(observation.w);
    }

    public void AddObservation(bool observation)
    {
        AddFloatObs(observation ? 1f : 0f);
    }

    public void AddOneHotObservation(int observation, int range)
    {
        for (var i = 0; i < range; i++)
        {
            AddFloatObs(i == observation ? 1.0f : 0.0f);
        }
    }

    internal void DebugCheckNanAndInfinity(float value, string valueCategory, string caller)
    {
        if (float.IsNaN(value))
        {
            throw new ArgumentException($"NaN {valueCategory} passed to {caller}.");
        }
        if (float.IsInfinity(value))
        {
            throw new ArgumentException($"Inifinity {valueCategory} passed to {caller}.");
        }
    }
}