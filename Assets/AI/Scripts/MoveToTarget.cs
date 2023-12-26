using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MoveToTarget : Agent
{
    [SerializeField] private Transform target;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(-8f, 0.3f, -1f);

        int random = Random.Range(0, 2);
        if (random == 0)
        {
            target.localPosition = new Vector3(-11f, 0.3f, -1f);
        }
        if (random == 1)
        {
            target.localPosition = new Vector3(-5f, 0.3f, -1f);
        }

        //base.OnEpisodeBegin();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);

        //base.CollectObservations(sensor);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float move = actions.ContinuousActions[0];
        float speed = 2f;

        transform.localPosition += new Vector3(move, 0f) * Time.deltaTime * speed;

        Debug.Log(actions.DiscreteActions[0]);
        //base.OnActionReceived(actions);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        //base.Heuristic(actionsOut);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Target")
        {
            AddReward(10f);
            EndEpisode();
        }
        if (other.gameObject.tag == "Wall")
        {
            AddReward(-5f);
            EndEpisode();
        }
    }
}