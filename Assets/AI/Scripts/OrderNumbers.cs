using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class OrderNumbers : Agent
{
    private List<float> _numbers;
    private int _currentIndex;

    private int _firstInput;
    private int _secondInput;

    /*
     * Initialize the agent.
     */
    public override void Initialize()
    {
        // Initialize variables or load any necessary resources here
    }

    /*
     * Called when a new episode begins.
     */
    public override void OnEpisodeBegin()
    {
        // Generate a random array of numbers
        _numbers = GenerateRandomNumbers(1, 10); // Generate numbers from 1 to 9

        // Reset the current index to the start
        _currentIndex = 0;

        Random.InitState((int)System.DateTime.Now.Ticks);

        _firstInput = Random.Range(1, 10);
        _secondInput = Random.Range(1, 10);
        Debug.Log("1º Numero escolhido: " + _firstInput);
        Debug.Log("2º Numero escolhido: " + _secondInput);

        string numbersString = string.Join(" ", _numbers);
        Debug.Log(numbersString);

        Debug.Log("OnEpisodeBegin");

        // Set the agent's starting position or state
        // (You might want to set the position of the numbers in the environment)


        //transform.localPosition = new Vector3(-8f, 0.3f, -1f);

        //int random = Random.Range(0, 2);
        //if (random == 0)
        //{
        //    target.localPosition = new Vector3(-11f, 0.3f, -1f);
        //}
        //if (random == 1)
        //{
        //    target.localPosition = new Vector3(-5f, 0.3f, -1f);
        //}

        //base.OnEpisodeBegin();
    }

    private List<float> GenerateRandomNumbers(int minValue, int maxValue)
    {
        List<float> randomNumbers = new List<float>();
        for (int i = minValue; i <= maxValue; i++)
        {
            randomNumbers.Add(i);
        }

        ShuffleList(randomNumbers);
        return randomNumbers;
    }

    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;

        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /*
     * Collect observations from the environment.
     */
    public override void CollectObservations(VectorSensor sensor)
    {
        // Provide the array of numbers as observations to the agent
        for (int i = 0; i < _numbers.Count; i++)
        {
            sensor.AddObservation(_numbers[i]);
        }

        Debug.Log("CollectObservations");

        //sensor.AddObservation(transform.localPosition);
        //sensor.AddObservation(target.localPosition);

        //base.CollectObservations(sensor);
    }

    /*
     * Process actions received from the neural network.
     */
    public override void OnActionReceived(ActionBuffers actions)
    {
        int action = actions.DiscreteActions[0];

        if (_currentIndex < _numbers.Count - 1 && action == 1)
        {
            float temp = _numbers[_currentIndex];
            _numbers[_currentIndex] = _numbers[_currentIndex + 1];
            _numbers[_currentIndex + 1] = temp;

            _currentIndex++;
        }

        if (IsSorted(_numbers))
        {
            Debug.Log("Recompensa positiva");
            SetReward(1f);
            EndEpisode();

            string numbersString = string.Join(" ", _numbers);
            Debug.Log(numbersString);
        }
        else
        {
            Debug.Log("Recompensa negativa");
            AddReward(-0.001f);
            EndEpisode();
        }

        Debug.Log("OnActionReceived");

        //if (_currentIndex < _numbers.Count - 1)
        //{
        //    float temp = _numbers[_currentIndex];
        //    _numbers[_currentIndex] = _numbers[_currentIndex + 1];
        //    _numbers[_currentIndex + 1] = temp;

        //    _currentIndex++;
        //}

        //if (IsSorted(_numbers))
        //{
        //    SetReward(1f);
        //    Debug.Log("Recompensa positiva");
        //    EndEpisode();

        //    string numbersString = string.Join(" ", _numbers);
        //    Debug.Log(numbersString);
        //}
        //else
        //{
        //    AddReward(-0.001f);
        //    Debug.Log("Recompensa negativa");
        //}

        //float move = actions.ContinuousActions[0];
        //float speed = 2f;

        //transform.localPosition += new Vector3(move, 0f) * Time.deltaTime * speed;

        //base.OnActionReceived(actions);
    }

    /*
     * Optionally handle when the agent takes a step in the environment.
     */
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Provide a simple heuristic for manual control (optional)
        //ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        //discreteActions[0] = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));

        //ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        //continuousActions[0] = Input.GetAxisRaw("Horizontal");

        //base.Heuristic(actionsOut);
    }

    /*
     * Optionally handle rewards and episode termination.
     */
    public void Update()
    {
        //// Example: Provide a reward when the array is successfully organized
        //if (_currentIndex == _numbers.Count - 1)
        //{
        //    // Check if the numbers are sorted
        //    if (IsSorted(_numbers))
        //    {
        //        SetReward(1f);
        //        Debug.Log("Recompensa positiva");
        //    }
        //    else
        //    {
        //        SetReward(-1f);
        //        Debug.Log("Recompensa negativa");
        //    }

        //    EndEpisode();
        //    string numbersString = string.Join(" ", _numbers);
        //    Debug.Log(numbersString);
        //}
    }

    private bool IsSorted(List<float> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            if (list[i] > list[i + 1])
            {
                return false;
            }
        }

        return true;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //if (other.gameObject.tag == "Target")
    //{
    //    AddReward(10f);
    //    EndEpisode();
    //}
    //if (other.gameObject.tag == "Wall")
    //{
    //    AddReward(-5f);
    //    EndEpisode();
    //}
    //}
}